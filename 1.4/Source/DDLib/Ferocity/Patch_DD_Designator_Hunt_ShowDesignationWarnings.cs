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
    [HarmonyPatch(typeof(Designator_Hunt), "ShowDesignationWarnings")]
    public static class DD_Designator_Hunt_ShowDesignationWarnings
    {
        public static bool Prefix(Pawn pawn)
        {
            if (pawn.IsEnraged())
            {
                //Skip messaging vanilla message if pawn is enraged.
                float percent = 1;

                Messages.Message("MessageAnimalsGoPsychoHunted".Translate(pawn.kindDef.GetLabelPlural().CapitalizeFirst(), percent.ToStringPercent(), pawn.Named("ANIMAL")).CapitalizeFirst(), pawn, MessageTypeDefOf.CautionInput, historical: false);

                return false;
            }
            return true;
        }
    }
}
