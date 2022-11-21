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
    [HarmonyPatch(typeof(Alert_HypothermicAnimals), "get_HypothermicAnimals")]
    public static class DD_Alert_HypothermicAnimals_get_HypothermicAnimals
    {
        public static void Postfix(ref List<Pawn> __result)
        {
            //List contains pawns (animals) with the hypothermia hediff.
            //Removing the pawns that have the hypothermia hediff, but aren't in an area that's cold enough to justify it.
            //This should only affect the (debug) message that shows up.
            __result.RemoveAll(pawn => pawn.AmbientTemperature > pawn.SafeTemperatureRange().min);
        }
    }
}
