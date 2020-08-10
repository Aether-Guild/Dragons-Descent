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

    public class IngestionOutcomeDoer_DoIfBody : IngestionOutcomeDoer
    {
        public string targetDef;
        public IngestionOutcomeDoer doer;

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            return doer.SpecialDisplayStats(parentDef);
        }

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (pawn.RaceProps.body.defName.Equals(targetDef))
            {
                doer.DoIngestionOutcome(pawn, ingested);
            }
        }
    }

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
