using RimWorld.Planet;
using Verse;
using VFECore.Abilities;

namespace DD;

public class CannotUseWhileFormingCaravanExtension : AbilityExtension_AbilityMod
{
    public string disableMessageKey = "Ability_CannotUseWhileFormingCaravan";

    public override bool IsEnabledForPawn(Ability ability, out string reason)
    {
        if (ability.CasterPawn == null || !ability.CasterPawn.IsFormingCaravan())
            return base.IsEnabledForPawn(ability, out reason);

        reason = disableMessageKey.Translate(ability.CasterPawn.LabelCap.Named("PAWN"));
        return false;
    }
}