using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityRecordCondition : AbilityCondition
    {
        public RecordDef recordDef;
        public FloatRange value;

        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.records.GetValue(recordDef) >= value.TrueMin;
        }

        public override bool IsFulfilled(Pawn pawn)
        {
            return pawn.records.GetValue(recordDef) >= value.TrueMax;
        }

        public override string ConditionString => "ConditionRecord".Translate(value.TrueMin.Named("VALUE"), recordDef.LabelCap.Named("RECORD"));
    }
}
