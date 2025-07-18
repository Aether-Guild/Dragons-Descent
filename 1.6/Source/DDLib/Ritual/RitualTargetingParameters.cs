using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class RitualTargetingParameters : TargetingParameters
    {
        public RitualTarget ritualTarget = RitualTarget.NoTarget;

        public bool mustBeRitualTarget = true;

        public bool onlyTargetFactionless;

        public bool canTargetWithMentalState;

        public List<HediffDef> canTargetWithHediff;

        public bool mustHaveMap, mustBeEmpty;
        public List<WorldObjectDef> targetWorldObjects;

        public FloatRange? tileRange;

        public bool canTargetOwnTile = true, canTargetPlayerFaction = true;

        public virtual bool CanTargetGlobal(Thing source, GlobalTargetInfo target)
        {
            if(!target.IsValid)
            {
                return false;
            }

            if(!canTargetOwnTile && target.Tile == source.Tile)
            {
                return false;
            }

            if (!InRange(source, target))
            {
                return false;
            }

            if (!canTargetPlayerFaction)
            {
                if (target.HasWorldObject && target.WorldObject.Faction != null && target.WorldObject.Faction.IsPlayer)
                {
                    return false;
                }

                if(target.HasThing && target.Thing.Faction != null && target.Thing.Faction.IsPlayer)
                {
                    return false;
                }
            }

            if (mustBeEmpty)
            {
                if(target.HasWorldObject || target.HasThing)
                {
                    return false;
                }
            }

            if(mustHaveMap)
            {
                if (target.HasWorldObject && target.WorldObject is MapParent mp)
                {
                    if (mp.HasMap)
                    {
                        return false;
                    }
                }
            }
            
            if (!targetWorldObjects.NullOrEmpty())
            {
                return target.HasWorldObject && targetWorldObjects.Any(def => def == target.WorldObject.def);
            }

            return true;
        }

        protected virtual bool CanAlsoTarget(TargetInfo target)
        {
            if (target == null)
            {
                return false;
            }

            if(target.HasThing)
            {
                if(target.ThingDestroyed)
                {
                    return false;
                }

                if (mustBeRitualTarget && !target.Thing.def.HasModExtension<RitualTargetExtension>())
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

                    if (canTargetWithMentalState || !canTargetWithHediff.NullOrEmpty())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public RitualTargetingParameters()
        {
            validator = CanAlsoTarget;
        }

        public bool InRange(Thing source, GlobalTargetInfo target)
        {
            if (!target.IsValid)
            {
                return false;
            }

            if(target.Tile <= 0)
            {
                return false;
            }

            if (tileRange.HasValue)
            {
                float tiles = Mathf.Round(Find.WorldGrid.ApproxDistanceInTiles(source.Tile, target.Tile));

                if(tileRange.Value.Span == 0)
                {
                    if(tiles >= tileRange.Value.TrueMax)
                    {
                        return false;
                    }
                } else
                {
                    if (!tileRange.Value.Includes(tiles))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
