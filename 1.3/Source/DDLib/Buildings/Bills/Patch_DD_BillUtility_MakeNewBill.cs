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
