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
    public class Ritual_WardAllPawns : Ritual_MapTargetingTicking
    {
        public override void ApplyRitual(TargetInfo target)
        {
            if (target.HasThing && !target.ThingDestroyed && target.Thing is Pawn pawn)
            {
                if (pawn.InAggroMentalState)
                {
                    pawn.ClearMind();
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
                            }
                        }
                    }
                }

                if (pawn.Faction == null && pawn.mindState.exitMapAfterTick != 0)
                {
                    pawn.mindState.exitMapAfterTick = 0;
                }
            }
        }

        protected override void PostActivation()
        {
            Messages.Message("RitualWardOnMessage".Translate(Duration.ToStringTicksToPeriodVague().Named("DURATION")), MessageTypeDefOf.PositiveEvent);
        }

        protected override void PostDeactivation()
        {
            Messages.Message("RitualWardOffMessage".Translate(), MessageTypeDefOf.NegativeEvent);
        }
    }
}
