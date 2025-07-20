using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DD
{
    public class JobGiver_LayCrossbredEgg : ThinkNode_JobGiver
    {
        private const float LayRadius = 5f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            CompCrossbredEggLayer layer = pawn.TryGetComp<CompCrossbredEggLayer>();
            if (layer == null || !layer.CanLayNow)
            {
                return null;
            }

            IntVec3 layPosition;


            Building_Bed bed = RestUtility.FindBedFor(pawn);
            if (bed != null)
            {
                layPosition = bed.Position;
            }
            else
            {
                layPosition = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, LayRadius, null, Danger.Some);
            }

            if (layPosition.GetFirstItem(pawn.Map) != null)
            {
                //Position occupied.
                if (RCellFinder.TryFindRandomCellNearWith(layPosition, cell => cell.GetFirstItem(pawn.Map) == null, pawn.Map, out IntVec3 result))
                {
                    //Find first unoccupied place near the selected position.
                    layPosition = result;
                }
                else
                {
                    return null;
                }
            }

            return JobMaker.MakeJob(DD_JobDefOf.LayCrossbredEgg, layPosition);
        }
    }
}
