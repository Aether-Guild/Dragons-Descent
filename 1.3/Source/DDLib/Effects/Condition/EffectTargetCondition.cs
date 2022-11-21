using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectTargetCondition : EffectCondition
    {
        public EffectTargets targets;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing thing)
        {
            if (thing is Pawn && targets.HasFlag(EffectTargets.Pawns))
            {
                return true;
            }

            if (thing is Corpse && targets.HasFlag(EffectTargets.Corpse))
            {
                return true;
            }

            if (thing is Plant && targets.HasFlag(EffectTargets.Plants))
            {
                return true;
            }

            if (thing is Building && targets.HasFlag(EffectTargets.Buildings))
            {
                return true;
            }

            if (thing != null && targets == EffectTargets.Things)
            {
                return true;
            }

            //Not one of the above and matching all 'Things', then just check for things.
            return false;
        }

        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell)
        {
            return targets.HasFlag(EffectTargets.Terrain);
        }
    }
}
