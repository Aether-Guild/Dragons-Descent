using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DD
{
    public class CompTargetEffect_HealChronic : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead)
            {
                pawn.health.hediffSet.hediffs.Where(injury => injury.def != null && injury.def.chronic).ToList().ForEach((injury) => pawn.health.RemoveHediff(injury));
            }
        }
    }
}
