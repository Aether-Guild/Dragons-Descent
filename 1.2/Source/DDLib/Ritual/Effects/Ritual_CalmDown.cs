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
    public class Ritual_CalmDown : Ritual_Targeting
    {
        public override void ApplyRitual(Pawn target)
        {
            if (target.InAggroMentalState)
            {
                target.ClearMind();

                Messages.Message("RitualCalmDownMessage".Translate(target.LabelCap.Named("TARGET")), target, MessageTypeDefOf.PositiveEvent);
            }

            if (Def.HasModExtension<RitualHediffModExtension>() && target.health != null && target.health.hediffSet != null)
            {
                foreach(List<HediffDef> defs in Def.modExtensions.OfType<RitualHediffModExtension>().Select(ext => ext.hediffs))
                {
                    foreach(HediffDef def in defs)
                    {
                        if (target.health.hediffSet.HasHediff(def))
                        {
                            target.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == def);

                            Messages.Message("RitualHediffRemoveMessage".Translate(target.LabelCap.Named("TARGET"), def.LabelCap.Named("HEDIFF")), target, MessageTypeDefOf.PositiveEvent);
                        }
                    }
                }
            }
        }
    }
}
