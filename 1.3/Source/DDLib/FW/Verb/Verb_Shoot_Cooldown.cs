using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using System.Linq;

namespace DD
{
    public class Verb_Shoot_Cooldown : Verb_Shoot
    {
        private int lastShotTick = 0;
        private int Cooldown => GenTicks.SecondsToTicks((verbProps as VerbProperties_Cooldown).cooldown);

        public bool Usable => (lastShotTick + Cooldown) < Find.TickManager.TicksGame;

        protected override bool TryCastShot()
        {
            if (Usable)
            {
                bool result = base.TryCastShot();
                if (burstShotsLeft <= 1)
                {
                    //Lock once it becomes unavailable.
                    lastShotTick = Find.TickManager.TicksGame;
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
            lastShotTick = 0;
        }

        public override bool Available()
        {
            return Usable && base.Available();
        }
    }
}
