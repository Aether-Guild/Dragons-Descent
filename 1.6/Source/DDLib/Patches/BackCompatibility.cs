using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Diagnostics;
using Verse;

namespace DD
{



    public static class Patch_DD_BackCompatibility
    {
        [HarmonyPatch(typeof(BackCompatibilityConverter_1_4), nameof(BackCompatibilityConverter_1_4.BackCompatibleDefName))]
        public class BackCompatibleDefName
        {
            public static void Postfix(Type defType, string defName, ref string __result)
            {
                if (defType == typeof(ThingDef))
                {
                    if (defName == "Projectile_RedDragonBreath_Fire") __result = "DD_Projectile_DragonBreath_Fire";
                }
                {
                    
                    if (defName == "Projectile_DragonSpit") __result = "DD_Projectile_DragonSpit ";
                }
                {
                    
                    if (defName == "Projectile_DragonSpitBlunt") __result = "DD_Projectile_DragonSpitBlunt";
                }
            }

        }
    }
}
