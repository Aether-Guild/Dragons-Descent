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
        public int penaltyThreshold = 1;
        public MentalStateDef penaltyMentalDef;
        public string penaltyReason;

        public FerociousPawnTracker GetTracker(Pawn pawn)
        {
            return pawn.Map.GetComponent<MapComponent_Tracker>().Ferocious;
        }

        protected bool ShouldBePenalized(Pawn pawn) => GetTracker(pawn) != null && GetTracker(pawn).Pawns.Count() > penaltyThreshold;

        public override bool OnHediffAdded(Pawn pawn, Hediff hediff)
        {
            if (FerocityUtils.IsHediff(hediff))
            {
                GetTracker(pawn).Track(pawn);
            }

            return base.OnHediffAdded(pawn, hediff);
        }

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if(pawn == null || !pawn.Spawned || pawn.Destroyed || pawn.Map == null)
            {
                return;
            }

            if (ShouldBePenalized(pawn))
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(penaltyMentalDef, reason: penaltyReason, forceWake: true, causedByMood: false);
            }

            //If pawn is a ferocity source
            if (FerocityUtils.HasHediff(pawn))
            {
                FerociousPawnTracker tracker = GetTracker(pawn);

                //Track it if its not already being tracked.
                if (!tracker.IsTracked(pawn))
                {
                    tracker.Track(pawn);
                }
            }
            else
            {
                //A pawn on map is ferocious and this pawn isn't pack ferocious.
                if (FerocityUtils.InMapWithFerocious(pawn))
                {
                    if(!FerocityUtils.HasPackHediff(pawn))
                    {
                        FerocityUtils.AddPackHediff(pawn);
                    }
                }
                else
                {
                    if (FerocityUtils.HasPackHediff(pawn))
                    {
                        //No pawns are ferocious anymore.
                        FerocityUtils.RemovePackHediff(pawn);
                    }
                }
            }
        }
    }
}
