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
    [StaticConstructorOnStartup]
    public static class Patcher
    {
        private static readonly string HarmonyPatchID = "com.rimworld.mod.dd";

        static Patcher()
        {
            Harmony harmony = new Harmony(HarmonyPatchID);
            harmony.PatchAll();
        }
    }
}
