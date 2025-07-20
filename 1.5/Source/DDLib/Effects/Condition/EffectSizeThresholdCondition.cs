using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectSizeThresholdCondition : EffectCondition
    {
        public FloatRange range;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing target)
        {
            return range.Includes(effect.Size);
        }

        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell)
        {
            return range.Includes(effect.Size);
        }
    }
}
