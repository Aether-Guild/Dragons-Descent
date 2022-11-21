using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    //[HarmonyPatch(typeof(Pawn), "TryStartAttack")]
    //public static class DD_Pawn_TryStartAttack
    //{
    //    public static void Postfix(Pawn __instance, ref bool __result, LocalTargetInfo targ)
    //    {
    //        if (__instance.stances.FullBodyBusy || __instance.WorkTagIsDisabled(WorkTags.Violent) || !targ.HasThing || __result)
    //        {
    //            return;
    //        }

    //        if (!__instance.kindDef.HasModExtension<VerbSettingExtension>() || !__instance.kindDef.GetModExtension<VerbSettingExtension>().useExtendedVerbs)
    //        {
    //            //Doesn't have the extension, or not set to use extended verbs.
    //            return;
    //        }

    //        if (VerbUtils.GetPossibleAbilityVerbs(__instance).Filter_KeepInRange(targ.Thing).TryRandomElementByWeight(v => v.verbProps.commonality, out Verb verb))
    //        {
    //            if (__instance.CurJobDef != null && __instance.CurJobDef.defName == "Mounted")
    //            {
    //                //GiddyUp
    //                if (verb.VerbIsControlledByAbilityBase(out var ability))
    //                {
    //                    if (verb.Ability_ExpandVerbSelection().TryRandomElementByWeight(v => v.verbProps.commonality, out verb))
    //                    {
    //                        __result = verb.TryStartCastOn(targ);
    //                        if (__result && ability.HasCooldown)
    //                        {
    //                            ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
