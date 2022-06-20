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
    [HarmonyPatch(typeof(TameUtility), "CanTame")]
    public static class DD_TameUtility_CanTame
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            __result = __result && pawn.IsTameable(); //Taming can be disabled.
        }
    }
}
