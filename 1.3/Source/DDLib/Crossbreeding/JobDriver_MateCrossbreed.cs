using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DD
{
    public class JobDriver_MateCrossbreed : JobDriver_Mate
    {
        protected Pawn Female => (Pawn)job.GetTarget(TargetIndex.A).Thing;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Skip the last toil (We're replacing it.)
            IEnumerable<Toil> toils = base.MakeNewToils().Reverse().Skip(1).Reverse();
            foreach (Toil toil in toils)
            {
                yield return toil;
            }
            yield return Toils_General.Do(() => { MateUtils.Mated(pawn, Female); });
        }
    }
}
