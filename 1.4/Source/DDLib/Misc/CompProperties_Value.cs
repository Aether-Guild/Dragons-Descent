using Verse;

namespace DD
{
    public class CompProperties_Value : CompProperties
    {
        public float value;

        public virtual float? GetAmount(Pawn pawn) => value;
    }
}
