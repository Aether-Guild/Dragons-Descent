//using System;
//using System.Collections.Generic;
//using System.Linq;
//using HarmonyLib;
//using RimWorld;
//using UnityEngine;
//using Verse;
//using RimWorld.Planet;
//using VFECore.Abilities;
//using Ability = VFECore.Abilities.Ability;

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
//                var abilityExtension = __instance.GetModExtension<VFECore.Abilities.PawnKindAbilityExtension>();
//                if (abilityExtension != null && !abilityExtension.giveAbilities.Any(a => a == VFECore.Abilities.Ability.DD_DragonJump))
//                {
//                    abilityExtension.giveAbilities.Add(VFECore.Abilities.Ability.DD_DragonJump);
//                    abilityExtension.giveAbilities.Add(VFECore.Abilities.Ability.DD_DragonBreath_Fire);
//                    abilityExtension.giveAbilities.Add(VFECore.Abilities.Ability.DD_DragonSpit_Fire);
//                }
//            }
//        }
//    }
//}