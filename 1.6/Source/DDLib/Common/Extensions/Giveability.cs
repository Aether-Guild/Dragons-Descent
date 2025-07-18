//using System;
//using System.Collections.Generic;
//using System.Linq;
//using HarmonyLib;
//using RimWorld;
//using UnityEngine;
//using Verse;
//using RimWorld.Planet;
//using VEF.Abilities;
//using Ability = VEF.Abilities.Ability;

//namespace DD
//{
//    [HarmonyPatch(typeof(PawnKindDef), "Pawn")]
//    public class PawnKindDef_ctor_Patch
//    {
//        [HarmonyPostfix]
//        public static void Postfix(PawnKindDef __instance)
//        {
//            if (__instance.defName == "Black_Dragon")
//            {
//                var abilityExtension = __instance.GetModExtension<VEF.Abilities.PawnKindAbilityExtension>();
//                if (abilityExtension != null && !abilityExtension.giveAbilities.Any(a => a == VEF.Abilities.Ability.DD_DragonJump))
//                {
//                    abilityExtension.giveAbilities.Add(VEF.Abilities.Ability.DD_DragonJump);
//                    abilityExtension.giveAbilities.Add(VEF.Abilities.Ability.DD_DragonBreath_Fire);
//                    abilityExtension.giveAbilities.Add(VEF.Abilities.Ability.DD_DragonSpit_Fire);
//                }
//            }
//        }
//    }
//}