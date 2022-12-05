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
            if (!pawn.def.HasModExtension<LegacyModExtension>())
            {
                return;
            }

            LegacyModExtension ext = pawn.def.GetModExtension<LegacyModExtension>();

            if (ext.hasOwnership)
            {
                //Flagged to have the ability to own things (Beds, etc.)
                if (pawn.ownership == null)
                {
                    pawn.ownership = new Pawn_Ownership(pawn);
                }
            }

            if (ext.hasAbilities)
            {
                //Flagged to have abilities.
                if (pawn.abilities == null)
                {
                    pawn.abilities = new Pawn_AbilityTracker(pawn);
                }
            }
        }
    }
}
