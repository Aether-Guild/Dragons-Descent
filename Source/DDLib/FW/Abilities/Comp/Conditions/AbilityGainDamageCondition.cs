using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityGainDamageCondition : AbilityGainCondition
    {
        public FloatRange damageRange;

        public override bool IsSatisfied(CompAbilityDefinition def)
        {
            return def.DamageTotal >= damageRange.TrueMin;
        }

        public override bool IsFulfilled(CompAbilityDefinition def)
        {
            return def.DamageTotal > damageRange.TrueMax;
        }
    }
}
