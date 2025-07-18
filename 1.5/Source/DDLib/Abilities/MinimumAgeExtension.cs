using Verse;
using VFECore.Abilities;

namespace DD;

public class MinimumAgeExtension : AbilityExtension_AbilityMod
{
    public int minAge = 0;
    public string underMinAgeKey = "Ability_UnderMinimumAge";

    public override bool IsEnabledForPawn(Ability ability, out string reason)
    {
        if (ability.CasterPawn.ageTracker.AgeBiologicalYears < minAge)
        {
            reason = underMinAgeKey.Translate(ability.CasterPawn.Named("PAWN"), minAge.Named("AGE"));
            return false;
        }

        return base.IsEnabledForPawn(ability, out reason);
    }
}