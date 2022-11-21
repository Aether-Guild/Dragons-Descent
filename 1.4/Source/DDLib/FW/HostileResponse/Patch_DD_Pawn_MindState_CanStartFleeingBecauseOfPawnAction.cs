using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    //[HarmonyPatch(typeof(Pawn_MindState), "CanStartFleeingBecauseOfPawnAction")]
    //public class Patch_DD_Pawn_MindState_CanStartFleeingBecauseOfPawnAction
    //{
    //    public static void Postfix(Pawn p, ref bool __result)
    //    {
    //        CompHostileResponse comp = p.GetComp<CompHostileResponse>();

    //        if (!__result)
    //        {
    //            return;
    //        }

    //        if (comp == null)
    //        {
    //            return;
    //        }

    //        if (comp.Type == HostilityResponseType.Passive)
    //        {
    //            return;
    //        }

    //        //If it knows an enemy.
    //         __result = !comp.Targets.EnumerableNullOrEmpty();
    //    }
    //}
}
