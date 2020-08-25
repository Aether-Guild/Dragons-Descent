using System;
using Verse;

namespace DD
{
    public class HediffCompProperties_ExitMap : HediffCompProperties
    {
        public IntRange exitAfterTicks;

        public bool showRemainingTime;

        public HediffCompProperties_ExitMap()
        {
            compClass = typeof(HediffComp_ExitMap);
        }
    }
}
