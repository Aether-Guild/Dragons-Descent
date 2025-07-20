using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using VEF;
using VEF.Abilities;
using RimWorld.Planet;
using Ability = VEF.Abilities.Ability;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace DD
{
    //[HarmonyPatch(typeof(LearnedAbility), "Tick")]
    //public class GiveAbilitiesAfter500TicksPatch
    //{
    //    private static int tickCount = 0;

    //    public static void Postfix(LearnedAbility __instance)
    //    {
    //        tickCount++;
    //        if (tickCount >= 500)
    //        {
    //            tickCount = 0;
    //            foreach (VEF.Abilities.AbilityDef abilityDef in new VEF.Abilities.AbilityDef[]
    //            {
    //                // Add the ability definitions you want to give here
    //                VEF.Abilities.AbilityDef.MyFirstAbility,
    //                VEF.Abilities.AbilityDef.MySecondAbility,
    //                // ...
    //            })
    //            {
    //                CompAbilities comp = __instance.GetComp<CompAbilities>();
    //                if (comp != null)
    //                {
    //                    comp.GiveAbility(abilityDef);
    //                }
    //            }
    //        }
    //    }
    //}
}