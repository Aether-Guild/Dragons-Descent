using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DD
{
    //For targeting a single animal
    public class CompTargetable_SingleAnimal : CompTargetable
    {
        protected override bool PlayerChoosesTarget => true;

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetHumans = false,
                canTargetBuildings = false,
                canTargetAnimals = true,
                validator = ((TargetInfo x) => BaseTargetValidator(x.Thing))
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }
    }
}
