using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DD
{
    public class JobDriver_LayCrossbredEgg : JobDriver_LayEgg
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Skip the last toil (We're replacing it.)
            IEnumerable<Toil> toils = base.MakeNewToils().Reverse().Skip(1).Reverse();
            foreach (Toil toil in toils)
            {
                yield return toil;
            }
            yield return Toils_General.Do(() => { GenSpawn.Spawn(pawn.GetComp<CompEggLayer>().ProduceEgg(), pawn.Position, base.Map).SetForbiddenIfOutsideHomeArea(); });
        }
    }
}
