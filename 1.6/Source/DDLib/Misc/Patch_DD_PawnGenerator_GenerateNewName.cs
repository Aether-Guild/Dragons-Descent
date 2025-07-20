using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateNewPawnInternal")]
    public static class Patch_DD_PawnGenerator_GenerateNewPawnInternal
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __result, ref PawnGenerationRequest request)
        {
            if (__result == null)
                return;
            if (DraconicOverseer.Settings.ShouldSpawnNamed(__result))
                __result.Name = PawnBioAndNameGenerator.GeneratePawnName(__result, NameStyle.Full);
        }
    }
}
