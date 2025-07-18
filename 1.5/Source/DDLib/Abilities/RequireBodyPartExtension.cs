using System.Collections.Generic;
using System.Linq;
using Verse;
using VFECore.Abilities;

namespace DD;

public class RequireBodyPartExtension : AbilityExtension_AbilityMod
{
    public BodyPartDef bodyPart;
    public bool missing;
    public IntRange partCount = IntRange.one;
    public string disableMessageKey = "Ability_RequiresBodyPart";

    //Get all missing parts if we're evaluating missing parts. Non-missing otherwise.
    private IEnumerable<BodyPartRecord> GetIntendedBodyParts(Pawn pawn) => AbilityComp_RequireBodyPart.CollectRecords(pawn.def.race.body.corePart, bodyPart).Where(rec => missing == pawn.health.hediffSet.PartIsMissing(rec));
    private int CountBodyParts(Pawn pawn) => GetIntendedBodyParts(pawn).Count(part => part.def == bodyPart);
    public bool ConditionSatisfied(Pawn pawn) => CountBodyParts(pawn) < partCount.TrueMax;

    public override bool IsEnabledForPawn(Ability ability, out string reason)
    {
        if (ability.CasterPawn == null || ConditionSatisfied(ability.CasterPawn))
            return base.IsEnabledForPawn(ability, out reason);

        reason = disableMessageKey.Translate(ability.CasterPawn.LabelCap.Named("PAWN"), CountBodyParts(ability.CasterPawn).Named("COUNT"), bodyPart.LabelCap.Named("BODYPART"));
        return false;
    }
}