using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DD
{
    public class JobGiver_MateCrossbreed : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.gender != Gender.Male || !pawn.ageTracker.CurLifeStage.reproductive)
            {
                return null;
            }

            Predicate<Thing> validator = thing => thing is Pawn p && !p.Downed && p.CanCasuallyInteractNow(false) && !p.IsForbidden(pawn) && p.Faction == pawn.Faction && MateUtils.FertileMateTarget(pawn, p);

            Pawn closest = null;
            foreach (ThingDef def in GetPossibleMates(pawn))
            {
                Pawn candidate = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(def), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 30f, validator, null, 0, -1, false, RegionType.Set_Passable, false) as Pawn;
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
            //Is part of a breeding pool
            if(pawn.def.HasModExtension<BreedingPoolExtension>())
            {
                //Can be part of multiple breeding pools (mod extensions seem to allow this...)
                IEnumerable<BreedingPoolExtension> exts = pawn.def.modExtensions.OfType<BreedingPoolExtension>();

                //Find all the pawns that are part of the same breeding pools.
                IEnumerable<ThingDef> mateDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.HasModExtension<BreedingPoolExtension>() && exts.Any(ext => ext.pool == def.GetModExtension<BreedingPoolExtension>().pool));
                
                if (mateDefs.Any())
                {
                    //Found some ThingDefs that are part of the same pool.
                    return mateDefs;
                }
            }

            //Pool wasn't defined, can only breed with its own type.
            return new ThingDef[] { pawn.def };
        }
    }
}
