using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace DD
{
    public static class DD_FireUtility

    {
        // Token: 0x06007868 RID: 30824 RVA: 0x00296370 File Offset: 0x00294570
        public static bool CanEverAttachFire(this Thing t)
        {
            return !t.Destroyed && t.FlammableNow && t.def.category == ThingCategory.Pawn && t.TryGetComp<CompAttachBase>() != null;
        }

        // Token: 0x06007869 RID: 30825 RVA: 0x002963A4 File Offset: 0x002945A4
        public static float ChanceToStartFireIn(IntVec3 c, Map map)
        {
            List<Thing> thingList = c.GetThingList(map);
            float num = c.TerrainFlammableNow(map) ? c.GetTerrain(map).GetStatValueAbstract(StatDefOf.Flammability, null) : 0f;
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];
                if (thing is Fire)
                {
                    return 0f;
                }
                if (thing.def.category != ThingCategory.Pawn && thingList[i].FlammableNow)
                {
                    num = Mathf.Max(num, thing.GetStatValue(StatDefOf.Flammability, true, -1));
                }
            }
            if (num > 0f)
            {
                Building edifice = c.GetEdifice(map);
                if (edifice != null && edifice.def.passability == Traversability.Impassable && edifice.OccupiedRect().ContractedBy(1).Contains(c))
                {
                    return 0f;
                }
                List<Thing> thingList2 = c.GetThingList(map);
                for (int j = 0; j < thingList2.Count; j++)
                {
                    if (thingList2[j].def.category == ThingCategory.Filth && !thingList2[j].def.filth.allowsFire)
                    {
                        return 0f;
                    }
                }
            }
            return num;
        }

        // Token: 0x0600786A RID: 30826 RVA: 0x002964D3 File Offset: 0x002946D3
        public static bool TryStartFireIn(IntVec3 c, Map map, float fireSize)
        {
            if (DD_FireUtility.ChanceToStartFireIn(c, map) <= 0f)
            {
                return false;
            }
            Fire fire = (Fire)ThingMaker.MakeThing(DD_ThingDefOf.DraconicFlame, null);
            fire.fireSize = fireSize;
            GenSpawn.Spawn(fire, c, map, Rot4.North, WipeMode.Vanish, false);
            return true;
        }

        // Token: 0x0600786B RID: 30827 RVA: 0x0029650C File Offset: 0x0029470C
        public static float ChanceToAttachFireFromEvent(Thing t)
        {
            return DD_FireUtility.ChanceToAttachFireCumulative(t, 60f);
        }

        // Token: 0x0600786C RID: 30828 RVA: 0x0029651C File Offset: 0x0029471C
        public static float ChanceToAttachFireCumulative(Thing t, float freqInTicks)
        {
            if (!t.CanEverAttachFire())
            {
                return 0f;
            }
            if (t.HasAttachment(DD_ThingDefOf.DraconicFlame))
            {
                return 0f;
            }
            float num = DD_FireUtility.ChanceToCatchFirePerSecondForPawnFromFlammability.Evaluate(t.GetStatValue(StatDefOf.Flammability, true, -1));
            return 1f - Mathf.Pow(1f - num, freqInTicks / 60f);
        }

        // Token: 0x0600786D RID: 30829 RVA: 0x0029657C File Offset: 0x0029477C
        public static void TryAttachFire(this Thing t, float fireSize)
        {
            if (!t.CanEverAttachFire())
            {
                return;
            }
            if (t.HasAttachment(DD_ThingDefOf.DraconicFlame))
            {
                return;
            }
            Fire fire = (Fire)ThingMaker.MakeThing(DD_ThingDefOf.DraconicFlame, null);
            fire.fireSize = fireSize;
            fire.AttachTo(t);
            GenSpawn.Spawn(fire, t.Position, t.Map, Rot4.North, WipeMode.Vanish, false);
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                pawn.jobs.StopAll(false, true);
                pawn.records.Increment(RecordDefOf.TimesOnFire);
            }
        }

        // Token: 0x0600786E RID: 30830 RVA: 0x002965FE File Offset: 0x002947FE
        public static bool IsBurning(this TargetInfo t)
        {
            if (t.HasThing)
            {
                return t.Thing.IsBurning();
            }
            return t.Cell.ContainsStaticFire(t.Map);
        }

        // Token: 0x0600786F RID: 30831 RVA: 0x0029662C File Offset: 0x0029482C
        public static bool IsBurning(this Thing t)
        {
            if (t.Destroyed || !t.Spawned)
            {
                return false;
            }
            if (!(t.def.size == IntVec2.One))
            {
                using (CellRect.Enumerator enumerator = t.OccupiedRect().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.ContainsStaticFire(t.Map))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            if (t is Pawn)
            {
                return t.HasAttachment(DD_ThingDefOf.DraconicFlame);
            }
            return t.Position.ContainsStaticFire(t.Map);
        }

        // Token: 0x06007870 RID: 30832 RVA: 0x002966E0 File Offset: 0x002948E0
        public static bool ContainsStaticFire(this IntVec3 c, Map map)
        {
            List<Thing> list = map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                Fire fire = list[i] as Fire;
                if (fire != null && fire.parent == null)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06007871 RID: 30833 RVA: 0x00296728 File Offset: 0x00294928
        public static int NumFiresAt(IntVec3 c, Map map)
        {
            List<Thing> list = map.thingGrid.ThingsListAt(c);
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsBurning())
                {
                    num++;
                }
                if (list[i].def.CompDefForAssignableFrom<CompFireOverlayBase>() != null)
                {
                    CompGlower compGlower = list[i].TryGetComp<CompGlower>();
                    if (compGlower != null && compGlower.Glows)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        // Token: 0x06007872 RID: 30834 RVA: 0x0029679C File Offset: 0x0029499C
        public static bool ContainsTrap(this IntVec3 c, Map map)
        {
            Building edifice = c.GetEdifice(map);
            return edifice != null && edifice is Building_Trap;
        }

        // Token: 0x06007873 RID: 30835 RVA: 0x002967BF File Offset: 0x002949BF
        public static bool Flammable(this TerrainDef terrain)
        {
            return terrain.GetStatValueAbstract(StatDefOf.Flammability, null) > 0.01f;
        }

        // Token: 0x06007874 RID: 30836 RVA: 0x002967D4 File Offset: 0x002949D4
        public static bool TerrainFlammableNow(this IntVec3 c, Map map)
        {
            if (!c.GetTerrain(map).Flammable())
            {
                return false;
            }
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i].FireBulwark)
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x040044A2 RID: 17570
        private static readonly SimpleCurve ChanceToCatchFirePerSecondForPawnFromFlammability = new SimpleCurve
        {
            {
                new CurvePoint(0f, 0f),
                true
            },
            {
                new CurvePoint(0.1f, 0.07f),
                true
            },
            {
                new CurvePoint(0.3f, 1f),
                true
            },
            {
                new CurvePoint(1f, 1f),
                true
            }
        };
    }
}