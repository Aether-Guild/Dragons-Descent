using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class HediffGiver_Ferocity : HediffGiver
    {
        private List<Verse.WeakReference<Pawn>> tracker = new List<Verse.WeakReference<Pawn>>();

        public int penaltyThreshold = 1;
        public MentalStateDef penaltyMentalDef;
        public string penaltyReason;

        //Pawns with ferocity and are alive.
        public IEnumerable<Pawn> FerociousPawns => tracker.Where(r => r.IsAlive).Select(r => r.Target).Where(pawn => pawn != null && pawn.Spawned && !pawn.Dead && HasFerocityHediff(pawn));

        protected bool IsTracked(Pawn pawn) => FerociousPawns.Contains(pawn);

        protected void Track(Pawn pawn)
        {
            //Has ferocious hediff.
            if (HasFerocityHediff(pawn))
            {
                //Not being tracked.
                if (!IsTracked(pawn))
                {
                    tracker.Add(new Verse.WeakReference<Pawn>(pawn)); //Add it.
                }
            }
        }

        public void Clear()
        {
            foreach(Pawn pawn in FerociousPawns)
            {
                RemoveHediff(pawn, DD_HediffDefOf.DraconicFerocity_Male);
                RemoveHediff(pawn, DD_HediffDefOf.DraconicFerocity_Female);
            }
            tracker.Clear();
        }

        protected void Refresh()
        {
            //Remove all destroyed pawns or nulls, or pawns without ferocity hediff.
            tracker.RemoveAll(r => !r.IsAlive || !(r.Target is Pawn) || r.Target.DestroyedOrNull() || !r.Target.Spawned || r.Target.Map == null || !HasFerocityHediff(r.Target));
        }

        protected bool InMapWithFerocity(Pawn pawn)
        {
            return FerociousPawns.Any(p => p != pawn && p.Map == pawn.Map);
        }

        protected bool ShouldBePenalized(Pawn pawn)
        {
            return FerociousPawns.Where(p => p.Map == p.Map).Count() > penaltyThreshold;
        }

        protected bool HasFerocityHediff(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicFerocity_Male) || pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicFerocity_Female);
        }

        protected bool HasPackFerocityHediff(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicPackFerocity_Male) || pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicPackFerocity_Female);
        }

        private void RemoveHediff(Pawn pawn, HediffDef def)
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

        public override bool OnHediffAdded(Pawn pawn, Hediff hediff)
        {
            if (hediff.def == DD_HediffDefOf.DraconicFerocity_Male || hediff.def == DD_HediffDefOf.DraconicFerocity_Female)
            {
                Track(pawn);
            }

            return base.OnHediffAdded(pawn, hediff);
        }

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (ShouldBePenalized(pawn))
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(penaltyMentalDef, penaltyReason, true, false);
            }

            //If pawn is a ferocity source
            if (HasFerocityHediff(pawn))
            {
                //Remove its pack ferocity if it has it.
                if (HasPackFerocityHediff(pawn))
                {
                    RemoveHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Male);
                    RemoveHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Female);
                }

                //Track it if its not already being tracked.
                if (!IsTracked(pawn))
                {
                    Track(pawn);
                }
            }
            else
            {
                //A pawn on map is ferocious and this pawn isn't pack ferocious.
                if (InMapWithFerocity(pawn))
                {
                    if(!HasPackFerocityHediff(pawn))
                    {
                        HediffDef selectedHediff = null;

                        //Pick based on gender.
                        switch (pawn.gender)
                        {
                            case Gender.Male:
                                selectedHediff = DD_HediffDefOf.DraconicPackFerocity_Male;
                                break;
                            case Gender.Female:
                                selectedHediff = DD_HediffDefOf.DraconicPackFerocity_Female;
                                break;
                        }

                        //Load the hediff from the hediffgiver, if one hasn't been selected yet.
                        if (selectedHediff == null)
                        {
                            selectedHediff = hediff;
                        }

                        if (selectedHediff != null)
                        {
                            HediffGiverUtility.TryApply(pawn, selectedHediff, null);
                        }
                    }
                }
                else
                {
                    //No pawns are ferocious anymore.
                    RemoveHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Male);
                    RemoveHediff(pawn, DD_HediffDefOf.DraconicPackFerocity_Female);
                }
            }
        }
    }
}
