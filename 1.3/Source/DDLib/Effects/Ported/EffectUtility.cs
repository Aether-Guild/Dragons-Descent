using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using VFECore;

namespace DD
{
    public static class EffectUtility
    {
        public static void TryExtinguishEffect(this Pawn_NativeVerbs nverbs, AbstractEffect effect)
        {

        }

        public static bool CanApplyEffect(Thing target)
        {
            if (target == null)
            {
                return false;
            }
            if (target.Destroyed)
            {
                return false;
            }
            if (target.TryGetComp<CompAttachBase>() == null)
            {
                return false;
            }
            return true;
        }

        public static bool ApplyEffect(Thing target, ThingDef def, float value = -1f)
        {
            AbstractEffect effect = target as AbstractEffect;

            if (target is AbstractEffect)
            {
                //Is effect
                    if (effect.CanInteractWith(def))
                {
                    effect.MergeWith(def, value);
                }
                else
                {
                    //Doesn't interact with the effect, so we stack them.
                        effect = (AbstractEffect)ThingMaker.MakeThing(def);
                    if (value >= 0)
                    {
                        effect.EffectSize = value;
                    }
                    GenSpawn.Spawn(effect, target.Position, target.Map, Rot4.North);
                }
            }
            else
            {
                //Is a thing.
                    if (CanApplyEffect(target) && target.def.category == ThingCategory.Pawn)
                {
                    //Can be attached and is a pawn
                        if (target.HasAttachment(def))
                    {
                        //Already has this effect attached.
                            effect = (AbstractEffect)target.GetAttachment(def);
                        effect.EffectSize = Mathf.Max(effect.EffectSize, value); //Update the size to a the larger one.
                    }
                    else
                    {
                        //Attach a new effect to the pawn.
                        effect = (AbstractEffect)ThingMaker.MakeThing(def);
                        if (value >= 0)
                        {
                            effect.EffectSize = value;
                        }
                        effect.AttachTo(target);
                        GenSpawn.Spawn(effect, target.Position, target.Map, Rot4.North);
                    }
                }
            }

            return effect != null;
        }

        public static bool ApplyEffect(IntVec3 cell, Map map, ThingDef def, float value = -1)
        {
            IEnumerable<AbstractEffect> effects = cell.GetThingList(map).ToList().OfType<AbstractEffect>();
            bool processed = false;

            if (!effects.EnumerableNullOrEmpty())
            {
                foreach (AbstractEffect effect in effects)
                {
                    if (effect.CanInteractWith(def))
                    {
                        effect.MergeWith(def, value);
                        processed = true;
                    }
                }
            }

            if (!processed)
            {
                AbstractEffect effect = (AbstractEffect)ThingMaker.MakeThing(def);
                if (value >= 0)
                {
                    effect.EffectSize = value;
                }
                GenSpawn.Spawn(effect, cell, map, Rot4.North);
            }

            return true;
        }
    }
}
