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
    public class Ritual_MaintainNeeds : Ritual_TargetingTicking
    {
        private void Maintain(Need need, FloatRange range)
        {
            if(!range.Includes(need.CurLevelPercentage))
            {
                need.CurLevelPercentage = range.ClampToRange(need.CurLevelPercentage);
            }
        }

        public override void ApplyRitual(Pawn target)
        {
            RitualNeedsModExtension ext = Def.GetModExtension<RitualNeedsModExtension>();

            if (ext.food.HasValue)
            {
                Maintain(target.needs.food, ext.food.Value);
            }
            if (ext.rest.HasValue)
            {
                Maintain(target.needs.rest, ext.rest.Value);
            }
            if (ext.comfort.HasValue)
            {
                Maintain(target.needs.comfort, ext.comfort.Value);
            }
            if (ext.joy.HasValue)
            {
                Maintain(target.needs.joy, ext.joy.Value);
            }
            if (ext.mood.HasValue)
            {
                Maintain(target.needs.mood, ext.mood.Value);
            }
            if (ext.drugDesire.HasValue)
            {
                Maintain(target.needs.drugsDesire, ext.drugDesire.Value);
            }
        }

        protected override void PostActivation()
        {
            Messages.Message("RitualControlNeedsOnMessage".Translate(Target.LabelCap.Named("TARGET"), Duration.ToStringTicksToPeriodVague().Named("DURATION")), MessageTypeDefOf.PositiveEvent);
        }

        protected override void PostDeactivation()
        {
            Messages.Message("RitualControlNeedsOffMessage".Translate(Target.LabelCap.Named("TARGET")), MessageTypeDefOf.NegativeEvent);
        }
    }

    public class Ritual_MaintainAllNeeds : Ritual_AoETicking
    {
        private void Maintain(Need need, FloatRange range)
        {
            if (!range.Includes(need.CurLevelPercentage))
            {
                need.CurLevelPercentage = range.ClampToRange(need.CurLevelPercentage);
            }
        }

        public override void ApplyRitual(Pawn target)
        {
            RitualNeedsModExtension ext = Def.GetModExtension<RitualNeedsModExtension>();

            if (ext.food.HasValue)
            {
                Maintain(target.needs.food, ext.food.Value);
            }
            if (ext.rest.HasValue)
            {
                Maintain(target.needs.rest, ext.rest.Value);
            }
            if (ext.comfort.HasValue)
            {
                Maintain(target.needs.comfort, ext.comfort.Value);
            }
            if (ext.joy.HasValue)
            {
                Maintain(target.needs.joy, ext.joy.Value);
            }
            if (ext.mood.HasValue)
            {
                Maintain(target.needs.mood, ext.mood.Value);
            }
            if (ext.drugDesire.HasValue)
            {
                Maintain(target.needs.drugsDesire, ext.drugDesire.Value);
            }
        }

        protected override void PostActivation()
        {
            Messages.Message("RitualControlNeedsAllOnMessage".Translate(Duration.ToStringTicksToPeriodVague().Named("DURATION")), MessageTypeDefOf.PositiveEvent);
        }

        protected override void PostDeactivation()
        {
            Messages.Message("RitualControlNeedsAllOffMessage".Translate(), MessageTypeDefOf.NegativeEvent);
        }
    }

    public class Ritual_BanishPawn : Ritual_Targeting
    {
        public override void ApplyRitual(Pawn target)
        {
            if (target.Faction != null)
            {
                if (target.Faction.IsPlayer && target.InAggroMentalState)
                {
                    target.ClearMind();

                    Messages.Message("RitualCalmDownMessage".Translate(target.LabelCap.Named("TARGET")), target, MessageTypeDefOf.PositiveEvent);
                }
            }
            else
            {
                //target.ClearMind();
                target.mindState.exitMapAfterTick = 0;
                //target.jobs.StopAll();

                Messages.Message("RitualBanishMessage".Translate(target.LabelCap.Named("TARGET")), target, MessageTypeDefOf.NeutralEvent);
            }
        }
    }

    public class Ritual_BanishAllPawns : Ritual_AoE
    {
        public override void ApplyRitual(Pawn target)
        {
            if (target.Faction != null)
            {
                if (target.Faction.IsPlayer && target.InAggroMentalState)
                {
                    target.ClearMind();
                }
            }
            else
            {
                //target.ClearMind();
                target.mindState.exitMapAfterTick = 0;
                //target.jobs.StopAll();
            }
        }

        protected override void PreActivation()
        {
            IEnumerable<Pawn> CalmedDownPawns = base.AllTargetedPawns.Where(target => target.Faction != null && target.Faction.IsPlayer && target.InAggroMentalState);
            IEnumerable<Pawn> BanishedPawns = base.AllTargetedPawns.Where(target => target.Faction == null);

            if (CalmedDownPawns.Any())
            {
                Messages.Message("RitualCalmDownAllMessage".Translate(CalmedDownPawns.Count().Named("COUNT")), new LookTargets(CalmedDownPawns), MessageTypeDefOf.PositiveEvent);
            }

            if (BanishedPawns.Any())
            {
                Messages.Message("RitualBanishAllMessage".Translate(BanishedPawns.Count().Named("COUNT")), new LookTargets(BanishedPawns), MessageTypeDefOf.NeutralEvent);
            }
        }
    }

    public class Ritual_RemoveHediff : Ritual_Targeting
    {
        public override void ApplyRitual(Pawn target)
        {
            if (Def.HasModExtension<RitualHediffModExtension>() && target.health != null && target.health.hediffSet != null)
            {
                foreach(HediffDef def in Def.GetModExtension<RitualHediffModExtension>().hediffs)
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

    public class Ritual_RemoveHediffAll : Ritual_AoE
    {
        public override void ApplyRitual(Pawn target)
        {
            if (Def.HasModExtension<RitualHediffModExtension>() && target.health != null && target.health.hediffSet != null)
            {
                foreach (HediffDef def in Def.GetModExtension<RitualHediffModExtension>().hediffs)
                {
                    if (target.health.hediffSet.HasHediff(def))
                    {
                        target.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == def);
                    }
                }
            }
        }

        protected override void PreActivation()
        {
            if (Def.HasModExtension<RitualHediffModExtension>())
            {
                IEnumerable<HediffDef> defs = Def.GetModExtension<RitualHediffModExtension>().hediffs;

                IEnumerable<Pawn> pawns = AllTargetedPawns.Where(target => defs.Any(def => target.health.hediffSet.HasHediff(def)));
                
                if(pawns.Any())
                {
                    Messages.Message("RitualHediffRemoveAllMessage".Translate(string.Join(",", defs.Select(d => d.LabelCap).Distinct()).Named("HEDIFF"), pawns.Count().Named("COUNT")), new LookTargets(pawns), MessageTypeDefOf.PositiveEvent);
                }
            }
        }
    }

    public class Ritual_BanishAllPawnsWard : Ritual_AoETicking
    {
        public override void ApplyRitual(Pawn target)
        {
            if (target.Faction != null && target.Faction.IsPlayer && target.InAggroMentalState)
            {
                target.ClearMind();
            }

            if (Def.HasModExtension<RitualHediffModExtension>() && target.health != null && target.health.hediffSet != null)
            {
                foreach (HediffDef def in Def.GetModExtension<RitualHediffModExtension>().hediffs)
                {
                    if (target.health.hediffSet.HasHediff(def))
                    {
                        target.health.hediffSet.hediffs.RemoveAll(hediff => hediff.def == def);
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
