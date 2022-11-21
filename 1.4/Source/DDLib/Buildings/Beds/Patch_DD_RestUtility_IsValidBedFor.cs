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
    [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
    public static class DD_RestUtility_IsValidBedFor
    {
        public static void Postfix(ref bool __result, Thing bedThing, Pawn sleeper)
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
}
