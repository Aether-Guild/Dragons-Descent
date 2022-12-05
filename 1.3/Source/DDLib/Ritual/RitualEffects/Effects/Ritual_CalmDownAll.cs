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
    public class Ritual_CalmDownAll : Ritual_MapTargeting
    {
        private IEnumerable<Pawn> AffectedPawns => AllTargets.Where(target => target.HasThing && !target.ThingDestroyed).Select(target => target.Thing).OfType<Pawn>();

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
            }
        }

        protected override void PreActivation()
        {
            IEnumerable<Pawn> CalmedDownPawns = AffectedPawns.Where(target => target.InAggroMentalState);

            if (CalmedDownPawns.Any())
            {
                Messages.Message("RitualCalmDownAllMessage".Translate(CalmedDownPawns.Count().Named("COUNT")), new LookTargets(CalmedDownPawns), MessageTypeDefOf.PositiveEvent);
            }

            if (Def.HasModExtension<RitualHediffModExtension>())
            {
                foreach(List<HediffDef> defs in Def.modExtensions.OfType<RitualHediffModExtension>().Select(ext => ext.hediffs))
                {
                    foreach(HediffDef def in defs)
                    {
                        IEnumerable<Pawn> pawns = AffectedPawns.Where(target => defs.Any(d => target.health.hediffSet.HasHediff(d)));

                        if (pawns.Any())
                        {
                            Messages.Message("RitualHediffRemoveAllMessage".Translate(string.Join(",", defs.Select(d => d.LabelCap).Distinct()).Named("HEDIFF"), pawns.Count().Named("COUNT")), new LookTargets(pawns), MessageTypeDefOf.PositiveEvent);
                        }
                    }
                }
            }
        }
    }
}
