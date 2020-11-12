using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityAgeCondition : AbilityCondition
    {
        public FloatRange ageRange;

        public override bool IsSatisfied(Pawn pawn)
        {
            return pawn.ageTracker.AgeBiologicalYears >= ageRange.TrueMin;
        }

        public override bool IsFulfilled(Pawn pawn)
        {
            return pawn.ageTracker.AgeBiologicalYears > ageRange.TrueMax;
        }

        public override string ConditionString => "ConditionAge".Translate(ageRange.TrueMin.Named("AGE"));
    }
}
