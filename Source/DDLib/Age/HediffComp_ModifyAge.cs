using Verse;

namespace DD
{
    public class HediffComp_ModifyAge : HediffComp
    {
        private HediffCompProperties_ModifyAge valProps => (HediffCompProperties_ModifyAge)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            switch (valProps.Method)
            {
                case AgeUtils.AgeUpdateMethod.AddAge:
                    AgeUtils.AddPawnAge(Pawn, valProps.Amount);
                    break;
                case AgeUtils.AgeUpdateMethod.SetAge:
                    AgeUtils.SetPawnAge(Pawn, valProps.Amount);
                    break;
            }
        }
    }
}
