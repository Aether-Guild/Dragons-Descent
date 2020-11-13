using Verse;

namespace DD
{
    public class CompProperties_Range : CompProperties_Value
    {
        public FloatRange values = new FloatRange();

        public override float? GetAmount(Pawn pawn) => value + values.RandomInRange;
    }
}
