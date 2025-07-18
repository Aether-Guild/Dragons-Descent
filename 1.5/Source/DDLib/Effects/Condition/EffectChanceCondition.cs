using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectChanceCondition : EffectCondition
    {
        public float chance;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing thing) => Rand.Chance(chance);
        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell) => Rand.Chance(chance);
    }
}
