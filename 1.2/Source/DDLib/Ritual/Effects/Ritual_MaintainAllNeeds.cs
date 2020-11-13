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
}
