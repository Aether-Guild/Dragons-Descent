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
                if (pawn.IsEnraged())
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

        [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
        public static class DD_RestUtility_IsValidBedFor
        {
            public static void Postfix(ref bool __result, Thing bedThing, Pawn sleeper, Pawn traveler, bool sleeperWillBePrisoner, bool checkSocialProperness, bool allowMedBedEvenIfSetToNoCare = false, bool ignoreOtherReservations = false)
            {
                //If is valid bed, doublecheck who can be assigned to the bed, if you are able to assign pawns to it.
                if (__result)
                {
                    CompAssignableToPawn compAssign = bedThing.TryGetComp<CompAssignableToPawn>();
                    if (compAssign != null)
                    {
                        __result = compAssign.CanAssignTo(sleeper).Accepted;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Thing), "SpawnSetup")]
        public static class DD_Thing_SpawnSetup
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> raw, ILGenerator generator)
            {
                FieldInfo stackCountField = AccessTools.Field(typeof(Thing), "stackCount");
                FieldInfo thingDefField = AccessTools.Field(typeof(Thing), "def");
                FieldInfo stackLimitField = AccessTools.Field(typeof(ThingDef), "stackLimit");
                MethodInfo hasModDef = AccessTools.Method(typeof(ThingDef), "HasModExtension", null, new Type[] { typeof(LegacyModExtension) });
                MethodInfo getModDef = AccessTools.Method(typeof(ThingDef), "GetModExtension", null, new Type[] { typeof(LegacyModExtension) });
                FieldInfo allowStackExceedField = AccessTools.Field(typeof(LegacyModExtension), "allowStackLimitExceed");

                Label? skipBranch = null;
                List<Predicate<CodeInstruction>> conds = new List<Predicate<CodeInstruction>>() {
                    inst => inst.IsLdarg(0),
                    inst => inst.LoadsField(stackCountField),
                    inst => inst.IsLdarg(0),
                    inst => inst.LoadsField(thingDefField),
                    inst => inst.LoadsField(stackLimitField),
                    inst => inst.Branches(out skipBranch)
                };

                List<CodeInstruction> ops = raw.ToList();

                for (int instI = 0; instI < ops.Count; instI++)
                {
                    int match = 0;

                    //Look forward.
                    for (int condI = 0; condI < conds.Count && (instI + condI) < ops.Count; condI++)
                    {
                        if (!conds[condI](ops[instI + condI]))
                        {
                            break;
                        }
                        match++;
                    }

                    //Look forward fully matched.
                    if (match >= conds.Count && skipBranch.HasValue)
                    {
                        Label truncateBranch = generator.DefineLabel();
                        
                        List<CodeInstruction> patch = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, thingDefField),
                            new CodeInstruction(OpCodes.Callvirt, hasModDef),
                            new CodeInstruction(OpCodes.Brfalse_S, truncateBranch),
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, thingDefField),
                            new CodeInstruction(OpCodes.Callvirt, getModDef),
                            new CodeInstruction(OpCodes.Ldfld, allowStackExceedField),
                            new CodeInstruction(OpCodes.Brtrue_S, skipBranch)
                        };

                        while (!ops[instI].OperandIs(skipBranch.Value))
                        {
                            instI++;
                            if (instI >= ops.Count)
                            {
                                //Failed.
                                Log.Error("Patch failed.");
                                return raw;
                            }
                        }

                        ops[instI + 1].labels.Add(truncateBranch);
                        ops.InsertRange(instI + 1, patch);
                    }
                }

                return ops;
            }
        }

        [HarmonyPatch(typeof(BillUtility), "MakeNewBill")]
        public static class DD_BillUtility_MakeNewBill
        {
            public static void Postfix(RecipeDef recipe, ref Bill __result)
            {
                if (recipe.HasModExtension<BillGeneratorOverrideExtension>())
                {
                    __result = recipe.GetModExtension<BillGeneratorOverrideExtension>().NewBill(recipe);
                }
            }
        }
    }
}
