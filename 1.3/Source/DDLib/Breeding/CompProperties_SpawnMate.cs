using Verse;

namespace DD
{
    public class CompProperties_SpawnMate : CompProperties
    {
        public bool spawnTamed = false;
        public bool targetHediff = false;
        public bool mateHediff = true;
        public FloatRange spawnedAgeRange = new FloatRange();
        public MentalStateDef penaltyMentalDef;
        public string penaltyReason;
        public bool targetPenalty = false;
        public bool matePenalty = true;
        public bool spawnDuringPenalty = false;
    }
}
