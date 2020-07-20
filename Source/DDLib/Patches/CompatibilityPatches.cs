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
        public static readonly string DragonBreathWeaponDef = "Gun_DragonBreathWeapon";
        public static readonly string DragonRaceBodyDef = "QuadrupedeAnimalWithClawsDragon";

        public static List<string> fightingJobDefNames = new List<string>()
            {
                //KFM
                "KFM_KillTarget",
                //HFM
                "HuntTrained",
                "HuntTrainedAssist",
                "HuntForMe_CatGift",
                //DD
                "AA_DragonAnimalRangeAttack"
            };

        private static readonly string HarmonyCompatibilityPatchID = "com.rimworld.mod.dd.compat";

        public static void Patch()
        {
            try
            {
                Log.Message("Preparing to apply compatibility patches...");
                KFM_IgnoreDragonRanged();
                HFM_IgnoreDragonRanged();

                Add_HarmonyPatch(typeof(PawnUtility).GetMethod("IsFighting"), null, new HarmonyMethod(typeof(CompatibilityPatch).GetMethod("PawnUtility_IsFighting_Patch")));
                Replace_HarmonyPatch(typeof(Pawn).GetMethod("TryGetAttackVerb"), HarmonyPatchType.Prefix, patch => patch.PatchMethod.ReflectedType.Name == "ARA__VerbCheck_Patch", null, new HarmonyMethod(typeof(CompatibilityPatch).GetMethod("Compat__VerbCheck_Patch")));
                Log.Message("Compatibility patches were applied successfully.");
            }
            catch (Exception ex)
            {
                Log.Message("Compatibility patching failed.");
                throw;
            }
        }

        //KFM
        private static void KFM_IgnoreDragonRanged()
        {
            //KFM-fix add dragons to block list.
            Type Type_KFM_Settings = Type.GetType("aRandomKiwi.KFM.Settings, aRandomKiwi_KillForMe");
            if (Type_KFM_Settings != null)
            {
                try
                {
                    Log.Message("KFM was detected.\nAttempting to add dragons to its internal range ignore list...");

                    //Retrieve the ignore list.
                    List<string> ignoredRangedAttack = (List<string>)Type_KFM_Settings.GetField("ignoredRangedAttack").GetValue(null);
                    //Lookup the dragon's thingdefs and add their defNames to the ignore list.
                    ignoredRangedAttack.AddRange(DefDatabase<ThingDef>.AllDefs.Where(t => t != null && t.race != null && t.race.body != null && t.race.body.defName.Equals(DragonRaceBodyDef) && !ignoredRangedAttack.Contains(t.defName)).Select(t => t.defName));

                    Log.Message("Dragons were successfully added to the KFM ignore range list.");
                }
                catch (Exception ex)
                {
                    Log.Message("Could not add dragons to the ignore list.");
                    throw;
                }
            }
        }

        //HFM
        private static void HFM_IgnoreDragonRanged()
        {
            //HFM-fix add dragons to block list.
            Type Type_KFM_Settings = Type.GetType("aRandomKiwi.HFM.Settings, aRandomKiwi_HuntForMe");
            if (Type_KFM_Settings != null)
            {
                try
                {
                    Log.Message("HFM was detected.\nAttempting to add dragons to its internal range ignore list...");

                    //Retrieve the ignore list.
                    List<string> ignoredRangedAttack = (List<string>)Type_KFM_Settings.GetField("ignoredRangedAttack").GetValue(null);
                    //Lookup the dragon's thingdefs and add their defNames to the ignore list.
                    ignoredRangedAttack.AddRange(DefDatabase<ThingDef>.AllDefs.Where(t => t != null && t.race != null && t.race.body != null && t.race.body.defName.Equals(DragonRaceBodyDef) && !ignoredRangedAttack.Contains(t.defName)).Select(t => t.defName));

                    Log.Message("Dragons were successfully added to the HFM ignore range list.");
                }
                catch (Exception ex)
                {
                    Log.Message("Could not add dragons to the ignore list.");
                    throw;
                }
            }
        }

        //Harmony Compatibility
        private static void Add_HarmonyPatch(MethodInfo original, HarmonyMethod prefixMethod = null, HarmonyMethod postfixMethod = null, HarmonyMethod transpilerMethod = null)
        {
            Harmony harmony = new Harmony(HarmonyCompatibilityPatchID);
            harmony.Patch(original, prefixMethod, postfixMethod, transpilerMethod);
        }

        private static void Replace_HarmonyPatch(MethodInfo original, HarmonyPatchType targetPatchType, Func<Patch, bool> isTargetPatch, HarmonyMethod prefixReplacementMethod = null, HarmonyMethod postfixReplacementMethod = null, HarmonyMethod transpilerReplacementMethod = null)
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

            // PawnUtility.IsFighting KFM/HFM
            public static void PawnUtility_IsFighting_Patch(ref bool __result, Pawn pawn)
            {
                if (!__result && pawn.CurJob != null)
                {
                    //Not fighting (vanilla) and has a job.
                    foreach (string defName in fightingJobDefNames)
                    {
                        JobDef def = DefDatabase<JobDef>.GetNamedSilentFail(defName);

                        __result = pawn.CurJobDef == def;

                        if (__result)
                        {
                            //Found a fighting job.
                            break;
                        }
                    }
                }
            }
        }
    }
}
