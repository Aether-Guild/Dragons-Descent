using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class WorkGiver_Scale : WorkGiver_GatherAnimalBodyResources
    {
        protected override JobDef JobDef => DD_JobDefOf.Scale;

        protected override CompHasGatherableBodyResource GetComp(Pawn animal)
        {
            return animal.TryGetComp<CompScaleable>();
        }
    }
}
