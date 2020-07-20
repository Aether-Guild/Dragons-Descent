using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public static class Extensions
    {
        public static bool IsTameable(this Pawn pawn)
        {
            if(pawn != null && pawn.health != null && pawn.health.hediffSet != null)
            {
                //If it has any hediff that disables taming, then it is not tameable.
                return !pawn.health.hediffSet.GetAllComps().OfType<HediffComp_DisableTaming>().Any();
            }
            return true;
        }

        public static bool IsEnraged(this Pawn pawn)
        {
            if (pawn != null && pawn.health != null && pawn.health.hediffSet != null)
            {
                //If it has either hediffs, it is not tameable.
                return pawn.health.hediffSet.GetAllComps().OfType<HediffComp_Aggressive>().Any();
            }
            return false;
        }
    }
}
