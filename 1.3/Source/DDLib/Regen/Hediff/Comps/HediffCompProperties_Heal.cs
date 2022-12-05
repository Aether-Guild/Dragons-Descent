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
    public class HediffCompProperties_Heal : HediffCompProperties_BaseRegen
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
