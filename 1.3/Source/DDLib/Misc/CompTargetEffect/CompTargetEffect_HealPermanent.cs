using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DD
{
    public class CompTargetEffect_HealPermanent : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead)
            {
                pawn.health.hediffSet.GetHediffs<Hediff_Injury>().Where(injury => injury.IsPermanent()).ToList().ForEach((injury) => pawn.health.RemoveHediff(injury));
            }
        }
    }
}
