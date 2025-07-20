using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    public class HediffCompProperties_GrowthSeverityScaling : HediffCompProperties_BaseRegen
    {
        public SimpleCurve interval;
        public FloatRange severityRange;

        public float MaximumTicks => interval.Points.Max(pt => pt.y);

        public HediffCompProperties_GrowthSeverityScaling()
        {
            compClass = typeof(HediffComp_GrowthSeverityScaling);
        }

        public virtual float GetSeverityAt(int ticks)
        {
            return interval.Evaluate(ticks);
        }

        public virtual int GetTicksAt(float severity)
        {
            return Mathf.RoundToInt(interval.EvaluateInverted(severity));
        }
    }
}
