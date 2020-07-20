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

namespace DD
{
    [StaticConstructorOnStartup]
    public static class Patcher
    {
        private static readonly string HarmonyPatchID = "com.rimworld.mod.dd";

        static Patcher()
        {
            Harmony harmony = new Harmony(HarmonyPatchID);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(TameUtility), "CanTame")]
        public static class DD_TameUtility_CanTame
        {
            public static void Postfix(Pawn pawn, ref bool __result)
            {
                __result = __result && pawn.IsTameable(); //Taming can be disabled.
            }
        }

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

        [HarmonyPatch(typeof(Designator_Hunt), "ShowDesignationWarnings")]
        public static class DD_Designator_Hunt_ShowDesignationWarnings
        {
            public static bool Prefix(Pawn pawn)
            {
                if(pawn.IsEnraged())
                {
                    //Skip messaging vanilla message if pawn is enraged.
                    float percent = 1;

                    Messages.Message("MessageAnimalsGoPsychoHunted".Translate(pawn.kindDef.GetLabelPlural().CapitalizeFirst(), percent.ToStringPercent(), pawn.Named("ANIMAL")).CapitalizeFirst(), pawn, MessageTypeDefOf.CautionInput, historical: false);

                    return false;
                }
                return true;
            }
        }

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
}
