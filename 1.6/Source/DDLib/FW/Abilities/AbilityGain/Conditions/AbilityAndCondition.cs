using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityAndCondition : AbilityCondition
    {
        public List<AbilityCondition> and;

        public override bool IsSatisfied(Pawn pawn)
        {
            if (!and.NullOrEmpty())
            {
                foreach (AbilityCondition cond in and)
                {
                    if (!cond.IsSatisfied(pawn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool IsFulfilled(Pawn pawn)
        {
            if (!and.NullOrEmpty())
            {
                foreach (AbilityCondition cond in and)
                {
                    if (!cond.IsFulfilled(pawn))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override string ConditionString =>
            and.Count <= 1
                    ? and.First().ConditionString
                    : and.Select(cond => cond.ConditionString).Aggregate((a, b) => "ConditionAnd".Translate(a.Named("CONDITION_A"), b.Named("CONDITION_B")));
    }
}
