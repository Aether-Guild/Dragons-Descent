using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace DD
{
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "CanInteractNowWith")]
    public static class Patch_DD_Pawn_InteractionsTracker_CanInteractNowWith
    {
        public static void Postfix(ref bool __result, Pawn ___pawn, Pawn recipient, InteractionDef interactionDef)
        {
            if (!__result) {
                return;
            }
            if (___pawn != null && recipient != null && ___pawn.IsAnimal && recipient.IsAnimal && interactionDef == InteractionDefOf.AnimalChat && ___pawn.kindDef?.defName != null && ___pawn.kindDef.defName.EndsWith("_Dragon")) {
                __result = false;
                return;
            }
        }
    }
}
