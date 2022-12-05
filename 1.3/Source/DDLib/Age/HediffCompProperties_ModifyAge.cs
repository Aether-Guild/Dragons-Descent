using System;
using Verse;

namespace DD
{
    public class HediffCompProperties_ModifyAge : HediffCompProperties
    {
        public AgeUtils.AgeUpdateMethod updateMethod;

        public GameTime ageAdjustment, duration;

        public HediffCompProperties_ModifyAge()
        {
            compClass = typeof(HediffComp_ModifyAge);
        }

        public AgeUtils.AgeUpdateMethod Method => updateMethod;

        public int AgeChangeAmount => ageAdjustment.TickRange.RandomInRange;
        public int DurationAmount => duration.TickRange.RandomInRange;
    }
}
