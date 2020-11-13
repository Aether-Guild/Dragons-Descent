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
    public class CompAbilityDefinition : ThingComp
    {
        public CompProperties_AbilityDefinition Props => (CompProperties_AbilityDefinition)props;

        public Pawn SelfPawn => (Pawn)parent;

        public IEnumerable<AbilityDefinitionEntry> GainableAbilities => Props.abilities.Where(entry => !entry.HasAbility(SelfPawn));

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (SelfPawn.abilities == null)
            {
                //Failsafe. Add <hasAbilities> to this pawn's ThingDef.
                Log.Error("Pawn " + parent + " has " + typeof(CompAbilityDefinition).Name + " but can't learn abilities.");
                return;
            }

            if (respawningAfterLoad)
            {
                //Process only when spawning. Not when respawning.
                //Doesn't have the ability, but fulfills the conditions. So act like they already had it.
                AbilitySettingsUtility.TryGainAbilities(SelfPawn, Props.abilities, true, true);
                return;
            }

            //Doesn't have the ability, but satisfies the conditions. So act like they already had it.
            AbilitySettingsUtility.TryGainAbilities(SelfPawn, Props.abilities, true);
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            CompTickRare();
        }

        public override void Notify_KilledPawn(Pawn pawn)
        {
            CompTickRare();
        }

        public override void CompTickRare()
        {
            if (SelfPawn.abilities == null)
            {
                return;
            }

            AbilitySettingsUtility.TryRemoveAbilities(SelfPawn, Props.abilities);
            AbilitySettingsUtility.TryGainAbilities(SelfPawn, Props.abilities);
        }
    }
}
