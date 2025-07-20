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
    public class Ritual_MaintainAllNeeds : Ritual_MapTargetingTicking
    {
        private void Maintain(Need need, FloatRange range)
        {
            if (!range.Includes(need.CurLevelPercentage))
            {
                need.CurLevelPercentage = range.ClampToRange(need.CurLevelPercentage);
            }
        }

        public override void ApplyRitual(TargetInfo target)
        {
            if(target.HasThing && !target.ThingDestroyed && target.Thing is Pawn pawn)
            {
                RitualNeedsModExtension ext = Def.GetModExtension<RitualNeedsModExtension>();

                if (ext.food.HasValue)
                {
                    Maintain(pawn.needs.food, ext.food.Value);
                }
                if (ext.rest.HasValue)
                {
                    Maintain(pawn.needs.rest, ext.rest.Value);
                }
                if (ext.comfort.HasValue)
                {
                    Maintain(pawn.needs.comfort, ext.comfort.Value);
                }
                if (ext.joy.HasValue)
                {
                    Maintain(pawn.needs.joy, ext.joy.Value);
                }
                if (ext.mood.HasValue)
                {
                    Maintain(pawn.needs.mood, ext.mood.Value);
                }
                if (ext.drugDesire.HasValue)
                {
                    Maintain(pawn.needs.drugsDesire, ext.drugDesire.Value);
                }
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
