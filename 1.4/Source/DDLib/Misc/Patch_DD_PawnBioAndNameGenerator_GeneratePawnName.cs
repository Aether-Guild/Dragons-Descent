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
    [HarmonyPatch(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GeneratePawnName))]
    public static class Patch_DD_PawnBioAndNameGenerator_GeneratePawnName
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn pawn, ref NameStyle style)
        {
            if (style == NameStyle.Full)
                return;
            if (pawn == null)
                return;
            if (DraconicOverseer.Settings.ShouldSpawnNamed(pawn))
                style = NameStyle.Full;
        }
    }
}
