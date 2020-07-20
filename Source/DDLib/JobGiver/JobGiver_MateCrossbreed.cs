using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DD
{
    public class ThingDefPool
    {
        public List<ThingDef> defs;


        public bool Contains(ThingDef def)
        {
            return defs.Contains(def);
        }
    }

    public class JobGiver_MateCrossbreed : ThinkNode_JobGiver
    {
        public List<ThingDefPool> matePools;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.gender != Gender.Male || !pawn.ageTracker.CurLifeStage.reproductive)
            {
                return null;
            }

            Predicate<Thing> validator = thing =>
            {
                Pawn p = thing as Pawn;
                return !p.Downed && p.CanCasuallyInteractNow(false) && (!p.IsForbidden(pawn) && p.Faction == pawn.Faction) && MateUtils.FertileMateTarget(pawn, p);
            };

            Pawn closest = null;
            foreach (ThingDef def in GetPossibleMates(pawn))
            {
                Pawn candidate = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(def), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 30f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
                if (candidate != null)
                { //Found candidate
                    if (closest == null || candidate.Position.DistanceTo(pawn.Position) < closest.Position.DistanceTo(pawn.Position))
                    { //No candidates selected yet OR candidate closer than previous closest
                        closest = candidate; //New candidate
                    }
                }
            }

            if (closest != null)
            {
                //Should match the def's name.
                return JobMaker.MakeJob(DD_JobDefOf.MateCrossbreed, closest);
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<ThingDef> GetPossibleMates(Pawn pawn)
        {
            //Filter all pools that don't include the pawn. Then collect all of the unique elements into a single pool.
            return matePools.Where(pool => pool.Contains(pawn.def)).Select(pool => (IEnumerable<ThingDef>)pool.defs).Aggregate((pool1, pool2) => pool1.Concat(pool2)).Distinct();
        }
    }
}
