using HarmonyLib;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    public class WingedFlyer : Thing, IThingHolder
    {
        private ThingOwner<Pawn> container;
        public IntVec3 start, dest;
        public int ticks, ticksTotal;

        private Graphic graphic;

        public Pawn InnerPawn => container.OfType<Pawn>().FirstOrFallback();
        public float Speed => InnerPawn?.def.GetModExtension<WingedFlyerExtension>()?.flightSpeed ?? 1f;
        public List<WingedFlyerVariant> Variants => InnerPawn?.def.GetModExtension<WingedFlyerExtension>()?.variants;
        public GraphicData DefaultGraphicData => InnerPawn.def.GetModExtension<WingedFlyerExtension>()?.flyingGraphicData;

        public static bool IsCellValid(IntVec3 cell, Map map, bool endpoint)
        {
            if (!cell.InBounds(map))
            {
                return false;
            }

            if (!cell.Walkable(map))
            {
                return false;
            }

            if (endpoint)
            {
                if (cell.Roofed(map))
                {
                    return false;
                }
            }

            return true;
        }

        public WingedFlyer() => container = new ThingOwner<Pawn>(this);

        public void GetChildHolders(List<IThingHolder> outChildren) => ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Notify_DestinationUpdated();
            }
        }

        public ThingOwner GetDirectlyHeldThings() => container;

        public override Graphic Graphic
        {
            get
            {
                if (graphic == null)
                {
                    GraphicData data = DefaultGraphicData;

                    if (data == null)
                    {
                        if (InnerPawn == null)
                        {
                            return BaseContent.BadGraphic;
                        }
                        return InnerPawn.Graphic;
                    }

                    if (Variants != null)
                    {
                        graphic = Variants.Where(variant => variant.variantPath == InnerPawn.Drawer.renderer.graphics.nakedGraphic.path).Select(variant => variant.variantData.GetGraphic(DefaultGraphicData.Graphic)).FirstOrFallback();

                        if (graphic == null)
                        {
                            Log.Warning("Unable to find flying variant for [" + InnerPawn.Drawer.renderer.graphics.nakedGraphic.path + "]");
                            graphic = data.GraphicColoredFor(InnerPawn);
                        }
                    }
                    else
                    {
                        graphic = data.GraphicColoredFor(InnerPawn);
                    }
                }
                return graphic;
            }
        }

        public override Vector3 DrawPos
        {
            get
            {
                Vector3 pos = Vector3.Lerp(start.ToVector3(), dest.ToVector3(), (float)ticks / (float)ticksTotal);
                pos.y += def.Altitude;
                return pos;
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (InnerPawn != null)
            {
                float rotation = (Mathf.Atan2(dest.z - start.z, dest.x - start.x) * Mathf.Rad2Deg) - 90f;
                Graphic.Draw(drawLoc, flip ? Rotation.Opposite : Rotation, InnerPawn, -rotation);
            }
        }

        public override void Tick()
        {
            if (InnerPawn == null)
            {
                Destroy();
                return;
            }

            if (!InnerPawn.def.HasModExtension<WingedFlyerExtension>())
            {
                Log.Error($"Pawn {InnerPawn.LabelCap} of def {InnerPawn.def.defName} not capable of flight (needs WingedFlyerExtension)");
                Drop();
                return;
            }

            if (Position == dest)
            {
                //Reached destination.
                Drop();
            }
            else
            {
                Update();

                if (container.Any)
                {
                    container.ThingOwnerTick();
                }
            }

            ticks++;
        }

        public virtual void Notify_DestinationUpdated()
        {
            start = Position;
            ticks = 0;
            ticksTotal = (Position.DistanceTo(dest) / Speed).SecondsToTicks();
        }

        protected virtual void RespawnPawn()
        {
            Pawn pawn = InnerPawn;
            container.Clear();
            GenSpawn.Spawn(pawn, Position, Map);
            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
        }

        public virtual void Drop()
        {
            if (InnerPawn != null)
            {
                RespawnPawn();
            }
            Destroy();
        }

        protected virtual void Update()
        {
            if (!IsCellValid(dest, Map, true))
            {
                //Destination became invalid, find a nearby valid cell.
                if (!RCellFinder.TryFindRandomCellNearWith(dest, cell => IsCellValid(cell, Map, true), Map, out dest))
                {
                    //Can't find a nearby valid cell.
                    Drop();
                    return;
                }
                Notify_DestinationUpdated();
            }

            Position = Vector3.Lerp(start.ToVector3(), dest.ToVector3(), (float)ticks / (float)ticksTotal).ToIntVec3();
            //Rotation = Rot4.FromAngleFlat(Position.AngleFlat - dest.AngleFlat);
        }

        public static WingedFlyer MakeFlyer(Pawn pawn, TargetInfo dest)
        {
            if (!pawn.def.HasModExtension<WingedFlyerExtension>())
            {
                Log.Error($"Pawn {pawn.LabelCap} of def {pawn.def.defName} not capable of flight (needs WingedFlyerExtension)");
                return null;
            }

            if (!IsCellValid(pawn.Position, pawn.Map, true))
            {
                Log.Error("Pawn can't fly indoors/under a roof");
                return null;
            }

            IntVec3 pos = pawn.Position;
            Map map = pawn.Map;

            WingedFlyer flyer = (WingedFlyer)ThingMaker.MakeThing(DD_ThingDefOf.WingedFlyer);

            flyer.start = pos;
            flyer.dest = dest.Cell;

            List<Pawn> affectedPawns = TryCollectRelatedPawns(pawn, new List<Pawn>());
            affectedPawns.ForEach(p => p.DeSpawn());
            if (affectedPawns.All(p => flyer.container.TryAddOrTransfer(p)))
            {
                GenSpawn.Spawn(flyer, pos, map);

                return flyer;
            }
            else
            {
                Log.Error("Unable to add " + pawn.LabelCap + " and related pawns to Flyer");

                //Respawn
                flyer.container.Clear();
                affectedPawns.ForEach(p => GenSpawn.Spawn(p, pos, map));

                return null;
            }
        }

        private static List<Pawn> TryCollectRelatedPawns(Pawn pawn, List<Pawn> collection)
        {
            if (pawn == null)
            {
                return collection;
            }

            if (!collection.Contains(pawn))
            {
                collection.Add(pawn);

                //Type giddyBaseType = Type.GetType("GiddyUpCore.Base, GiddyUpCore"); //Base
                //Type giddyStorage = Type.GetType("GiddyUpCore.Storage.ExtendedDataStorage, GiddyUpCore"); //ExtendedDataStorage
                //Type giddyPawn = Type.GetType("GiddyUpCore.Storage.ExtendedPawnData, GiddyUpCore"); //ExtendedPawnData

                //if (giddyBaseType == null || giddyStorage == null || giddyPawn == null)
                //{
                //    return collection;
                //}

                //PropertyInfo giddyBaseInstance = AccessTools.Property(giddyBaseType, "Instance"); //Base.Instance
                //MethodInfo giddyBaseStorage = AccessTools.Method(giddyBaseType, "GetExtendedDataStorage"); //Base.Instance.GetExtendedDataStorage()
                //FieldInfo giddyStoreList = AccessTools.Field(giddyStorage, "_extendedPawnDataWorkingList"); //ExtendedDataStorage._extendedPawnDataWorkingList
                //FieldInfo giddyMount = AccessTools.Field(giddyPawn, "mount"); //ExtendedPawnData.mount

                //if (giddyBaseInstance == null || giddyBaseStorage == null || giddyStoreList == null || giddyMount == null)
                //{
                //    return collection;
                //}

                //var objBase = giddyBaseInstance.GetValue(null);
                //var objStorage = giddyBaseStorage.Invoke(objBase, null);
                //IList objList = giddyStoreList.GetValue(objStorage) as IList;

                //if (objBase == null || objStorage == null || objList == null)
                //{
                //    return collection;
                //}

                //foreach (var data in objList)
                //{
                //    Pawn p = giddyMount.GetValue(data) as Pawn;
                //    if (p == pawn)
                //    {
                //        TryCollectRelatedPawns(p, collection);
                //    }
                //}
            }
            return collection;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref container, "container", this);
            Scribe_Values.Look(ref start, "startPoint");
            Scribe_Values.Look(ref dest, "endPoint");
            Scribe_Values.Look(ref ticks, "currentTick", 0);
            Scribe_Values.Look(ref ticksTotal, "lengthTicks", 0);
        }
    }
}
