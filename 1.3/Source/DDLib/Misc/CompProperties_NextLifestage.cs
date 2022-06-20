using Verse;

namespace DD
{
    public class CompProperties_NextLifestage : CompProperties_Value
    {
        public override float? GetAmount(Pawn pawn)
        {
            int index = pawn.ageTracker.CurLifeStageIndex + 1;
            return index < pawn.RaceProps.lifeStageAges.Count ? pawn.RaceProps.lifeStageAges[index].minAge : (float?)null;
        }
    }
}
