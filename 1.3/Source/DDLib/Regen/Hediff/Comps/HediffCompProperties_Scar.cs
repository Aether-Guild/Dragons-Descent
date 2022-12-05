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
    public class HediffCompProperties_Scar : HediffCompProperties_BaseRegen
    {
        public SimpleCurve healIntervalTicks;

        public virtual int GetHealIntervalTicks(Pawn pawn)
        {
            return Mathf.RoundToInt(healIntervalTicks.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat));
        }
    }
}
