using RimWorld.Planet;
using Verse;
using VFECore.Abilities;

namespace DD;

public class DisabledAbilityExtension : AbilityExtension_AbilityMod
{
    public override bool ShowGizmoOnPawn(Pawn pawn) => false;
    public override bool IsEnabledForPawn(Ability ability, out string reason)
    {
        reason = "Hidden/disabled ability - you should not be able to see this.";
        return false;
    }
    // Those are likely not needed, but may as well be 100% sure the AI can't use it.
    public override bool Valid(GlobalTargetInfo[] targets, Ability ability, bool throwMessages = false) => false;
    public override bool ValidateTarget(LocalTargetInfo target, Ability ability, bool throwMessages = false) => false;
    public override bool ValidTile(GlobalTargetInfo target, Ability ability, bool throwMessages = false) => false;
    public override bool CanApplyOn(LocalTargetInfo target, Ability ability, bool throwMessages = false) => false;
}