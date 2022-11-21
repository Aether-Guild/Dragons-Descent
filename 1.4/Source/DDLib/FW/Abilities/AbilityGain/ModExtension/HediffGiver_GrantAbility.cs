using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class HediffGiver_GrantAbility : HediffGiver
    {
        private IEnumerable<AbilityDefinitionEntry> GetEntries(Pawn pawn)
        {
            return pawn.def.modExtensions.OfType<AbilityDefinitionExtension>().SelectMany(ext => ext.abilities);
        }

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if(!pawn.def.HasModExtension<AbilityDefinitionExtension>())
            {
                //No definitions.
                return;
            }

            IEnumerable<AbilityDefinitionEntry> entries = GetEntries(pawn);

            AbilitySettingsUtility.TryRemoveAbilities(pawn, entries);
            AbilitySettingsUtility.TryGainAbilities(pawn, entries);
        }
    }
}
