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
    //[HarmonyPatch(typeof(Pawn), "Kill")]
    //public static class DD_Pawn_Kill
    //{
    //    public static void Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit, out Dictionary<EffectDef, float> __state)
    //    {
    //        __state = new Dictionary<EffectDef, float>();

    //        foreach(Effect effect in DefDatabase<EffectDef>.AllDefsListForReading.Where(def => def.spawnPostMortem).Select(def => __instance.GetAttachment(def.thingDef)).OfType<Effect>())
    //        {
    //            __state.Add(effect.EffectDef, effect.Size);
    //        }
    //    }

    //    public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit, Dictionary<EffectDef, float> __state)
    //    {
    //        if(!__state.EnumerableNullOrEmpty() && __instance.Position.IsValid && __instance.Map != null)
    //        {
    //            foreach(KeyValuePair<EffectDef, float> entry in __state)
    //            {
    //                EffectUtils.ApplyEffect(new TargetInfo(__instance.Position, __instance.Map), entry.Key, entry.Value);
    //            }
    //        }
    //    }
    //}
}
