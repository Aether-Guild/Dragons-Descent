using Verse;

namespace DD
{
    public class CompProperties_LifespanPercent : CompProperties_Value
    {
        public override float? GetAmount(Pawn pawn) => pawn.RaceProps.lifeExpectancy * value;
    }
}
