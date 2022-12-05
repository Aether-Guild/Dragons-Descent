using HarmonyLib;
using Verse;

namespace DD
{
    public class AbilityAgeCondition : AbilityCondition
    {
        public FloatRange ageRange;

        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.ageTracker.AgeBiologicalYears >= ageRange.TrueMin;
        }

        public override bool IsFulfilled(Pawn pawn)
        {
            return pawn.ageTracker.AgeBiologicalYears > ageRange.TrueMax;
        }

        public override string ConditionString => "ConditionAge".Translate(ageRange.TrueMin.Named("AGE"));
    }

    // The original method overflows because it floors the AdultMinAge. This patch fixes that by never casting to int.
    [HarmonyPatch(typeof(Pawn_AgeTracker), "get_AdultMinAgeTicks")]
    public static class DD_PawnAgeTracker_AdultMinAgeTicks
    {
        public static bool Prefix(Pawn_AgeTracker __instance, ref long __result)
        {
            __result = (long)(__instance.AdultMinAge * 3600000f);

            return false;
        }
    }
}