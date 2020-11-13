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
    [HarmonyPatch(typeof(Pawn), "PostApplyDamage")]
    public static class DD_Pawn_PostApplyDamage
    {
        public static void Postfix(Pawn __instance, DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.Instigator == null)
            {
                //Need to know who attacked.
                return;
            }

            if (dinfo.Instigator is Pawn pawn)
            {
                if (__instance.RaceProps != null && pawn.records != null)
                {
                    //Handling Damage Given. (Check what type of pawn this pawn attacked, then add the damage value to the appropriate category)
                    switch (__instance.RaceProps.intelligence)
                    {
                        case Intelligence.Animal:
                            pawn.records.AddTo(DD_RecordDefOf.DamageDealtAnimals, totalDamageDealt);
                            break;
                        case Intelligence.ToolUser:
                            pawn.records.AddTo(DD_RecordDefOf.DamageDealtMechanoids, totalDamageDealt);
                            break;
                        case Intelligence.Humanlike:
                            pawn.records.AddTo(DD_RecordDefOf.DamageDealtHumanlike, totalDamageDealt);
                            break;
                    }
                }

                if (pawn.RaceProps != null && __instance.records != null)
                {
                    //Handling Damage Taken. (Check what type of pawn attacked this pawn, then add the damage value to the appropriate category)
                    switch (pawn.RaceProps.intelligence)
                    {
                        case Intelligence.Animal:
                            __instance.records.AddTo(DD_RecordDefOf.DamageTakenAnimals, totalDamageDealt);
                            break;
                        case Intelligence.ToolUser:
                            __instance.records.AddTo(DD_RecordDefOf.DamageTakenMechanoids, totalDamageDealt);
                            break;
                        case Intelligence.Humanlike:
                            __instance.records.AddTo(DD_RecordDefOf.DamageTakenHumanlike, totalDamageDealt);
                            break;
                    }
                }
            }
        }
    }
}
