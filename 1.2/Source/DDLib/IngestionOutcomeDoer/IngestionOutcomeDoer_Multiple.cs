using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DD
{
    public class IngestionOutcomeDoer_Multiple : IngestionOutcomeDoer
    {
        public List<IngestionOutcomeDoer> doers;

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            foreach(IngestionOutcomeDoer doer in doers)
            {
                foreach(StatDrawEntry entry in doer.SpecialDisplayStats(parentDef))
                {
                    yield return entry;
                }   
            }
        }

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            foreach(IngestionOutcomeDoer doer in doers)
            {
                doer.DoIngestionOutcome(pawn, ingested);
            }
        }
    }
}
