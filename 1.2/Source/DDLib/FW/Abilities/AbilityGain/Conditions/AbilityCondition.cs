using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class AbilityCondition
    {
        public abstract bool IsSatisfied(Pawn pawn);

        public abstract bool IsFulfilled(Pawn pawn);

        public abstract string ConditionString { get; }
    }
}
