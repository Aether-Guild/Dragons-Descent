using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DD
{
    public class IngestionOutcomeDoer_DoIfAmbrosiaTarget : IngestionOutcomeDoer
    {
        public IngestionOutcomeDoer doer;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (pawn.def.HasModExtension<AmbrosiaTargetExtension>())
            {
                doer.DoIngestionOutcome(pawn, ingested);
            }
        }
    }
}
