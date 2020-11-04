using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD
{
    public class Verb_CastAbility_Base : Verb_CastAbility
    {
        public override bool Available()
        {
            return ability.CanCast && base.Available();
        }

        public override void Reset()
        {
            base.Reset();
            if (ability is Ability_Base ab)
            {
                ab.Reset();
            }
        }

        protected override bool TryCastShot()
        {
            if (ability is Ability_Base ba)
            {
                if (!ba.CanApplyOn(CurrentTarget) && !ba.CanApplyOn(CurrentDestination))
                {
                    //Should be applicable on either the target or the destination.
                    return false;
                }
            }

            if (ability.CanCast)
            {
                return base.TryCastShot();
            }
            else
            {
                return false;
            }
        }
    }
}
