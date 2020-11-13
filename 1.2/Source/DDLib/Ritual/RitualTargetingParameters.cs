using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class RitualTargetingParameters : TargetingParameters
    {
        public bool onlyTargetFactionless;

        public bool canTargetWithMentalState;

        public List<HediffDef> canTargetWithHediff;

        public RitualTargetingParameters()
        {
            validator = target =>
            {
                if (target == null || !target.HasThing || target.ThingDestroyed)
                {
                    return false;
                }

                if (!target.Thing.def.HasModExtension<RitualTargetExtension>())
                {
                    return false;
                }

                if (onlyTargetFactionless && target.Thing.Faction != null)
                {
                    return false;
                }

                if (target.Thing is Pawn pawn)
                {
                    if (canTargetWithMentalState && pawn.InMentalState)
                    {
                        return true;
                    }

                    if (!canTargetWithHediff.NullOrEmpty() && canTargetWithHediff.Any(def => pawn.health.hediffSet.HasHediff(def)))
                    {
                        return true;
                    }

                    if(canTargetWithMentalState || !canTargetWithHediff.NullOrEmpty())
                    {
                        return false;
                    }
                }

                return true;
            };
        }
    }
}
