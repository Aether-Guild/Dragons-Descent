using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectAndCondition : EffectCondition
    {
        public List<EffectCondition> conditions;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing thing)
        {
            foreach(EffectCondition condition in conditions)
            {
                if(!condition.ConditionIsSatisfied(effect, thing))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell)
        {
            foreach (EffectCondition condition in conditions)
            {
                if (!condition.ConditionIsSatisfied(effect, map, cell))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
