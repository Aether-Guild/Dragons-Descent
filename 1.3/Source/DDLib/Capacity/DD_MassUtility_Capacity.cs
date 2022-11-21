using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    [HarmonyPatch(typeof(MassUtility), "Capacity")]
    public static class DD_MassUtility_Capacity
    {
        public static void Postfix(ref float __result, Pawn p, StringBuilder explanation)
        {
            if (p.def.GetModExtension<CarryCapacityExtension>() is CarryCapacityExtension ext)
            {
                float value = __result;

                if (ext.constant.HasValue)
                {
                    value = ext.constant.Value;
                }
                else
                {
                    value += ext.offset;
                    value *= ext.factor;
                    if (ext.cap.HasValue)
                    {
                        value = ext.cap.Value.ClampToRange(value);
                    }
                }

                if (explanation != null)
                {
                    int index = explanation.ToString().LastIndexOf(": ");
                    if(index >= 0)
                    {
                        explanation.Remove(index, explanation.Length - index);
                        explanation.Append(": " + value.ToStringMassOffset());
                    }
                }

                __result = value;
            }
        }
    }
}
