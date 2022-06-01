using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityDefinitionEntry
    {
        public AbilityDef abilityDef;

        public HediffDef growthHediff;
        public BodyPartDef growthBodyPart;

        public AbilityCondition gainCondition, loseCondition;

        public bool ShouldGainHediff => growthHediff != null;

        public bool ConditionsSatisfied(Pawn pawn)
        {
            if(ShouldRemove(pawn))
            {
                //Don't add if should be removed.
                return false;
            }

            if (growthBodyPart != null && pawn.health.hediffSet.GetNotMissingParts().All(part => part.def != growthBodyPart))
            {
                //Avoid trying to add the hediff if the part is missing.
                return false;
            }

            if (gainCondition == null)
            {
                //No gain condition then always possible to add.
                return true;
            }

            return gainCondition.IsSatisfied(pawn);
        }

        public bool ConditionsFulfilled(Pawn pawn)
        {
            if (ShouldRemove(pawn))
            {
                //Don't add if should be removed.
                return false;
            }

            if (growthBodyPart != null && pawn.health.hediffSet.GetNotMissingParts().All(part => part.def != growthBodyPart))
            {
                //Avoid trying to add the hediff if the part is missing.
                return false;
            }

            if (gainCondition == null)
            {
                //No gain condition then always possible to add.
                return true;
            }

            return gainCondition.IsFulfilled(pawn);
        }

        public bool ShouldRemove(Pawn pawn)
        {
            if(loseCondition == null)
            {
                //No removal condition.
                return false;
            }

            return loseCondition.IsSatisfied(pawn);
        }

        public bool HasAbility(Pawn pawn)
        {
            return pawn.abilities.GetAbility(abilityDef) != null;
        }

        public void GainAbility(Pawn pawn)
        {
            pawn.abilities.GainAbility(abilityDef);
        }

        public void RemoveAbility(Pawn pawn)
        {
            pawn.abilities.RemoveAbility(abilityDef);
        }

        public bool HasHediff(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(growthHediff);
        }

        public void GainHediff(Pawn pawn)
        {
            List<Hediff> hediffs = new List<Hediff>();
            List<BodyPartDef> bodyPart = null;

            if(growthBodyPart != null)
            {
                bodyPart = new List<BodyPartDef>() { growthBodyPart };
            }

            HediffGiverUtility.TryApply(pawn, growthHediff, bodyPart, outAddedHediffs: hediffs);
            foreach (HediffComp_GrowthSeverityScaling comp in hediffs.Select(h => h.TryGetComp<HediffComp_GrowthSeverityScaling>()).OfType<HediffComp_GrowthSeverityScaling>())
            {
                comp.abilityDef = abilityDef;
            }
        }

        public void RemoveHediff(Pawn pawn)
        {
            IEnumerable<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where(hediff => hediff.def == growthHediff && hediff.TryGetComp<HediffComp_GrowthSeverityScaling>().abilityDef == abilityDef);

            if(growthBodyPart != null)
            {
                hediffs = hediffs.Where(hediff => hediff?.Part.def == growthBodyPart);
            }

            foreach (Hediff hediff in hediffs)
            {
                hediff.Severity = 0;
            }
        }
    }
}
