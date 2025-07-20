using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using VFECore;
using VFECore.Abilities;
using RimWorld.Planet;
using Ability = VFECore.Abilities.Ability;

namespace DD
{
    
    //[HarmonyPatch(typeof(PawnGenerator), "GenerateNewPawnInternal")]
    //public class PawnGen_Patch
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(Pawn __result)
    //    {
    //        if (__result != null)
    //        {
    //            PawnKindAbilityExtension abilityExtension = __result.kindDef?.GetModExtension<PawnKindAbilityExtension>();

    //            if (abilityExtension != null)
    //            {
    //                int age = __result.ageTracker;
                
    //                // Define the age ranges for each ability
    //                int dragonJumpAgeRangeStart = 5;
    //                int dragonBreathFireAgeRangeEnd = 20;
    //                int dragonSpitFireAgeRangeEnd = 40;

    //                if (age >= dragonJumpAgeRangeStart && age <= 50)
    //                {
    //                    // Give the DD_DragonJump ability
    //                    CompAbilities comp = __result.GetComp<CompAbilities>();
    //                    if (comp != null) comp.GiveAbility(VFECore.Abilities.DD_DragonJump);
    //                }

    //                if (age >= dragonBreathFireAgeRangeStart && age <= dragonBreathFireAgeRangeEnd)
    //                {
    //                    // Give the DD_DragonBreath_Fire ability
    //                    CompAbilities comp = __result.GetComp<CompAbilities>();
    //                    if (comp != null) comp.GiveAbility(VFECore.Abilities.DD_DragonBreath_Fire);
    //                }

    //                if (age >= dragonSpitFireAgeRangeStart && age <= dragonSpitFireAgeRangeEnd)
    //                {
    //                    // Give the DD_DragonSpit_Fire ability
    //                    CompAbilities comp = __result.GetComp<CompAbilities>();
    //                    if (comp != null) comp.GiveAbility(VFECore.Abilities.DD_DragonSpit_Fire);
    //                }
    //            }
    //        }
    //    }
    //}
}
