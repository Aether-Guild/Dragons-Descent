using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class HediffGiver_Regeneration : HediffGiver
    {
        public float regenTimeout = 10f;
        public bool startInBed = false;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if(pawn.health.hediffSet.HasHediff(hediff))
            {
                return;
            }

            if (HealthUtils.CanRegen(pawn, regenTimeout) && HealthUtils.ShouldRegen(pawn) && (!startInBed || pawn.InBed()))
            {
                TryApply(pawn);
            }
        }
    }
}
