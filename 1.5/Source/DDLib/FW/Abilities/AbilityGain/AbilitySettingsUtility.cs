using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public static class AbilitySettingsUtility
    {
        public static void TryGainAbilities(Pawn pawn, IEnumerable<AbilityDefinitionEntry> entries, bool skipHediff = false, bool onlyFulfilled = false)
        {
            //Doesn't currently have the ability nor has the hediff but conditions are satisfied.
            entries = entries.Where(entry => !entry.HasAbility(pawn) && !entry.HasHediff(pawn) && entry.ConditionsSatisfied(pawn));

            if (onlyFulfilled)
            {
                //Processing fulfilled entries.
                entries = entries.Where(entry => entry.ConditionsFulfilled(pawn));
            }

            foreach (AbilityDefinitionEntry entry in entries)
            {
                if (entry.ShouldGainHediff && !skipHediff)
                {
                    //Has a hediff to be given, and not skipping the hediff.
                    if (entry.ConditionsFulfilled(pawn))
                    {
                        //Gain the ability. (skipping hediff)
                        entry.GainAbility(pawn);
                    }
                    else
                    {
                        //Gain the hediff.
                        entry.GainHediff(pawn);
                    }
                }
                else
                {
                    //Gain the ability directly.
                    entry.GainAbility(pawn);
                }
            }
        }

        public static void TryRemoveAbilities(Pawn pawn, IEnumerable<AbilityDefinitionEntry> entries)
        {
            foreach (AbilityDefinitionEntry entry in entries.Where(entry => (entry.HasAbility(pawn) || entry.HasHediff(pawn)) && entry.ShouldRemove(pawn)))
            {
                if (entry.HasAbility(pawn))
                {
                    //Remove abilities that have been gained
                    entry.RemoveAbility(pawn);
                }

                if (entry.ShouldGainHediff && entry.HasHediff(pawn))
                {
                    //Remove the hediff if it is in progress
                    entry.RemoveHediff(pawn);
                }
            }
        }

        public static void DoInfoSettingUI(IEnumerable<Pawn> pawns)
        {
            Find.WindowStack.Add(new Dialog_AbilityBaseInfo(pawns));
        }
    }
}
