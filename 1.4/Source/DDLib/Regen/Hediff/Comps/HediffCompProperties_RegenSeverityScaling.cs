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
    public class HediffCompProperties_RegenSeverityScaling : HediffCompProperties_BaseRegen
    {
        public SimpleCurve interval;

        public float MaximumTicks => interval.Points.Max(pt => pt.y);

        public HediffCompProperties_RegenSeverityScaling()
        {
            compClass = typeof(HediffComp_RegenSeverityScaling);
        }
        public virtual bool ShouldRegenBeActive(Pawn pawn)
        {
            return pawn.GetPosture().Laying();
        }

        public virtual float GetSeverityAt(int ticks)
        {
            return interval.Evaluate(ticks);
        }

        public virtual int GetTicksAt(float severity)
        {
            return Mathf.RoundToInt(interval.EvaluateInverted(severity));
        }

        public float GetStableSeverity(HediffStage stage)
        {
            if (stage is HediffStage_Adjusting)
            {
                return (stage as HediffStage_Adjusting).severity;
            }

            return stage.minSeverity;
        }

    }
}
