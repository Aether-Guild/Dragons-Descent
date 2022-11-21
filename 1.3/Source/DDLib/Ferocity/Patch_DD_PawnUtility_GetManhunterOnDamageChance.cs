using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    [HarmonyPatch(typeof(PawnUtility), "GetManhunterOnDamageChance", new Type[] { typeof(Pawn), typeof(float), typeof(Thing) })]
    public static class DD_PawnUtility_GetManhunterOnDamageChance
    {
        public static void Postfix(Pawn pawn, float distance, Thing instigator, ref float __result)
        {
            if (pawn.IsEnraged())
            {
                __result = 1f; //Guaranteed to manhunt on damage if its enraged.
            }
        }
    }
}
