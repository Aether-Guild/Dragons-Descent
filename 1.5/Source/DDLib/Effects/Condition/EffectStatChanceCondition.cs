using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectStatChanceCondition : EffectCondition
    {
        public StatDef statDef;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing target)
        {
            return Rand.Chance(target.GetStatValue(statDef));
        }

        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell)
        {
            return Rand.Chance(cell.GetTerrain(map).GetStatValueAbstract(statDef));
        }
    }
}
