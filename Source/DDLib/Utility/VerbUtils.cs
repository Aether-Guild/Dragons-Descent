using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using System.Linq;

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

            return verbs;
        }

        public static IEnumerable<Verb> GetPossibleVerbs(Pawn pawn)
        {
            //Filter the verbs to keep the ones that -can- be selected. (Zero chance of triggering will be not included.)
            return GetVerbs(pawn).Where(verb => verb.Available());
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

        public static IEnumerable<Verb> Filter_KeepOffensive(this IEnumerable<Verb> verbs)
        {
            //Filter the sequence to keep verbs that are tagged as 'harms health'.
            return verbs.Where((verb) => verb.HarmsHealth());
        }

        public static IEnumerable<Verb> Filter_KeepInRange(this IEnumerable<Verb> verbs, Thing target)
        {
            //Filter the sequence to keep only verbs that can hit the target.
            return verbs.Where((verb) => verb.CanHitTarget(target));
        }

        public static IEnumerable<Verb> Sort_OrderByPreference(this IEnumerable<Verb> verbs)
        {
            //Sort the verb list by preference.
            return verbs.OrderByDescending(verb => CalculatePreference(verb));
        }

        public static Verb Get_MostPreferred(this IEnumerable<Verb> verbs)
        {
            //Select the verb with the maximum preference.
            return verbs.MaxBy(verb => CalculatePreference(verb));
        }

        private static double CalculatePreference(Verb verb)
        {
            VerbProperties props = verb.verbProps;
            DamageDef dd = verb.GetDamageDef();

            double preferenceValue = dd.defaultDamage; //Start with base damage

            preferenceValue += dd.defaultDamage * dd.defaultStoppingPower; //Prefer higher stopping power
            preferenceValue += dd.defaultDamage * dd.defaultArmorPenetration; //Prefer higher armor penetration

            if (dd.additionalHediffs != null)
            {
                preferenceValue += dd.defaultDamage * dd.additionalHediffs.Where(hddd => hddd.hediff.isBad).Sum(hddd => hddd.severityPerDamageDealt); //Prefer hediff causing attacks
            }
            
            if(dd.isRanged)
            {
                preferenceValue += dd.defaultDamage; //Prefer ranged
            }

            if(dd.isExplosive)
            {
                preferenceValue += dd.defaultDamage; //Prefer explosions
            }

            preferenceValue += preferenceValue * props.commonality; //Adjust by commonality //P.S: SCALING BY COMMONALITY IS A BAD IDEA.

            return preferenceValue;
        }
    }

}