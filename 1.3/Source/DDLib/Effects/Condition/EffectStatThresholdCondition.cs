using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectStatThresholdCondition : EffectCondition
    {
        public StatDef statDef;
        public FloatRange range;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing target)
        {
            return range.Includes(target.GetStatValue(statDef));
        }

        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell)
        {
            return range.Includes(cell.GetTerrain(map).GetStatValueAbstract(statDef));
        }
    }
}
