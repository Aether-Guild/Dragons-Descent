using UnityEngine;
using Verse;
using RimWorld;

namespace DD
{
    public class HediffComp_ModifyAge : HediffComp
    {
        private long amount;
        private TimeKeeper timer = new TimeKeeper();

        private HediffCompProperties_ModifyAge valProps => (HediffCompProperties_ModifyAge)props;

        public override string CompLabelInBracketsExtra => timer.Remaining.ToStringTicksToPeriodVague();

        public override string CompTipStringExtra => "Remaining:\n-Age Adjustment: " + ((int)(amount * timer.Remaining)).ToStringTicksToPeriodVague(true) + "\n-Duration: " + timer.Remaining.ToStringTicksToPeriodVague(false, false);

        public override string CompDebugString() => "Duration: " + timer.Remaining.ToStringSecondsFromTicks();

        public override bool CompShouldRemove => base.CompShouldRemove || amount == 0 || timer.Expired;

        public override void CompPostMake()
        {
            int hediffDuration = valProps.DurationAmount;

            amount = valProps.AgeChangeAmount / hediffDuration;
            timer.Update(hediffDuration);
        }

        public override void CompExposeData()
        {
            Scribe_Deep.Look(ref timer, "timer", 0);
            Scribe_Values.Look(ref amount, "amount", 0);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            switch (valProps.Method)
            {
                case AgeUtils.AgeUpdateMethod.AddAge:
                    AgeUtils.AddPawnAge(Pawn, amount);
                    break;
                case AgeUtils.AgeUpdateMethod.SetAge:
                    AgeUtils.SetPawnAge(Pawn, amount);
                    break;
            }
        }
    }
}
