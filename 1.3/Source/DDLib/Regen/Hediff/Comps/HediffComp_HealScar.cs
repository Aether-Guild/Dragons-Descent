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
    public class HediffComp_HealScar : HediffComp
    {
        public int ticksTillHeal;

        //dbg
        public Hediff lastHealedHediff;
        public float lastSeenSeverity;

        //Don't need to regen anymore, if shouldn't have it while fighting and pawn is fighting.
        public override bool CompShouldRemove => !HealthUtils.ShouldRegen(Pawn);

        public HediffCompProperties_Scar Props => (HediffCompProperties_Scar)props;

        private int NextHealTick => Mathf.RoundToInt(Props.GetHealIntervalTicks(Pawn) * (1f - parent.Severity));

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksTillHeal, "ticksTillHeal", 0);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (ticksTillHeal >= NextHealTick)
            {
                HealScar(Pawn);

                ticksTillHeal = 0;
            } else
            {
                ticksTillHeal++;
            }
        }

        public override string CompDebugString()
        {
            return "ticksTillHeal=" + ticksTillHeal + "/" + NextHealTick + "\nlastHealedHediff=" + lastHealedHediff.ToStringSafe() + " (" + lastSeenSeverity + ")";
        }

        protected virtual bool HealScar(Pawn pawn)
        {
            IEnumerable<Hediff> hediffs = Pawn.health.hediffSet.hediffs;
            IEnumerable<Hediff_Injury> injuries = hediffs.OfType<Hediff_Injury>();

            Hediff selected = null;

            if (injuries.Where(injury => injury.IsPermanent()).TryRandomElement(out selected))
            {
                bool result = Heal(pawn, selected);
                if (selected.IsPermanent() && selected.Severity <= 0 && PawnUtility.ShouldSendNotificationAbout(Pawn))
                {
                    Messages.Message("MessagePermanentWoundHealed".Translate(parent.LabelCap, Pawn.LabelShort, selected.Label, Pawn.Named("PAWN")), Pawn, MessageTypeDefOf.PositiveEvent);
                }
                return result;
            }

            //No more scars.
            return false;
        }

        private bool Heal(Pawn pawn, Hediff hediff)
        {
            lastHealedHediff = hediff;
            lastSeenSeverity = hediff.Severity;

            HealthUtils.Heal(hediff, hediff.Severity);

            return true;
        }
    }
}
