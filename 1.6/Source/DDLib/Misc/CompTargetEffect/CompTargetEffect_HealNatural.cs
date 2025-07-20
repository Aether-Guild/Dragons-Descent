using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DD
{
    public class CompTargetEffect_HealNatural : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead) {
                List<Hediff_Injury> tmpInjuries = new List<Hediff_Injury>();
                pawn.health.hediffSet.GetHediffs(ref tmpInjuries, (Hediff_Injury x) => x != null && x.CanHealNaturally());
                if (tmpInjuries.Count > 0)
                    tmpInjuries.ForEach((injury) => pawn.health.RemoveHediff(injury));
            }
        }
    }
}
