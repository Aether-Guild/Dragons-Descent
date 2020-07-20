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
    public class RegenHediffModExtension : DefModExtension
    {
        public bool keepWhileFighting = false;
    }

    public abstract class HediffCompProperties_BaseRegen : HediffCompProperties
    {
        public virtual bool ShouldKeepWhileFighting(HediffDef def)
        {
            if (def.HasModExtension<RegenHediffModExtension>())
            {
                return def.GetModExtension<RegenHediffModExtension>().keepWhileFighting;
            }

            return false;
        }
    }

    public class HediffCompProperties_Regen : HediffCompProperties_BaseRegen
    {
        public SimpleCurve healAmount, healIntervalTicks;

        public virtual float GetHealAmount(Pawn pawn)
        {
            return healAmount.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
        }

        public virtual int GetHealIntervalTicks(Pawn pawn)
        {
            return Mathf.RoundToInt(healIntervalTicks.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat));
        }
    }
}
