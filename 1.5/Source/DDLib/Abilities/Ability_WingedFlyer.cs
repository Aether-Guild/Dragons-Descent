using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using Ability = VFECore.Abilities.Ability;

namespace DD;

public class Ability_WingedFlyer : Ability
{
    public WingedFlyerAbilityExtension Extension => def.GetModExtension<WingedFlyerAbilityExtension>();
    
    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);

        // Mostly meaningless as the ability def should enforce proper targets here,
        // so the check for any target non-global is here for extra safety.
        if (targets.Length > 0 && !targets[0].IsWorldTarget)
        {
            // Inside DD the flyer can depend on the DD defs.
            // If the flyer would be extracted into a standalone .dll file, it can't depend on it
            // and needs to use a separate def for the flyer.
            WingedFlyer.MakeFlyer(CasterPawn, (TargetInfo)targets[0], Extension?.flyerDef ?? DD_ThingDefOf.WingedFlyer);
        }
    }

    public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
    {
        var extension = Extension;

        // If null or ignore roof is false
        if (extension is not { ignoreRoofCheck: true })
        {
            // Check if starting under the roof
            if (!WingedFlyer.IsCellValid(CasterPawn.Position, Caster.Map, true))
            {
                // Don't show messages/play sounds unless showMessages is true to prevent sounds without user input
                if (showMessages)
                {
                    // Translate the text (if possible) and display a message, else just play a rejection sound
                    if (!(extension?.underRoofStartKey).NullOrEmpty())
                        Messages.Message(extension!.underRoofStartKey.Translate(CasterPawn.Named("PAWN")), CasterPawn, MessageTypeDefOf.RejectInput, false);
                    else
                        SoundDefOf.ClickReject.PlayOneShotOnCamera();
                }
                return false;
            }

            // Check if ending under the roof
            if (!WingedFlyer.IsCellValid(target.Cell, Caster.Map, true))
            {
                // Don't show messages/play sounds unless showMessages is true to prevent sounds without user input
                if (showMessages)
                {
                    // Translate the text (if possible) and display a message, else just play a rejection sound
                    if (!(extension?.underRoofEndKey).NullOrEmpty())
                        Messages.Message(extension!.underRoofEndKey.Translate(CasterPawn.Named("PAWN")), CasterPawn, MessageTypeDefOf.RejectInput, false);
                    else
                        SoundDefOf.ClickReject.PlayOneShotOnCamera();
                }
                return false;
            }
        }

        return base.ValidateTarget(target, showMessages);
    }
}