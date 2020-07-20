using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public static class HealthUtils
    {
        public static bool CanRegen(Pawn pawn, bool giveWhileFighting = false)
        {
            if (giveWhileFighting || (!giveWhileFighting && !pawn.IsFighting()))
            {
                if (pawn.health.hediffSet.hediffs.Where(hediff => hediff.Bleeding || hediff is Hediff_Injury).Any())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ShouldLieDown(Pawn pawn)
        {
            //Not fighting && Is bleeding && Started to regen.
            return !pawn.IsFighting() && pawn.health.hediffSet.hediffs.Where(hediff => hediff.Bleeding).Any() && pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicRegeneration, true);
        }

        public static bool Heal(Hediff hediff, float healAmount)
        {
            hediff.Severity -= healAmount;
            if (hediff.Severity <= 0 && hediff is Hediff_MissingPart)
            {
                (hediff as Hediff_MissingPart).IsFresh = false;
            }

            return true;
        }
    }
}
