using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    public class Verb_AoE : Verb
    {
        protected override bool TryCastShot()
        {
            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(Caster.Position, Caster.Map, verbProps.range, true).Where(t => t != Caster && Caster.CanSee(t)))
            {
                thing.TakeDamage(new DamageInfo(verbProps.meleeDamageDef, verbProps.meleeDamageBaseAmount, instigator: Caster));
            }
            return true;
        }
    }
}
