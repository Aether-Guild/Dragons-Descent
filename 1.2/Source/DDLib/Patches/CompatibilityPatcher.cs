using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public static class CompatibilityPatcher
    {
        private static readonly string HarmonyCompatibilityPatchID = "com.rimworld.mod.dd.compat";

        public static bool Patched { get; private set; } = false;

        public static void Patch()
        {
            if (!Patched)
            {
                try
                {
                    Log.Message("Preparing to apply compatibility patches...");

                    if (DraconicOverseer.Settings.KFM_IgnoreRange)
                    {
                        KFM_AddRanged();
                    }

                    if (DraconicOverseer.Settings.HFM_IgnoreRange)
                    {
                        HFM_AddRanged();
                    }

                    if (DraconicOverseer.Settings.ARA_VerbCheck)
                    {
                        ARA_VerbCheck_Compat();
                    }

                    Patched = true;
                    Log.Message("Compatibility patches were applied successfully.");
                }
                catch (Exception ex)
                {
                    Log.Message("Compatibility patching failed.");
                    throw;
                }
            }
        }

        //KFM
        private static void KFM_AddRanged()
        {
            //KFM-fix add dragons to block list.
            Type Type_KFM_Utils = Type.GetType("aRandomKiwi.KFM.Utils, aRandomKiwi_KillForMe");
            if (Type_KFM_Utils != null)
            {
                try
                {
                    Log.Message("KFM was detected.\nPatching to add ability verbs...");

                    MethodInfo originalMethod = AccessTools.Method(Type_KFM_Utils, "hasRemoteVerbAttack");
                    Add_HarmonyPatch(originalMethod, postfixMethod: new HarmonyMethod(typeof(CompatibilityPatch), "DFM_hasRemoteAttack_Patch"));

                    Log.Message("KFM patch was successfully applied.");
                }
                catch (Exception ex)
                {
                    Log.Message("Could not patch KFM.");
                    throw;
                }
            }
        }

        //HFM
        private static void HFM_AddRanged()
        {
            //HFM-fix add dragons to block list.
            Type Type_HFM_Utils = Type.GetType("aRandomKiwi.HFM.Utils, aRandomKiwi_HuntForMe");
            if (Type_HFM_Utils != null)
            {
                try
                {
                    Log.Message("HFM was detected.\nPatching to add ability verbs...");

                    MethodInfo originalMethod = AccessTools.Method(Type_HFM_Utils, "hasRemoteVerbAttack");
                    Add_HarmonyPatch(originalMethod, postfixMethod: new HarmonyMethod(typeof(CompatibilityPatch), "DFM_hasRemoteAttack_Patch"));

                    Log.Message("HFM patch was successfully applied.");
                }
                catch (Exception ex)
                {
                    Log.Message("Could not patch HFM.");
                    throw;
                }
            }
        }

        //ARA
        private static void ARA_VerbCheck_Compat()
        {
            MethodInfo pawn_attackVerb = AccessTools.Method(typeof(Pawn), "TryGetAttackVerb");
            HarmonyMethod attackVerb_Postfix = new HarmonyMethod(typeof(CompatibilityPatch), "Compat__VerbCheck_Patch");

            Replace_HarmonyPatch(pawn_attackVerb, HarmonyPatchType.Prefix, patch => patch.PatchMethod.ReflectedType.Name == "ARA__VerbCheck_Patch", null, attackVerb_Postfix);
        }


        //Harmony Compatibility
        internal static void Add_HarmonyPatch(MethodInfo original, HarmonyMethod prefixMethod = null, HarmonyMethod postfixMethod = null, HarmonyMethod transpilerMethod = null)
        {
            Harmony harmony = new Harmony(HarmonyCompatibilityPatchID);
            harmony.Patch(original, prefixMethod, postfixMethod, transpilerMethod);
        }

        internal static void Replace_HarmonyPatch(MethodInfo original, HarmonyPatchType targetPatchType, Func<Patch, bool> isTargetPatch, HarmonyMethod prefixReplacementMethod = null, HarmonyMethod postfixReplacementMethod = null, HarmonyMethod transpilerReplacementMethod = null)
        {
            List<Patch> patches = null;

            Patches HPatches = Harmony.GetPatchInfo(original);

            if (HPatches != null)
            {
                //Has patches on the specified method.
                switch (targetPatchType)
                {
                    case HarmonyPatchType.Prefix:
                        patches = HPatches.Prefixes.ToList();
                        break;
                    case HarmonyPatchType.Postfix:
                        patches = HPatches.Postfixes.ToList();
                        break;
                    case HarmonyPatchType.Transpiler:
                        patches = HPatches.Transpilers.ToList();
                        break;
                }
            }

            if (!patches.NullOrEmpty())
            {
                //Selected a valid patch type.
                foreach (Patch patch in patches.Where(isTargetPatch))
                {
                    //Only iterates over target patches.
                    try
                    {
                        Log.Message("Found an incompatible Harmony Prefix Patch '" + patch.PatchMethod.ReflectedType.Name + "' on '" + original.ReflectedType.Name + "." + original.Name + "' in " + patch.PatchMethod.DeclaringType.Assembly.FullName + "(" + patch.owner + ")\nAttempting to replace it with compatibility patches...");

                        Harmony harmony = new Harmony(patch.owner);
                        harmony.Unpatch(original, patch.PatchMethod); //Remove their patch
                        harmony.Patch(original, prefixReplacementMethod, postfixReplacementMethod, transpilerReplacementMethod); //Replace with compatibility patch

                        Log.Message("Compatibility patch was applied successfully.");
                    }
                    catch (Exception ex)
                    {
                        Log.Message("Compatibility patch failed.");
                        throw;
                    }
                }
            }
        }

        //Harmony Compatibility Patches
        private static class CompatibilityPatch
        {
            //ARA__VerbCheck_Patch
            public static void Compat__VerbCheck_Patch(ref Pawn __instance, ref Verb __result)
            {
                //Execute for animals and wild people only
                if (__instance.AnimalOrWildMan())
                {
                    //If no Verb was selected already, or the selected verb was melee, or the selected verb's range is less than or equal to 1.
                    if (__result == null || __result.IsMeleeAttack || __result.verbProps.range <= 1f)
                    {
                        foreach (Verb verb in __instance.verbTracker.AllVerbs)
                        {
                            if (verb.Available() && verb.verbProps.range > 1f)
                            {
                                //Verb that is both available and ranged.
                                __result = verb;
                                break;
                            }
                        }
                    }
                }
            }

            //KFM/HFM Compat
            public static void DFM_hasRemoteAttack_Patch(IEnumerable<Verb> verbs, ref bool __result)
            {
                if (!__result)
                {
                    foreach(Pawn pawn in verbs.Select(verb => verb.Caster))
                    {
                        if(VerbUtils.GetPossibleVerbs(pawn).Filter_KeepRanged().Any())
                        {
                            __result = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}
