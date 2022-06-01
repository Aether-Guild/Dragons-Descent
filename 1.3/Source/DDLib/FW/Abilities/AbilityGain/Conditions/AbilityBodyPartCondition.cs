using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityBodyPartCondition : AbilityCondition
    {
        public BodyPartDef bodyPart;
        public bool missing;
        public IntRange partCount = IntRange.one;

        private static IEnumerable<BodyPartRecord> CollectRecords(BodyPartRecord record, BodyPartDef part, List<BodyPartRecord> records = null)
        {
            if(records == null)
            {
                records = new List<BodyPartRecord>();
            }

            if (record.def == part)
            {
                records.Add(record);
            }

            if(!record.parts.NullOrEmpty())
            {
                record.parts.ForEach(rec => CollectRecords(rec, part, records));
            }
            return records;
        }

        //Get all missing parts if we're evaluating missing parts. Non-missing otherwise.
        private IEnumerable<BodyPartRecord> GetIntendedBodyParts(Pawn pawn) => CollectRecords(pawn.def.race.body.corePart, bodyPart).Where(rec => missing == pawn.health.hediffSet.PartIsMissing(rec));
        
        public override bool IsSatisfied(Pawn pawn)
        {
            return GetIntendedBodyParts(pawn).Count(part => part.def == bodyPart) >= partCount.TrueMin;
        }

        public override bool IsFulfilled(Pawn pawn)
        {
            return GetIntendedBodyParts(pawn).Count(part => part.def == bodyPart) >= partCount.TrueMax;
        }

        public override string ConditionString => (!missing ? "ConditionBodyPart" : "ConditionBodyPart_Missing").Translate(bodyPart.LabelCap.Named("BODY_PART"));
    }
}
