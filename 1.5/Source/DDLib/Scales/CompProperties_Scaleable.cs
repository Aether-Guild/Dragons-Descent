using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class CompProperties_Scaleable : CompProperties
    {
        public int scaleIntervalDays;

        public int scaleAmount = 1;

        public ThingDef scaleDef;

        public LifeStageDef minScaleableLifeStage;

        public CompProperties_Scaleable()
        {
            compClass = typeof(CompScaleable);
        }
    }
}
