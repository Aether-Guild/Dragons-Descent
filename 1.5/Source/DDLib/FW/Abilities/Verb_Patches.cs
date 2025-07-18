using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    //[HarmonyPatch(typeof(Verb), "Available")]
    //public static class DD_Verb_Available
    //{
    //    public static void Postfix(Verb __instance, ref bool __result)
    //    {
    //        if (__result && __instance.VerbIsControlledByAbilityBase(out var ability))
    //        {
    //            __result = ability.Ready;
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Verb), "CanHitTarget")]
    //public static class DD_Verb_CanHitTarget
    //{
    //    public static void Postfix(Verb __instance, ref bool __result, LocalTargetInfo targ)
    //    {
    //        if (__result && __instance.VerbIsControlledByAbilityBase(out var ability))
    //        {
    //            __result = ability.CanApplyOn(targ);
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Verb), "Reset")]
    //public static class DD_Verb_Reset
    //{
    //    public static void Postfix(Verb __instance)
    //    {
    //        if (__instance.VerbIsControlledByAbilityBase(out var ability))
    //        {
    //            ability.Reset();
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Verb), "CanHitTargetFrom")]
    //public static class DD_Verb_CanHitTargetFrom
    //{
    //    public static void Postfix(Verb __instance, ref bool __result, IntVec3 root, LocalTargetInfo targ)
    //    {
    //        if (__result && __instance.VerbIsControlledByAbilityBase(out var ability))
    //        {
    //            __result = ability.CanApplyOn(targ);
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(Verb), "TryCastNextBurstShot")]
    //public static class DD_Verb_TryCastNextBurstShot
    //{
    //    public static void Postfix(Verb __instance)
    //    {
    //        if (__instance.VerbIsControlledByAbilityBase(out var ability) && __instance.state == VerbState.Idle)
    //        {
    //            var comp = ability.CompOfType<AbilityComp_Cooldown>();
    //            Log.Message(ability.def + " - comp: " + comp);
    //            comp?.TryStartCooldown();
    //        }
    //    }
    //}

    //[HarmonyPatch]
    //public static class DD_Verb_OrderForceTarget
    //{
    //    [HarmonyTargetMethods]
    //    public static IEnumerable<MethodBase> TargetMethods()
    //    {
    //        foreach (var subType in typeof(Verb).AllSubclasses().AddItem(typeof(Verb)))
    //        {
    //            var method = AccessTools.DeclaredMethod(subType, "OrderForceTarget");
    //            if (method != null)
    //            {
    //                Log.Message("Patching " + method);
    //                yield return method;
    //            }
    //        }
    //    }
    //    public static void Postfix(Verb __instance)
    //    {
    //        Log.Message("OrderForceTarget: " + __instance + " - " + __instance.verbTracker?.directOwner);
    //        if (__instance.verbTracker?.directOwner is Ability_Base ability)
    //        {
    //            __instance.CasterPawn.CurJob.verbToUse = __instance;
    //        }
    //    }
    //}
}
