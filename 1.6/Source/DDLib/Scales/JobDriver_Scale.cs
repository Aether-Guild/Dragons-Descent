using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class JobDriver_Scale : JobDriver_GatherAnimalBodyResources
    {
        protected override float WorkTotal => 400f;

        protected override CompHasGatherableBodyResource GetComp(Pawn animal)
        {
            return animal.TryGetComp<CompScaleable>();
        }
    }
}
