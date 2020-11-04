using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityGainKillCondition : AbilityGainCondition
    {
        public IntRange killCount;

        public override bool IsSatisfied(CompAbilityDefinition def)
        {
            return def.KillCounter >= killCount.TrueMin;
        }

        public override bool IsFulfilled(CompAbilityDefinition def)
        {
            return def.KillCounter > killCount.TrueMax;
        }
    }
}
