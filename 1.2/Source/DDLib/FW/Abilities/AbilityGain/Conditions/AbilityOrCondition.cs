using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityOrCondition : AbilityCondition
    {
        public List<AbilityCondition> or;

        public override bool IsSatisfied(Pawn pawn)
        {
            if (!or.NullOrEmpty())
            {
                foreach (AbilityCondition cond in or)
                {
                    if (cond.IsSatisfied(pawn))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool IsFulfilled(Pawn pawn)
        {
            if (!or.NullOrEmpty())
            {
                foreach (AbilityCondition cond in or)
                {
                    if (cond.IsFulfilled(pawn))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ConditionString =>
            or.Count <= 1
                    ? or.First().ConditionString
                    : or.Select(cond => cond.ConditionString).Aggregate((a, b) => "ConditionOr".Translate(a.Named("CONDITION_A"), b.Named("CONDITION_B")));
    }
}
