using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using System.Linq;

namespace DD
{
    public class Verb_Shoot_Limited : Verb_Shoot
    {
        public bool Usable { get; private set; }

        protected override bool TryCastShot()
        {
            if (Usable)
            {
                bool result = base.TryCastShot();
                if (burstShotsLeft <= 1)
                {
                    //Lock once it becomes unavailable.
                    Usable = false;
                }
                return true;
            }
            return false; //Disabled.
        }

        public override void Reset()
        {
            Refresh();
            base.Reset();
        }

        public void Refresh()
        {
            //Unlocks the verb.
            Usable = true;
        }

        public override bool Available()
        {
            return Usable && base.Available();
        }
    }
}
