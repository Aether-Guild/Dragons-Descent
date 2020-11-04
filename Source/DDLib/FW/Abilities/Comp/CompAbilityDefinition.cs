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

        public IEnumerable<AbilityGainEntry> GainableAbilities => Props.abilities.Where(entry => !entry.HasAbility(SelfPawn));

        private float damageTotal;
        private int killCounter;

        public float DamageTotal => damageTotal;
        public int KillCounter => killCounter;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (respawningAfterLoad)
            {
                //Process only when spawning. Not when respawning.
                foreach (AbilityGainEntry entry in Props.abilities.Where(entry => !entry.HasAbility(SelfPawn) && !entry.HasHediff(SelfPawn) && entry.ConditionsFulfilled(this)))
                {
                    //Doesn't have the ability, but fulfills the conditions. So act like they already had it.
                    entry.GainAbility(SelfPawn);
                }
                return;
            }

            if (!Props.spawnKillCount.EnumerableNullOrEmpty())
            {
                //If set, spawn the pawn with kills based on its age.
                killCounter = Mathf.FloorToInt(Props.spawnKillCount.Evaluate(SelfPawn.ageTracker.AgeBiologicalYearsFloat));
            }

            if (!Props.spawnDamageTotal.EnumerableNullOrEmpty())
            {
                //If set, spawn the pawn with damage based on its age.
                damageTotal = Props.spawnDamageTotal.Evaluate(SelfPawn.ageTracker.AgeBiologicalYearsFloat);
            }

            foreach (AbilityGainEntry entry in Props.abilities.Where(entry => !entry.HasAbility(SelfPawn) && !entry.HasHediff(SelfPawn) && entry.ConditionsSatisfied(this)))
            {
                //Doesn't have the ability, but satisfies the conditions. So act like they already had it.
                entry.GainAbility(SelfPawn);
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            damageTotal += dinfo.Amount;
        }

        public override void Notify_KilledPawn(Pawn pawn)
        {
            killCounter++;
        }

        public override string CompInspectStringExtra()
        {
            return base.CompInspectStringExtra();
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref damageTotal, "damageTotal", 0);
            Scribe_Values.Look(ref killCounter, "killCounter", 0);
        }

        public override void CompTickRare()
        {
            foreach (AbilityGainEntry entry in Props.abilities.Where(entry => !entry.HasAbility(SelfPawn) && !entry.HasHediff(SelfPawn) && entry.ConditionsSatisfied(this)))
            {
                //Doesn't have the ability, but satisfies the conditions.
                if (entry.ShouldGainHediff)
                {
                    if (entry.ConditionsFulfilled(this))
                    {
                        //Gain the ability. (skipping hediff)
                        entry.GainAbility(SelfPawn);
                    }
                    else
                    {
                        //Gain the hediff.
                        entry.GainHediff(SelfPawn);
                    }
                }
                else
                {
                    //Gain the ability directly.
                    entry.GainAbility(SelfPawn);
                }
            }
        }
    }
}
