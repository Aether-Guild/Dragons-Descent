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

        public string CooldownPool => VProps.cooldownPool;
        public virtual bool IsIndependent => VProps.independent;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            //Not self, can cooldown, has a cooldown comp
            IEnumerable<Ability> abilities = parent.pawn.abilities.abilities.Where(ability => ability != parent && ability.HasCooldown && ability.CompOfType<AbilityComp_Cooldown>() != null);

            //Not independent
            abilities = abilities.Where(ability => !ability.CompOfType<AbilityComp_Cooldown>().IsIndependent);

            if (VProps.cooldownPool != null)
            {
                //Filter out abilities that don't have the required pool identifier.
                abilities = abilities.Where(ability => ability.CompOfType<AbilityComp_Cooldown>().CooldownPool == CooldownPool);
            }

            if (!VProps.resetsTimer)
            {
                //Filter out abilities that are currently cooling down.
                abilities = abilities.Where(ability => ability.CooldownTicksRemaining <= 0);
            }

            foreach (Ability ability in abilities)
            {
                if (VProps.cooldownTicksRange == default(IntRange))
                {
                    //If no cooldown set, use the ability's own cooldown value.
                    ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
                }
                else
                {
                    //If cooldown is set, use it instead.
                    ability.StartCooldown(VProps.cooldownTicksRange.RandomInRange);
                }
            }
        }
    }
}
