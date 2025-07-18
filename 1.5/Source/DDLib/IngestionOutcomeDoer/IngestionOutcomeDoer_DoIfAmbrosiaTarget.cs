using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DD
{
    public class IngestionOutcomeDoer_DoIfDrugTarget : IngestionOutcomeDoer_Conditional
    {
        protected override bool IsConditionSatisfied(Pawn pawn, Thing ingested)
        {
            return pawn.def.HasModExtension<DrugTargetExtension>();
        }
    }
}
