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
    public class Ritual_WardAllPawns : Ritual_AoETicking
    {
        public override void ApplyRitual(Pawn target)
        {
            if (target.InAggroMentalState)
            {
                target.ClearMind();
            }

            if (Def.HasModExtension<RitualHediffModExtension>() && target.health != null && target.health.hediffSet != null)
            {
                foreach (List<HediffDef> defs in Def.modExtensions.OfType<RitualHediffModExtension>().Select(ext => ext.hediffs))
                {
                    foreach(HediffDef def in defs)
                    {
                        if (target.health.hediffSet.HasHediff(def))
                        {
                            target.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == def);
                        }
                    }
                }
            }

            if (target.Faction == null && target.mindState.exitMapAfterTick != 0)
            {
                //target.ClearMind();
                target.mindState.exitMapAfterTick = 0;
                //target.jobs.StopAll();
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
