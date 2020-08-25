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
    [HarmonyPatch(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
    public static class DD_PawnComponentsUtility_AddAndRemoveDynamicComponents
    {
        public static void Postfix(Pawn pawn)
        {
            if (pawn.ownership == null && pawn.def.HasModExtension<LegacyModExtension>())
            {
                //All flagged pawns capable of owning things. (e.g: Beds)
                if (pawn.def.GetModExtension<LegacyModExtension>().hasOwnership)
                {
                    pawn.ownership = new Pawn_Ownership(pawn);
                }
            }
        }
    }
}
