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
        public bool giveWhileFighting = false;
        public bool startInBed = false;

        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if (HealthUtils.CanRegen(pawn, giveWhileFighting) && (!startInBed || (startInBed && pawn.InBed())))
            {
                TryApply(pawn);
            }
        }
    }
}
