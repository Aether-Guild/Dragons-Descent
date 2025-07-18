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
    public class HediffComp_HealInjury : HediffComp
    {
        public int ticksTillHeal;

        //dbg
        public Hediff lastHealedHediff;
        public float lastSeenSeverity;

        //Don't need to regen anymore, if shouldn't have it while fighting and pawn is fighting.
        public override bool CompShouldRemove => !HealthUtils.ShouldRegen(Pawn);

        public HediffCompProperties_Heal Props => (HediffCompProperties_Heal)props;

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksTillHeal, "ticksSinceHeal", 0);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (ticksTillHeal <= 0)
            {
                HealInjury(Pawn);

                ticksTillHeal = Props.GetHealIntervalTicks(Pawn);
            }
            else
            {
                ticksTillHeal--;
            }
        }

        public override string CompDebugString()
        {
            return "ticksTillHeal=" + ticksTillHeal + "/" + Props.GetHealIntervalTicks(Pawn) + "\nhealAmount=" + Props.GetHealAmount(Pawn) * parent.Severity + "\nlastHealedHediff=" + lastHealedHediff.ToStringSafe() + " (" + lastSeenSeverity + ")";
        }

        protected virtual bool HealInjury(Pawn pawn)
        {
            IEnumerable<Hediff> hediffs = Pawn.health.hediffSet.hediffs;
            IEnumerable<Hediff_Injury> injuries = hediffs.OfType<Hediff_Injury>();

            Hediff selected = null;

            if (injuries.Where(injury => injury.CauseDeathNow()).TryRandomElement(out selected))
            {
                return Heal(pawn, selected);
            }

            if (hediffs.Where(injury => injury.Bleeding).TryRandomElement(out selected))
            {
                return Heal(pawn, selected);
            }

            if (injuries.Where(injury => injury.CanHealFromTending()).TryRandomElement(out selected))
            {
                return Heal(pawn, selected);
            }

            if (injuries.Where(injury => injury.CanHealNaturally()).TryRandomElement(out selected))
            {
                return Heal(pawn, selected);
            }

            if (injuries.Where(injury => !injury.IsPermanent()).TryRandomElement(out selected))
            {
                return Heal(pawn, selected);
            }

            //No more injuries.
            return false;
        }

        private bool Heal(Pawn pawn, Hediff hediff)
        {
            lastHealedHediff = hediff;
            lastSeenSeverity = hediff.Severity;

            HealthUtils.Heal(hediff, Props.GetHealAmount(pawn) * parent.Severity);

            return true;
        }
    }
}
