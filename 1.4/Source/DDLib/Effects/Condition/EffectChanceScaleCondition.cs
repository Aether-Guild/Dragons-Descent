using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectChanceScaleCondition : EffectCondition
    {
        public SimpleCurve chances;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing thing) => Rand.Chance(chances.Evaluate(effect.Size));
        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell) => Rand.Chance(chances.Evaluate(effect.Size));
    }
}
