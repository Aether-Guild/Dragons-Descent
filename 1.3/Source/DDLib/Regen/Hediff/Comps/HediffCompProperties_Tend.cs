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
    public class HediffCompProperties_Tend : HediffCompProperties_BaseRegen
    {
        public SimpleCurve tendQuality, maxTendQuality, tendIntervalTicks;

        public virtual float GetTendQuality(Pawn pawn)
        {
            return tendQuality.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
        }

        public virtual float GetMaxTendQuality(Pawn pawn)
        {
            if (maxTendQuality != null)
            {
                return maxTendQuality.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
            }
            else
            {
                return tendQuality.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat) * 1.75f;
            }
        }

        public virtual int GetTendIntervalTicks(Pawn pawn)
        {
            return Mathf.RoundToInt(tendIntervalTicks.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat));
        }
    }
}
