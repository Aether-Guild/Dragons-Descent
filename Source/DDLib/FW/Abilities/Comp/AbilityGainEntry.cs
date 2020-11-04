using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityGainEntry
    {
        public AbilityDef abilityDef;

        public HediffDef hediffDef;
        public BodyPartDef bodyPartDef;

        public List<AbilityGainCondition> conditions;

        public bool ShouldGainHediff => hediffDef != null;

        public bool ConditionsSatisfied(CompAbilityDefinition def)
        {
            if(!conditions.NullOrEmpty())
            {
                foreach (AbilityGainCondition cond in conditions)
                {
                    if (!cond.IsSatisfied(def))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool ConditionsFulfilled(CompAbilityDefinition def)
        {
            if(!conditions.NullOrEmpty())
            {
                foreach (AbilityGainCondition cond in conditions)
                {
                    if (!cond.IsFulfilled(def))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool HasAbility(Pawn pawn)
        {
            Ability ab = pawn.abilities.GetAbility(abilityDef);
            return pawn.abilities.GetAbility(abilityDef) != null;
        }

        public void GainAbility(Pawn pawn)
        {
            pawn.abilities.GainAbility(abilityDef);
        }

        public bool HasHediff(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(hediffDef);
        }

        public void GainHediff(Pawn pawn)
        {
            List<Hediff> hediffs = new List<Hediff>();
            List<BodyPartDef> bodyPart = null;

            if(bodyPartDef != null)
            {
                bodyPart = new List<BodyPartDef>() { bodyPartDef };
            }

            HediffGiverUtility.TryApply(pawn, hediffDef, bodyPart, outAddedHediffs: hediffs);
            foreach (HediffComp_GrowthSeverityScaling comp in hediffs.Select(h => h.TryGetComp<HediffComp_GrowthSeverityScaling>()).OfType<HediffComp_GrowthSeverityScaling>())
            {
                comp.abilityDef = abilityDef;
            }
        }
    }
}
