using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public static class HealthUtils
    {
        //Can activate regen (timeout complete)
        public static bool CanRegen(Pawn pawn, float timeout = 60f)
        {
            int curTick = GenTicks.TicksGame;
            int timeoutTicks = +GenTicks.SecondsToTicks(timeout);
            return (pawn.mindState.lastAttackTargetTick + timeoutTicks) < curTick && (pawn.mindState.lastHarmTick + timeoutTicks) < curTick;
        }

        //Is damaged
        public static bool ShouldRegen(Pawn pawn) => pawn.health.hediffSet.hediffs.Any(hediff => hediff.Bleeding || hediff is Hediff_Injury);

        //Is bleeding && Started to regen.
        public static bool ShouldLieDown(Pawn pawn) => CanRegen(pawn) && pawn.health.hediffSet.hediffs.Any(hediff => hediff.Bleeding) && pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicRegeneration, true);

        public static bool Heal(Hediff hediff, float healAmount)
        {
            hediff.Severity -= healAmount;
            if (hediff.Severity <= 0 && hediff is Hediff_MissingPart)
            {
                (hediff as Hediff_MissingPart).IsFresh = false;
            }

            return true;
        }

        public static bool ShouldBeDoctored(Pawn pawn)
        {
            //Not scar & Not fully immunized & Is neither an injury nor is bleeding & requires tending.
            return pawn.health.hediffSet.hediffs.Any(hediff => !hediff.IsPermanent() && !hediff.FullyImmune() && !(hediff is Hediff_Injury || hediff.Bleeding) && hediff.TendableNow());
        }

        public static bool ShouldTend(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs.Any(hd => hd.Bleeding || (hd is Hediff_Injury && hd.TendableNow()));
        }

        public static void Tend(List<Hediff> hediffs, float quality, float maxQuality)
        {
            for (int i = 0; i < hediffs.Count; i++)
            {
                hediffs[i].Tended(Rand.Range(quality, maxQuality), maxQuality, i);
            }
        }
    }
}
