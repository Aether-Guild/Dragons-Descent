using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace DD
{

    public static class VerbUtils
    {
        public static IEnumerable<Verb> GetVerbs(Pawn pawn)
        {
            //Create a sequence of all verbs.
            IEnumerable<Verb> verbs = new List<Verb>();

            if (pawn.equipment != null && pawn.equipment.AllEquipmentVerbs != null)
            {
                //If pawn has equipments, include the equipment verbs.
                verbs = verbs.Concat(pawn.equipment.AllEquipmentVerbs);
            }

            if (pawn.VerbTracker != null && pawn.VerbTracker.AllVerbs != null)
            {
                //If the pawn has verbs, include them.
                verbs = verbs.Concat(pawn.VerbTracker.AllVerbs);
            }

            if (pawn.abilities != null && pawn.abilities.abilities != null)
            {
                //Add it if it has a comp that states that the comp can be used automatically.
                foreach (VerbTracker tracker in pawn.abilities.abilities.OfType<Ability_Base>().Where(ability => ability.CanAutoCast).Select(a => a.VerbTracker))
                {
                    if (tracker != null && tracker.AllVerbs.Any())
                    {
                        verbs = verbs.Concat(tracker.AllVerbs);
                    }
                }
            }

            return verbs;
        }

        public static bool VerbIsControlledByAbilityBase(this Verb verb, out Ability_Base ability)
        {
            ability = verb?.verbTracker?.directOwner as Ability_Base;
            if (ability != null)
            {
                return true;
            }
            return false;
        }
        public static IEnumerable<Verb> GetPossibleVerbs(Pawn pawn)
        {
            //Filter the verbs to keep the ones that -can- be selected.
            return GetVerbs(pawn).Where(verb => verb.Available());
        }

        public static IEnumerable<Verb> GetPossibleAbilityVerbs(Pawn pawn)
        {
            IEnumerable<Verb> verbs = new List<Verb>();
            if (pawn.abilities != null && pawn.abilities.abilities != null)
            {
                //Add it if it has a comp that states that the comp can be used automatically.
                foreach (VerbTracker tracker in pawn.abilities.abilities.OfType<Ability_Base>().Where(ability => ability.CanAutoCast).Select(a => a.VerbTracker))
                {
                    if (tracker != null && tracker.AllVerbs.Any())
                    {
                        verbs = verbs.Concat(tracker.AllVerbs);
                    }
                }
            }

            //Filter the verbs to keep the ones that -can- be selected.
            return GetVerbs(pawn).Where(verb => verb.Available());
        }

        public static IEnumerable<Verb> Ability_ExpandVerbSelection(this Verb verb)
        {
            //Expand selection to verbs contained in Ability_Bases and remove trigger abilities.
            if (verb.VerbIsControlledByAbilityBase(out var ability))
            {
                return ability.Verbs;
            }

            return new Verb[0];
        }

        public static IEnumerable<Verb> Filter_KeepMelee(this IEnumerable<Verb> verbs)
        {
            //Filter the sequence to keep melee verbs.
            return verbs.Where(verb => verb.IsMeleeAttack);
        }

        public static IEnumerable<Verb> Filter_KeepRanged(this IEnumerable<Verb> verbs)
        {
            //Filter the sequence to keep ranged verbs.
            return verbs.Where((verb) => !verb.IsMeleeAttack);
        }

        public static IEnumerable<Verb> Filter_ExcludeAbilityVerbs(this IEnumerable<Verb> verbs)
        {
            //Filter the sequence to exclude ability verbs.
            return verbs.Where(verb => !(verb.VerbIsControlledByAbilityBase(out _)));
        }

        //public static IEnumerable<Verb> Filter_KeepOffensive(this IEnumerable<Verb> verbs)
        //{
        //    //Filter the sequence to keep verbs that are tagged as 'harms health'.
        //    return verbs.Where((verb) => verb.HarmsHealth());
        //}

        public static IEnumerable<Verb> Filter_KeepInRange(this IEnumerable<Verb> verbs, Thing target)
        {
            //Filter the sequence to keep only verbs that can hit the target.
            return verbs.Where(verb =>
            {

                bool b = verb.CanHitTarget(target);
                return b;
            });
        }

        public static IEnumerable<Verb> Sort_OrderByPreference(this IEnumerable<Verb> verbs)
        {
            //Sort the verb list by preference.
            //return verbs.OrderByDescending(verb => CalculatePreference(verb));
            return verbs.OrderByDescending(verb => verb.verbProps.commonality);
        }


        public static Verb Get_MostPreferred(this IEnumerable<Verb> verbs, bool primary)
        {
            //Select the verb with the maximum preference.
            //return verbs.RandomElementByWeightWithFallback(verb => CalculatePreference(verb));
            if (primary)
            {
                return verbs.MaxByWithFallback(verb => Mathf.Pow(verb.verbProps.range - verb.verbProps.EffectiveMinRange(true), 2));
            }
            else
            {
                return verbs.RandomElementByWeightWithFallback(verb => verb.verbProps.commonality);
            }
        }

        public static float CalculateCommonality(this Verb verb, Pawn source, Thing target)
        {
            if (source.IsAdjacentToCardinalOrInside(target))
            {
                return verb.verbProps.commonality;
            }
            else
            {
                return verb.verbProps.commonality + verb.verbProps.EffectiveMinRange(target, source) + verb.verbProps.range;
            }
        }

        //private static float CalculatePreference(Verb verb)
        //{
        //    VerbProperties props = verb.verbProps;
        //    DamageDef dd = verb.GetDamageDef();

        //    float preferenceValue = dd.defaultDamage; //Start with base damage

        //    preferenceValue += dd.defaultDamage * dd.defaultStoppingPower; //Prefer higher stopping power
        //    preferenceValue += dd.defaultDamage * dd.defaultArmorPenetration; //Prefer higher armor penetration

        //    if (dd.additionalHediffs != null)
        //    {
        //        preferenceValue += dd.defaultDamage * dd.additionalHediffs.Where(hddd => hddd.hediff.isBad).Sum(hddd => hddd.severityPerDamageDealt); //Prefer hediff causing attacks
        //    }

        //    if(dd.isRanged)
        //    {
        //        preferenceValue += dd.defaultDamage*(props.minRange - props.range); //Prefer ranged
        //    }

        //    if(dd.isExplosive)
        //    {
        //        preferenceValue += dd.defaultDamage; //Prefer explosions
        //    }

        //    preferenceValue = Mathf.Abs(preferenceValue * props.commonality); //Scale by commonality

        //    return preferenceValue;
        //}
    }

}