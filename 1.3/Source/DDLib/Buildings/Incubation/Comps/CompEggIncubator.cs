using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public abstract class CompEggIncubator : ThingComp
    {
        private static FieldInfo ProgressField = AccessTools.Field(typeof(CompHatcher), "gestateProgress");

        private bool allowGrowth = true;

        private int factionTick = 0;

        private CompPowerTrader power;
        private CompRefuelable fuel;

        protected CompHatcher Hatcher
        {
            get
            {
                Thing egg = Egg;
                if (egg != null)
                {
                    return egg.TryGetComp<CompHatcher>();
                }
                return null;
            }
        }
        protected CompPowerTrader CompPower
        {
            get
            {
                if (power == null)
                {
                    power = parent.TryGetComp<CompPowerTrader>();
                }
                return power;
            }
        }
        protected CompRefuelable CompFuel
        {
            get
            {
                if (fuel == null)
                {
                    fuel = parent.TryGetComp<CompRefuelable>();
                }
                return fuel;
            }
        }

        public CompProperties_EggIncubator Props => props as CompProperties_EggIncubator;
        public bool AllowAdditionalGrowth => allowGrowth;

        public abstract Thing Egg { get; }
        public abstract Thing RetrieveEgg();
        public abstract void HatchEgg();
        public abstract void GrowthTick();

        public virtual void IncubateEgg(Thing egg)
        {
            factionTick = 0;
        }

        public virtual float BaseProgress
        {
            get
            {
                CompHatcher hatcher = Hatcher;
                if (hatcher != null)
                {
                    return 1f / (hatcher.Props.hatcherDaystoHatch * 60000f);
                }
                return 0;
            }
        }

        public virtual float Progress
        {
            get
            {
                CompHatcher hatcher = Hatcher;
                if (hatcher != null)
                {
                    return (float)ProgressField.GetValue(hatcher);
                }
                return -1f;
            }
            set
            {
                CompHatcher hatcher = Hatcher;
                if (hatcher != null)
                {
                    if (Props.debug)
                    {
                        float oldV = (float)ProgressField.GetValue(hatcher);
                        float newV = Mathf.Clamp01(value);
                        float deltaV = newV - oldV;

                        if (oldV != newV)
                        {
                            Log.Message(oldV + " -> " + newV + " | Δ: " + deltaV + " θ: " + (1f - oldV) / deltaV + " ~ " + GenTicks.TicksToSeconds(Mathf.RoundToInt((1f - oldV) / deltaV)) + " seconds");
                        }
                    }
                    ProgressField.SetValue(hatcher, Mathf.Clamp01(value));
                }
            }
        }

        public virtual bool IsIncubatingEgg()
        {
            return Egg != null;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (CompPower != null && !CompPower.PowerOn)
            {
                //Has CompPowerTrader, but is powered off
                return;
            }

            if (CompFuel != null && !CompFuel.HasFuel)
            {
                //Has CompRefuelable, but no fuel.
                return;
            }

            if (Hatcher != null)
            {
                if (Hatcher.hatcheeFaction != parent.Faction)
                {
                    //Part of a different faction (Should block it from changing factions after being set once)
                    if(Hatcher.hatcheeFaction != null)
                    {
                        //Change to wild straight away.
                        Hatcher.hatcheeFaction = null;
                    }

                    if(Props.factionCheckInterval != null && Props.factionChangeInterval != null)
                    {
                        //Configured.
                        if (factionTick % Props.factionCheckInterval.Ticks == 0)
                        {
                            //Try changing factions.
                            if (Rand.Chance((float)factionTick / (float)Props.factionChangeInterval.Ticks))
                            {
                                //Successfully changed.
                                Hatcher.hatcheeFaction = parent.Faction;
                            }
                        }

                        factionTick++;
                    }
                }
            }

            if (AllowAdditionalGrowth)
            {
                GrowthTick();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref allowGrowth, "allowGrowth", true);
            Scribe_Values.Look(ref factionTick, "factionTick", 0);
        }

        public override string CompInspectStringExtra()
        {
            Thing egg = Egg;
            string text = "";

            if (IsIncubatingEgg())
            {
                text += "Egg: " + egg.LabelCap.CapitalizeFirst() + "\n";
                text += egg.GetInspectString() + "\n";
            }
            if (CompPower != null && !CompPower.PowerOn)
            {
                text += "Device is inactive.\n";
            }
            if (CompFuel != null && !CompFuel.HasFuel)
            {
                text += "Device has no fuel.\n";
            }

            return text.TrimEndNewlines();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Command_Toggle toggle_Grow = new Command_Toggle
            {
                defaultLabel = (allowGrowth ? "Accelerated Growth" : "Vanilla Growth"),
                defaultDesc = (allowGrowth ? "Speeds up the growth rate of the egg" : "Revert egg growth rate to normal speed"),
                hotKey = KeyBindingDefOf.Command_ItemForbid,
                icon = (allowGrowth ? TexCommand.ForbidOff : TexCommand.ForbidOn),
                isActive = (() => allowGrowth),
                toggleAction = () => allowGrowth = !allowGrowth
            };
            yield return toggle_Grow;

            if (IsIncubatingEgg())
            {
                if (Prefs.DevMode)
                {
                    Command_Action action_DebugReset = new Command_Action
                    {
                        defaultLabel = "Debug: Reset Progress",
                        action = () => Progress = 0
                    };
                    yield return action_DebugReset;

                    Command_Action action_DebugTick = new Command_Action
                    {
                        defaultLabel = "Debug: Force Tick",
                        action = () => GrowthTick()
                    };
                    yield return action_DebugTick;

                    Command_Action action_DebugHatch = new Command_Action
                    {
                        defaultLabel = "Debug: Hatch Now",
                        action = () => HatchEgg()
                    };
                    yield return action_DebugHatch;
                }
            }
        }

    }

    public class CompEggIncubator_Nest : CompEggIncubator
    {
        public override Thing Egg
        {
            get
            {
                if (parent != null)
                {
                    if (parent.Spawned && parent.Position != null && parent.Map != null)
                    {
                        return parent.Position.GetFirstThingWithComp<CompHatcher>(parent.Map);
                    }
                }
                return null;
            }
        }

        public override void IncubateEgg(Thing egg)
        {
            base.IncubateEgg(egg);
        }

        public override void HatchEgg()
        {
            if (IsIncubatingEgg())
            {
                CompHatcher hatcher = Hatcher;
                if (hatcher != null)
                {
                    hatcher.Hatch();
                }
            }
        }

        public override Thing RetrieveEgg()
        {
            return Egg;
        }


        public override void GrowthTick()
        {
            CompHatcher hatcher = Hatcher;
            if (hatcher != null && !hatcher.TemperatureDamaged)
            {
                Progress += Props.GetProgressIncrease(this);
            }
        }
    }

    public class CompEggIncubator_Container : CompEggIncubator, IThingHolder
    {
        protected ThingOwner<Thing> incubatedEgg;

        public override Thing Egg => incubatedEgg.FirstOrFallback();

        private Gizmo gizmoRetrieveEgg;

        public CompEggIncubator_Container()
        {
            incubatedEgg = new ThingOwner<Thing>(this, true, LookMode.Deep);
        }

        public virtual void PlaceEggOnGround(Map map)
        {
            if (IsIncubatingEgg())
            {
                GenPlace.TryPlaceThing(RetrieveEgg(), parent.Position, map, ThingPlaceMode.Near);
            }
        }

        public override void IncubateEgg(Thing egg)
        {
            CompHatcher hatcher = egg.TryGetComp<CompHatcher>();
            if (hatcher != null)
            {
                if (IsIncubatingEgg())
                {
                    PlaceEggOnGround(parent.Map);
                }
                egg.DeSpawn();

                incubatedEgg.TryAddOrTransfer(egg);
                SetAvailable(false);
                base.IncubateEgg(egg);
            }
        }

        public override Thing RetrieveEgg()
        {
            if (IsIncubatingEgg())
            {
                Thing egg = Egg;

                incubatedEgg.Remove(egg);
                SetAvailable(true);

                return egg;
            }
            return null;
        }

        public override void HatchEgg()
        {
            if (IsIncubatingEgg())
            {
                Thing egg = Egg;

                GenSpawn.Spawn(egg, parent.Position, parent.Map);

                CompHatcher hatcher = egg.TryGetComp<CompHatcher>();
                if (hatcher != null)
                {
                    hatcher.Hatch();
                }

                incubatedEgg.Remove(egg);
                SetAvailable(true);
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            Thing egg = Egg;
            if (egg != null)
            {
                CompHatcher hatcher = egg.TryGetComp<CompHatcher>();
                if (hatcher != null)
                {
                    hatcher.CompTick();
                    if (egg.DestroyedOrNull())
                    {
                        incubatedEgg.Remove(egg);
                    }
                }
            }

            SetAvailable(egg == null);
        }

        public override void PostDraw()
        {
            base.PostDraw();

            Thing egg = Egg;
            if (egg != null && parent.Rotation == Rot4.North)
            {
                Egg.Graphic.Draw(GenThing.TrueCenter(parent.Position, parent.Rotation, parent.def.size, AltitudeLayer.Item.AltitudeFor()) + Props.eggDrawOffset, parent.Rotation, parent, 90f);
            }
        }

        public override void GrowthTick()
        {
            Thing egg = Egg;
            if (egg != null)
            {
                CompHatcher hatcher = egg.TryGetComp<CompHatcher>();
                if (hatcher != null)
                {
                    if (hatcher.TemperatureDamaged)
                    {
                        Messages.Message("DragonEggRuinedIncubatorMessage".Translate(egg.Named("EGG")), egg, MessageTypeDefOf.NegativeEvent);
                        PlaceEggOnGround(parent.Map);
                    }
                    else
                    {
                        Progress += Props.GetProgressIncrease(this);
                    }
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref incubatedEgg, "incubatedEgg", this);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (previousMap != null && IsIncubatingEgg())
            {
                PlaceEggOnGround(previousMap);
            }
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return incubatedEgg;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (IsIncubatingEgg())
            {
                if (gizmoRetrieveEgg == null)
                {
                    gizmoRetrieveEgg = Props.CreateGizmo(this);
                }

                yield return gizmoRetrieveEgg;
            }
        }

        private void SetAvailable(bool state)
        {
            Building_WorkTable building = parent as Building_WorkTable;
            if (building != null)
            {
                building.BillStack.Bills.ForEach(bill => bill.suspended = !state);
            }
        }
    }
}
