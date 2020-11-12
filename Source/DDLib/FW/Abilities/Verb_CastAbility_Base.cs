using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    public class Verb_CastAbility_Base : Verb_CastAbility
    {
        public override bool Available()
        {
            return ability.CanCast && base.Available();
        }

        public override bool CanHitTarget(LocalTargetInfo targ)
        {
            if (ability.CanApplyOn(targ))
            {
                return base.CanHitTarget(targ);
            }
            else
            {
                return false;
            }
        }

        public override void Reset()
        {
            base.Reset();
            if (ability is Ability_Base ab)
            {
                ab.Reset();
            }
        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            Ability_Base ba = ability as Ability_Base;

            return base.CanHitTargetFrom(root, targ) && (ba == null || ba.CanApplyOn(targ));
        }

        protected override bool TryCastShot()
        {
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
