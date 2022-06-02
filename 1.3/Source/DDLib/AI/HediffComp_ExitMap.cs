using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class HediffComp_ExitMap : HediffComp
    {
        public int ticks;

        public HediffCompProperties_ExitMap Props => (HediffCompProperties_ExitMap)props;

        public override bool CompShouldRemove => false;

        public override string CompLabelInBracketsExtra => Props.showRemainingTime ? ticks.ToStringTicksToDays() : base.CompLabelInBracketsExtra;

        public override void CompPostMake()
        {
            base.CompPostMake();

            ticks = Props.exitAfterTicks.RandomInRange;
            Pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + ticks;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if(ticks > 0)
            {
                ticks--;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticks, "ticks", 0);
        }

        public override string CompDebugString()
        {
            return "ticksToExit: " + ticks;
        }
    }
}
