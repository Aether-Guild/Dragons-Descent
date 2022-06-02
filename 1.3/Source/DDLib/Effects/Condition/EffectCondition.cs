using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class EffectCondition
    {
        public virtual bool IsSatisfied(EffectInfo effect, TargetInfo target)
        {
            if (target.HasThing)
            {
                //Is thing
                if (target.ThingDestroyed)
                {
                    //Is destroyed.
                    return false;
                }

                if (target.Thing.TryGetComp<CompAttachBase>() == null)
                {
                    //Can't attach.
                    return false;
                }

                return ConditionIsSatisfied(effect, target.Thing);
            }
            else if (target.Cell.IsValid && target.Map != null)
            {
                //Is cell
                List<Thing> things = target.Cell.GetThingList(target.Map);
                if (!things.NullOrEmpty())
                {
                    //Check for things to attach to.
                    return things.Any(thing => IsSatisfied(effect, thing));
                }
                else
                {
                    //Nothing to attach to.
                    return ConditionIsSatisfied(effect, target.Map, target.Cell);
                }
            }

            return false;
        }

        public virtual IEnumerable<TargetInfo> PickTarget(EffectInfo effect, TargetInfo target)
        {
            if (target.HasThing)
            {
                //Is thing
                if (target.ThingDestroyed)
                {
                    //Is destroyed.
                    yield break;
                }

                if (target.Thing.TryGetComp<CompAttachBase>() == null)
                {
                    //Can't attach.
                    yield break;
                }

                if (ConditionIsSatisfied(effect, target.Thing))
                {
                    //Applicable.
                    yield return target.Thing;
                }
            }
            else if (target.Cell.IsValid && target.Map != null)
            {
                //Is cell
                List<Thing> things = target.Cell.GetThingList(target.Map);
                if (!things.NullOrEmpty())
                {
                    //Check for things to attach to.
                    TargetInfo t = things.Where(thing => IsSatisfied(effect, thing)).OrderBy(thing => thing.def.category).FirstOrFallback();
                    if(t.IsValid)
                    {
                        //Selected a thing
                        yield return t;
                    }
                }

                if (ConditionIsSatisfied(effect, target.Map, target.Cell))
                {
                    yield return target;
                }
            }

            yield break;
        }

        public abstract bool ConditionIsSatisfied(EffectInfo effect, Thing thing);
        public abstract bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell);
    }
}
