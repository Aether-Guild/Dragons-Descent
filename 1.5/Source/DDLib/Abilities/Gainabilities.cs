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
using AbilityDef = VFECore.Abilities.AbilityDef;

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
    //            foreach (VFECore.Abilities.AbilityDef abilityDef in new VFECore.Abilities.AbilityDef[]
    //            {
    //                // Add the ability definitions you want to give here
    //                VFECore.Abilities.AbilityDef.MyFirstAbility,
    //                VFECore.Abilities.AbilityDef.MySecondAbility,
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