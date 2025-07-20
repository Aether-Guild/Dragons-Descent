using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DD
{
    public class IngestionOutcomeDoer_DoIfBody : IngestionOutcomeDoer
    {
        public string targetDef;
        public IngestionOutcomeDoer doer;

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            return doer.SpecialDisplayStats(parentDef);
        }

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn.RaceProps.body.defName.Equals(targetDef))
            {
                doer.DoIngestionOutcome(pawn, ingested, ingestedCount);
            }
        }
    }
}
