using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class IngestionOutcomeDoer_Conditional : IngestionOutcomeDoer
    {
        public IngestionOutcomeDoer ifTrue, ifFalse;

        protected abstract bool IsConditionSatisfied(Pawn pawn, Thing ingested);

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (IsConditionSatisfied(pawn, ingested))
            {
                ifTrue?.DoIngestionOutcome(pawn, ingested);
            }
            else
            {
                ifFalse?.DoIngestionOutcome(pawn, ingested);
            }
        }
    }
}
