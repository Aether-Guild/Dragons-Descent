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
    [HarmonyPatch(typeof(JobGiver_GetFood), "TryGiveJob")]
    public static class DD_JobGiver_GetFood_TryGiveJob
    {
        public static void Postfix(Pawn pawn, ref Job __result)
        {
            if (pawn != null && pawn.VerbTracker != null && !pawn.VerbTracker.AllVerbs.NullOrEmpty() && __result != null && __result.def == JobDefOf.PredatorHunt)
            {
                if (pawn.VerbTracker.AllVerbs.Any(verb => verb.maneuver != null && verb.maneuver.HasModExtension<VerbUsageExtension>() && !verb.maneuver.GetModExtension<VerbUsageExtension>().useWhileHunting))
                {
                    __result.verbToUse = pawn.VerbTracker.AllVerbs.Where(verb => verb.IsMeleeAttack && verb.Available() && verb.maneuver != null && (!verb.maneuver.HasModExtension<VerbUsageExtension>() || verb.maneuver.GetModExtension<VerbUsageExtension>().useWhileHunting)).RandomElementWithFallback();
                }
            }
        }
    }
}
