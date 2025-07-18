//using System;
//using System.Collections.Generic;
//using RimWorld;
//using Verse;
//using Verse.AI;
//using System.Reflection;
//using System.Linq;
//using UnityEngine;

//namespace DD
//{

//    public static class VerbUtils
//    {
//        public static IEnumerable<Verb> GetVerbs(Pawn pawn)
//        {
//            //Create a sequence of all verbs.   IEnumerable<Verb> verbs = new List<Verb>();

//            if (pawn.equipment != null && pawn.equipment.AllEquipmentVerbs != null)
//            {

//                verbs = verbs.Concat(pawn.equipment.AllEquipmentVerbs);
//            }

//            if (pawn.VerbTracker != null && pawn.VerbTracker.AllVerbs != null)
//            {

//                verbs = verbs.Concat(pawn.VerbTracker.AllVerbs);
//            }

//            if (pawn.abilities != null && pawn.abilities.abilities != null)
//            {

//                foreach (VerbTracker tracker in pawn.abilities.abilities.OfType<Ability_Base>().Where(ability => ability.CanAutoCast).Select(a => a.VerbTracker))
//                {
//                    if (tracker != null && tracker.AllVerbs.Any())
//                    {
//                        verbs = verbs.Concat(tracker.AllVerbs);
//                    }
//                }
//            }

//            return verbs;
//        }

//        public static bool VerbIsControlledByAbilityBase(this Verb verb, out Ability_Base ability)
//        {
//            ability = verb?.verbTracker?.directOwner as Ability_Base;
//            if (ability != null)
//            {
//                return true;
//            }
//            return false;
//        }
//        public static IEnumerable<Verb> GetPossibleVerbs(Pawn pawn)
//        {
//            return GetVerbs(pawn).Where(verb => verb.Available());
//        }

//        public static IEnumerable<Verb> GetPossibleAbilityVerbs(Pawn pawn)
//        {
//            IEnumerable<Verb> verbs = new List<Verb>();
//            if (pawn.abilities != null && pawn.abilities.abilities != null)
//            {

//                foreach (VerbTracker tracker in pawn.abilities.abilities.OfType<Ability_Base>().Where(ability => ability.CanAutoCast).Select(a => a.VerbTracker))
//                {
//                    if (tracker != null && tracker.AllVerbs.Any())
//                    {
//                        verbs = verbs.Concat(tracker.AllVerbs);
//                    }
//                }
//            }


//            return GetVerbs(pawn).Where(verb => verb.Available());
//        }

//        public static IEnumerable<Verb> Ability_ExpandVerbSelection(this Verb verb)
//        {
//            if (verb.VerbIsControlledByAbilityBase(out var ability))
//            {
//                return ability.Verbs;
//            }

//            return new Verb[0];
//        }

//        public static IEnumerable<Verb> Filter_KeepMelee(this IEnumerable<Verb> verbs)
//        {

//            return verbs.Where(verb => verb.IsMeleeAttack);
//        }

//        public static IEnumerable<Verb> Filter_KeepRanged(this IEnumerable<Verb> verbs)
     
//            return verbs.Where((verb) => !verb.IsMeleeAttack);
//        }

//    public static IEnumerable<Verb> Filter_ExcludeAbilityVerbs(this IEnumerable<Verb> verbs)
//    {

//        return verbs.Where(verb => !(verb.VerbIsControlledByAbilityBase(out _)));
//    }



//    public static IEnumerable<Verb> Filter_KeepInRange(this IEnumerable<Verb> verbs, Thing target)
//    {

//        return verbs.Where(verb =>
//        {

//            bool b = verb.CanHitTarget(target);
//            return b;
//        });
//    }

//    public static IEnumerable<Verb> Sort_OrderByPreference(this IEnumerable<Verb> verbs)
//    {

//        return verbs.OrderByDescending(verb => verb.verbProps.commonality);
//    }


//    public static Verb Get_MostPreferred(this IEnumerable<Verb> verbs, bool primary)
//    {

//        if (primary)
//        {
//            return verbs.MaxByWithFallback(verb => Mathf.Pow(verb.verbProps.range - verb.verbProps.EffectiveMinRange(true), 2));
//        }
//        else
//        {
//            return verbs.RandomElementByWeightWithFallback(verb => verb.verbProps.commonality);
//        }
//    }

//    public static float CalculateCommonality(this Verb verb, Pawn source, Thing target)
//    {
//        if (source.IsAdjacentToCardinalOrInside(target))
//        {
//            return verb.verbProps.commonality;
//        }
//        else
//        {
//            return verb.verbProps.commonality + verb.verbProps.EffectiveMinRange(target, source) + verb.verbProps.range;
//        }
//    }


//}

//}