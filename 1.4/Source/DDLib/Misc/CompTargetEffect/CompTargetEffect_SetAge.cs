using RimWorld;
using Verse;

namespace DD
{
    public class CompTargetEffect_SetAge : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead)
            {
                CompProperties_Value vProps = props as CompProperties_Value;
                if (vProps != null)
                {
                    float? amount = vProps.GetAmount(pawn);
                    if (amount.HasValue)
                    {
                        AgeUtils.SetPawnAge(pawn, amount.Value);
                    }
                }
            }
        }
    }
}
