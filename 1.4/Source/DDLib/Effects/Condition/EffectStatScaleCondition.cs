using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectStatScaleCondition : EffectCondition
    {
        public StatDef statDef;
        public SimpleCurve thresholds;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing target)
        {
            return target.GetStatValue(statDef) > thresholds.Evaluate(effect.Size);
        }

        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell)
        {
            return cell.GetTerrain(map).GetStatValueAbstract(statDef) > thresholds.Evaluate(effect.Size);
        }
    }
}
