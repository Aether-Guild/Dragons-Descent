using System;
using Verse;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;

namespace DD
{
    public static class EffectUtils
    {
        //Spawn
        public static Effect MakeEffect(EffectDef def, float size, Thing thing = null)
        {
            Effect effect = (Effect)ThingMaker.MakeThing(def.thingDef);
            effect.effectDef = def;
            effect.size = size;
            if (thing != null)
            {
                effect.AttachTo(thing);
            }
            return effect;
        }

        //Getters
        public static float DamageToSize(EffectDef def, float damage) => def.damagePerSize.EvaluateInverted(damage);
        public static float SizeToDamage(EffectDef def, float size) => def.damagePerSize.Evaluate(size);

        public static bool TryAffect(EffectInfo einfo, TargetInfo target)
        {
            if (einfo.Def.affectCondition == null)
            {
                return false;
            }

            if (einfo.Def.affectCondition.IsSatisfied(einfo, target))
            {
                List<TargetInfo> targets = einfo.Def.affectCondition.PickTarget(einfo, target).ToList();
                bool result = false;
                foreach (TargetInfo t in targets.Where(targ => targ.IsValid))
                {
                    result = result || ApplyEffect(t, einfo.Def, einfo.Size / targets.Count);
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        public static bool CanDamage(EffectInfo einfo, Thing target)
        {
            return einfo.Def.damageCondition != null && einfo.Def.damageCondition.IsSatisfied(einfo, target);
        }

        public static bool TrySpreadTo(EffectInfo einfo, TargetInfo target)
        {
            if (einfo.Def.spreadCondition == null)
            {
                return false;
            }

            if (einfo.Def.spreadCondition.IsSatisfied(einfo, target))
            {
                if (einfo.Def.spreadCondition.PickTarget(einfo, target).Where(targ => targ.IsValid && targ.HasThing && einfo.Effect != targ.Thing).TryRandomElement(out TargetInfo t))
                {
                    return ApplyEffect(t, einfo.Def, einfo.Size);
                }
            }
            return false;
        }

        public static bool ApplyEffect(TargetInfo target, EffectDef def, float size)
        {
            if (target.IsValid)
            {
                if (target.HasThing && !target.ThingDestroyed)
                {
                    if (HasInteractableEffects(def, size, target.Thing))
                    {
                        return InteractEffects(def, size, target.Thing);
                    }
                    else
                    {
                        return AttachEffect(def, size, target.Thing);
                    }
                }
                else if (target.Map != null && target.Cell.IsValid)
                {
                    return SpawnEffect(def, size, target.Map, target.Cell);
                }
            }
            return false;
        }

        private static bool SpawnEffect(EffectDef def, float size, Map map, IntVec3 cell)
        {
            if (cell.GetFirstThing(map, def.thingDef) is Effect effect)
            {
                effect.AddSize(size);
                return true;
            }
            else
            {
                return GenSpawn.Spawn(MakeEffect(def, size), cell, map, Rot4.North) != null;
            }
        }

        private static bool AttachEffect(EffectDef def, float size, Thing thing)
        {
            if (thing.GetAttachment(def.thingDef) is Effect effect)
            {
                effect.AddSize(size);
                return true;
            }
            else
            {
                return GenSpawn.Spawn(MakeEffect(def, size, thing), thing.Position, thing.Map, Rot4.North) != null;
            }
        }

        private static bool HasInteractableEffects(EffectDef effectDef, float size, Thing target)
        {
            IEnumerable<EffectDef> defs = new List<EffectDef>();
            if (!effectDef.boostedBy.NullOrEmpty())
            {
                defs = defs.Concat(effectDef.boostedBy);
            }
            if (!effectDef.reducedBy.NullOrEmpty())
            {
                defs = defs.Concat(effectDef.reducedBy);
            }

            return defs.Any(def => target.GetAttachment(def.thingDef) != null);
        }

        private static bool InteractEffects(EffectDef effectDef, float size, Thing target)
        {
            float remainingSize = size;
            IEnumerable<EffectDef> defs = new List<EffectDef>();
            if (!effectDef.boostedBy.NullOrEmpty())
            {
                defs = defs.Concat(effectDef.boostedBy);
            }
            if (!effectDef.reducedBy.NullOrEmpty())
            {
                defs = defs.Concat(effectDef.reducedBy);
            }

            int count = defs.Count(def => target.GetAttachment(def.thingDef) != null);

            if (count > 0)
            {
                remainingSize = 0;
                float dividedSize = size / count;

                if (!effectDef.boostedBy.NullOrEmpty())
                {
                    foreach (Effect effect in effectDef.boostedBy.Select(Def => target.GetAttachment(effectDef.thingDef)).OfType<Effect>())
                    {
                        remainingSize += effect.AddSize(size);
                    }
                }

                if (!effectDef.reducedBy.NullOrEmpty())
                {
                    foreach (Effect effect in effectDef.reducedBy.Select(Def => target.GetAttachment(effectDef.thingDef)).OfType<Effect>())
                    {
                        remainingSize += -effect.AddSize(-size);
                    }
                }
            }

            if (remainingSize > 0)
            {
                return AttachEffect(effectDef, remainingSize, target);
            }
            else
            {
                return true;
            }
        }
    }
}
