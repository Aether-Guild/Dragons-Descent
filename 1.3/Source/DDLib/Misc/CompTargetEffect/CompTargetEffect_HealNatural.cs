using RimWorld;
using System.Linq;
using Verse;

namespace DD
{
    public class CompTargetEffect_HealNatural : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead)
            {
                pawn.health.hediffSet.GetHediffs<Hediff_Injury>().Where(injury => injury.CanHealNaturally()).ToList().ForEach((injury) => pawn.health.RemoveHediff(injury));
            }
        }
    }
}
