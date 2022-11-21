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
    public class HediffComp_TendInjury : HediffComp
    {
        public int ticksTillTend;

        //dbg
        public Hediff lastTendedHediff;
        public float lastSeenSeverity;

        private List<Hediff> injuries;

        //Don't need to regen anymore, if shouldn't have it while fighting and pawn is fighting.
        public override bool CompShouldRemove => !HealthUtils.ShouldRegen(Pawn);

        public HediffCompProperties_Tend Props => (HediffCompProperties_Tend)props;

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksTillTend, "ticksTillTend", 0);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (ticksTillTend <= 0)
            {
                if (HasHediffsNeedingTend(Pawn))
                {
                    TendInjury(Pawn);

                    ticksTillTend = Props.GetTendIntervalTicks(Pawn);
                }
            }
            else
            {
                ticksTillTend--;
            }
        }

        public override string CompDebugString()
        {
            return "hasHediffs=" + HasHediffsNeedingTend(Pawn) + "\nticksTillTend=" + ticksTillTend + "/" + Props.GetTendIntervalTicks(Pawn) + "\ntendQuality=" + Props.GetTendQuality(Pawn) * parent.Severity + "\nlastTendedHediff=" + lastTendedHediff.ToStringSafe() + " (" + lastSeenSeverity + ")";
        }

        protected virtual bool HasHediffsNeedingTend(Pawn pawn) => !injuries.NullOrEmpty() || HealthUtils.ShouldTend(Pawn);

        protected virtual bool TendInjury(Pawn pawn)
        {
            if (injuries.NullOrEmpty())
            {
                if (injuries == null)
                {
                    injuries = new List<Hediff>();
                }
                TendUtility.GetOptimalHediffsToTendWithSingleTreatment(pawn, false, injuries, pawn.health.hediffSet.hediffs.Where(hd => hd is Hediff_Injury || hd.Bleeding).ToList());
            }

            injuries.RemoveAll(hd => hd.IsTended());

            Hediff hediff = injuries.RandomElementWithFallback();
            if (hediff != null)
            {
                //TendUtility.DoTend(pawn, pawn, null);
                Tend(pawn, injuries);

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Tend(Pawn pawn, List<Hediff> hediffs)
        {
            lastTendedHediff = hediffs.Last();
            lastSeenSeverity = lastTendedHediff.Severity;

            HealthUtils.Tend(hediffs, Props.GetTendQuality(pawn) * parent.Severity, Props.GetMaxTendQuality(pawn));

            return true;
        }
    }
}
