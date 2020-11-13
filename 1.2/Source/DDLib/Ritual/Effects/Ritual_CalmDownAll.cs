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
    public class Ritual_CalmDownAll : Ritual_AoE
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
        }

        protected override void PreActivation()
        {
            IEnumerable<Pawn> CalmedDownPawns = base.AllTargetedPawns.Where(target => target.InAggroMentalState);

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
                        IEnumerable<Pawn> pawns = AllTargetedPawns.Where(target => defs.Any(d => target.health.hediffSet.HasHediff(d)));

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
