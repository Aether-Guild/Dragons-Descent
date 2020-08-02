using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public static class FerocityUtils
    {
        public static bool HasAnyHediff(Pawn pawn) => HasHediff(pawn) || HasPackHediff(pawn);
        public static bool HasHediff(Pawn pawn) => pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicFerocity_Male) || pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicFerocity_Female);
        public static bool HasPackHediff(Pawn pawn) => pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicPackFerocity_Male) || pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicPackFerocity_Female);

        public static bool IsHediff(Hediff hediff) => hediff.def == DD_HediffDefOf.DraconicFerocity_Male || hediff.def == DD_HediffDefOf.DraconicFerocity_Female;
        public static bool IsPackHediff(Hediff hediff) => hediff.def == DD_HediffDefOf.DraconicPackFerocity_Male || hediff.def == DD_HediffDefOf.DraconicPackFerocity_Female;

        public static bool InMapWithFerocious(Pawn pawn) => pawn != null && pawn.Map != null && pawn.Map.GetComponent<MapComponent_Tracker>().Ferocious.Pawns.Any(p => p != pawn && p.Map == pawn.Map);

        public static void AddHediff(Pawn pawn)
        {
            switch (pawn.gender)
            {
                case Gender.Male:
                    AddHediff(pawn, DD_HediffDefOf.DraconicFerocity_Male);
                    break;
                case Gender.Female:
                    AddHediff(pawn, DD_HediffDefOf.DraconicFerocity_Female);
                    break;
            }
        }
        public static void AddPackHediff(Pawn pawn)
        {
            switch (pawn.gender)
            {
                case Gender.Male:
                    AddHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Male);
                    break;
                case Gender.Female:
                    AddHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Female);
                    break;
            }
        }

        public static void RemoveHediff(Pawn pawn)
        {
            RemoveHediff(pawn, DD_HediffDefOf.DraconicFerocity_Male);
            RemoveHediff(pawn, DD_HediffDefOf.DraconicFerocity_Female);
        }
        public static void RemovePackHediff(Pawn pawn)
        {
            RemoveHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Male);
            RemoveHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Female);
        }

        private static void AddHediff(Pawn pawn, HediffDef def)
        {
            if (def != null)
            {
                HediffGiverUtility.TryApply(pawn, def, null);
            }
        }
        private static void RemoveHediff(Pawn pawn, HediffDef def)
        {
            Hediff hediff;
            do
            {
                hediff = pawn.health.hediffSet.GetFirstHediffOfDef(def);
                if (hediff != null)
                {
                    pawn.health.RemoveHediff(hediff);
                }
            }
            while (hediff != null);
        }
    }

    public class FerociousPawnTracker : TrackerComponent
    {
        private Map map;
        private List<Pawn> tracker;

        //Pawns with ferocity and are alive.
        public IEnumerable<Pawn> Pawns => tracker.Where(pawn => pawn != null && pawn.Spawned && !pawn.Dead && FerocityUtils.HasHediff(pawn));

        public FerociousPawnTracker(Map map)
        {
            this.map = map;
            this.tracker = new List<Pawn>();
        }

        public bool IsTracked(Pawn pawn) => Pawns.Contains(pawn);

        public void Track(Pawn pawn)
        {
            //Remove its pack ferocity if it has it.
            if (FerocityUtils.HasPackHediff(pawn))
            {
                FerocityUtils.RemovePackHediff(pawn);
            }

            //Has ferocious hediff.
            if (FerocityUtils.HasHediff(pawn))
            {
                //Not being tracked.
                if (!IsTracked(pawn))
                {
                    tracker.Add(pawn); //Add it.
                }
            }
        }

        public override void Tick()
        {
            tracker.RemoveAll(pawn => pawn.DestroyedOrNull() || !pawn.Spawned || pawn.Map == null || !FerocityUtils.HasHediff(pawn));
            foreach (Pawn pawn in tracker.Where(pawn => FerocityUtils.HasPackHediff(pawn)))
            {
                FerocityUtils.RemovePackHediff(pawn);
            }
        }

        public void Clear()
        {
            foreach (Pawn pawn in Pawns)
            {
                FerocityUtils.RemoveHediff(pawn);
            }
            tracker.Clear();
        }

        //Remove all destroyed pawns or nulls, or pawns without ferocity hediff.
        public void Refresh() => tracker.RemoveAll(r => r.DestroyedOrNull() || !r.Spawned || r.Map == null || !FerocityUtils.HasHediff(r));

        public override void ExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_Collections.Look(ref tracker, "ferocious", LookMode.Reference);
            if (tracker == null)
            {
                tracker = new List<Pawn>();
            }
        }
    }
}
