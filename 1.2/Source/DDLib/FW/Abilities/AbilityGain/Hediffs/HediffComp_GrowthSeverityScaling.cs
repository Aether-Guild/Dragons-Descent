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
    public class HediffComp_GrowthSeverityScaling : HediffComp
    {
        public int ticks;

        public AbilityDef abilityDef;

        public HediffCompProperties_GrowthSeverityScaling Props => (HediffCompProperties_GrowthSeverityScaling)props;

        //If no longer in range or unable to gain abilities, if grant ability was set, check if already has the ability.
        public override bool CompShouldRemove => !SeverityInRange || parent.pawn.abilities == null || (abilityDef != null && parent.pawn.abilities.GetAbility(abilityDef) != null);

        public bool SeverityInRange => Props.severityRange.TrueMin < parent.Severity && parent.Severity < Props.severityRange.TrueMax;

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticks, "ticks", 0);
            Scribe_Defs.Look(ref abilityDef, "abilityDef");
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (abilityDef != null && !SeverityInRange)
            {
                //Fanfare: Prolly message?
                parent.pawn.abilities.GainAbility(abilityDef);
                Messages.Message("AbilityGainHediffMessage".Translate(parent.pawn.Named("PAWN"), abilityDef.LabelCap.Named("ABILITY")), parent.pawn, MessageTypeDefOf.PositiveEvent);
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            ticks++;
            severityAdjustment = Props.GetSeverityAt(ticks) - parent.Severity;
        }

        public override string CompLabelInBracketsExtra => base.CompLabelInBracketsExtra + "(" + abilityDef + " " + (Props.GetTicksAt(Props.severityRange.TrueMax) - ticks).ToStringTicksToPeriodVague(false) + ")";
        public override string CompDebugString() => "ticks=" + ticks + "/" + Props.GetTicksAt(Props.severityRange.TrueMax) + (abilityDef != null ? "\nApply on Completion: " + abilityDef.defName : "");
    }
}
