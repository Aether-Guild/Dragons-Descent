using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    public class AbilityComp_Cooldown : AbilityComp_Base
    {
        public AbilityCompProperties_Cooldown VProps => props as AbilityCompProperties_Cooldown;

        public void TryStartCooldown()
        {
            //Not self, can cooldown, has a cooldown comp
           // Log.Message("Cooldown on " + this.parent + " - " + this.parent.def);
            if (VProps.cooldownTicksRange == default(IntRange))
            {
                //If no cooldown set, use the ability's own cooldown value.
                this.parent.StartCooldown(this.parent.def.cooldownTicksRange.RandomInRange);
            }
            else
            {
                //If cooldown is set, use it instead.
                this.parent.StartCooldown(VProps.cooldownTicksRange.RandomInRange);
            }
            this.parent.pawn.ClearMind();
        }
    }
}
