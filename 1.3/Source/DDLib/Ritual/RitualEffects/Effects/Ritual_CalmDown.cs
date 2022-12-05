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
        public override void ApplyRitual(TargetInfo target)
        {
            if (target.HasThing && !target.ThingDestroyed && target.Thing is Pawn pawn)
            {
                if (pawn.InAggroMentalState)
                {
                    pawn.ClearMind();

                    Messages.Message("RitualCalmDownMessage".Translate(pawn.LabelCap.Named("TARGET")), target, MessageTypeDefOf.PositiveEvent);
                }

                if (Def.HasModExtension<RitualHediffModExtension>() && pawn.health != null && pawn.health.hediffSet != null)
                {
                    foreach (List<HediffDef> defs in Def.modExtensions.OfType<RitualHediffModExtension>().Select(ext => ext.hediffs))
                    {
                        foreach (HediffDef def in defs)
                        {
                            if (pawn.health.hediffSet.HasHediff(def))
                            {
                                pawn.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == def);

                                Messages.Message("RitualHediffRemoveMessage".Translate(pawn.LabelCap.Named("TARGET"), def.LabelCap.Named("HEDIFF")), target, MessageTypeDefOf.PositiveEvent);
                            }
                        }
                    }
                }
            }
        }
    }
}
