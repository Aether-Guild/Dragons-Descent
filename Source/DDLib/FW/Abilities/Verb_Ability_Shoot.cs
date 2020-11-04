using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class Verb_Shoot_Ability : Verb_Shoot, IAttackVerb
    {
        public Ability ability;

        public Ability Ability { get => ability; set => ability = value; }
        public Verb Verb => this;

        protected override bool TryCastShot()
        {
            if ((Ability?.CanCast ?? true) && base.TryCastShot())
            {
                if (burstShotsLeft <= 1)
                {
                    //Activate on last shot.
                    return ability.Activate(CurrentTarget, CurrentDestination);
                }
                return true;
            }
            return false;
        }

        public override bool Available()
        {
            return (Ability?.CanCast ?? true) && base.Available();
        }
    }

}
