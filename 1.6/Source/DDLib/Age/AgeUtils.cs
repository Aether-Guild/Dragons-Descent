using RimWorld;
using Verse;
namespace DD
{
    public static class AgeUtils
    {
        public enum AgeUpdateMethod { AddAge, SetAge }

        public static long YearsToTicks(float years)
        {
            return (long)(years * (float)GenDate.TicksPerYear);
        }

        public static float TicksToYears(long ticks)
        {
            return (ticks / (float)GenDate.TicksPerYear);
        }

        private static void BatchedAddTicks(Pawn pawn, long ticks)
        {
            //Hack. Integer -can- overflow if you add more than 597 years at once. But we're limited by the method parameter...
            while (ticks != 0)
            {
                int tickBatch = 0;
                if (ticks < int.MinValue)
                { //Take a slice that fits within an int. (negative)
                    tickBatch = int.MinValue;
                }
                else if (ticks > int.MaxValue)
                { //Take a slice that fits within an int. (positive)
                    tickBatch = int.MaxValue;
                }
                else
                { //Take the remaining slice.
                    tickBatch = (int)ticks;
                }

                ticks -= tickBatch; //Remove slice from the total amount of ticks we need to process.
                pawn.ageTracker.AgeTickMothballed(tickBatch); //Process a slice.
            }

            //Another hack. Force a refresh by setting the ticks manually, then requesting the index.
            pawn.ageTracker.AgeBiologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
            int index = pawn.ageTracker.CurLifeStageIndex;
        }

        public static void SetPawnAge(Pawn pawn, float years)
        {
            if (years < 0)
            {
                years = 0;
            }

            SetPawnAge(pawn, YearsToTicks(years));
        }

        public static void SetPawnAge(Pawn pawn, long ticks)
        {
            if (ticks < 0)
            {
                ticks = 0;
            }

            ticks = ticks - pawn.ageTracker.AgeBiologicalTicks;

            BatchedAddTicks(pawn, ticks);
        }

        public static void AddPawnAge(Pawn pawn, float years)
        {
            AddPawnAge(pawn, YearsToTicks(years));
        }

        public static void AddPawnAge(Pawn pawn, long ticks)
        {
            if (pawn.ageTracker.AgeBiologicalTicks + ticks < 0)
            { //Avoids age going negative when reducing it.
                SetPawnAge(pawn, 0);
            }
            else
            {
                BatchedAddTicks(pawn, ticks);
            }
        }
    }
}
