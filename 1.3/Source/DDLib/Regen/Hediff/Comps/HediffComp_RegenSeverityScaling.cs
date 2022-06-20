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
    public class HediffComp_RegenSeverityScaling : HediffComp
    {
        private HediffStage_Adjusting stableStage;
        private float startSeverity, endSeverity;

        public int ticks, requiredTicks;

        public bool remove = false;

        public override bool CompShouldRemove => base.CompShouldRemove || remove;

        public HediffCompProperties_RegenSeverityScaling Props => (HediffCompProperties_RegenSeverityScaling)props;

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticks, "ticks", 0);
        }

        private void SelectStage(HediffStage_Adjusting stage)
        {
            if(stage != stableStage)
            {
                stableStage = stage;
                startSeverity = parent.Severity;
                endSeverity = stableStage.severity;
                ticks = 0;
                requiredTicks = Math.Abs(Props.GetTicksAt(stableStage.severity) - Props.GetTicksAt(parent.Severity));
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            //Initialize Current Stage if unset.
            if (stableStage == null)
            {
                SelectStage((HediffStage_Adjusting)parent.CurStage);
            }

            //Decide which stage is the correct one to target.
            if (HealthUtils.ShouldRegen(Pawn))
            {
                if (Props.ShouldRegenBeActive(Pawn))
                {
                    SelectStage((HediffStage_Adjusting)Def.stages[2]);
                }
                else
                {
                    SelectStage((HediffStage_Adjusting)Def.stages[1]);
                }
            }
            else
            {
                SelectStage((HediffStage_Adjusting)Def.stages[0]);
            }

            //Make a slight adjustment to towards the stable stage.
            if (parent.Severity != stableStage.severity)
            {
                if (requiredTicks != 0)
                {
                    severityAdjustment = Mathf.Lerp(startSeverity, endSeverity, (float)ticks / (float)requiredTicks) - parent.Severity;
                    ticks++;
                    if (ticks > requiredTicks)
                    {
                        ticks = 0;
                    }
                }
            }
        }

        public override string CompDebugString()
        {
            return "ticks=" + ticks + "/" + requiredTicks + (requiredTicks != 0 ? " (" + ((ticks / requiredTicks) * 100) + "%)" : "");
        }

        public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
        {
            if(verb.HarmsHealth())
            {
                remove = true;
            }
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            remove = true;
        }
    }
}
