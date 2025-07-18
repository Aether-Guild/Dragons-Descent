using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DD
{
    public class CompTargetEffect_HealHediffs : CompTargetEffect
    {
        public CompProperties_HealHediffs HProps => (CompProperties_HealHediffs)props;

        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead)
            {
                pawn.health.hediffSet.hediffs.Where(hediff => HProps.defs != null && HProps.defs.Contains(hediff.def)).ToList().ForEach(hediff => pawn.health.RemoveHediff(hediff));
            }
        }
    }
}
