using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DD
{
    //For Targets with a specific body defs
    public class CompTargetable_SingleBody : CompTargetable
    {
        protected override bool PlayerChoosesTarget => true;

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetBuildings = false,
                validator = (x =>
                {
                    Pawn p = x.Thing as Pawn;
                    CompProperties_TargetableBody bodyProps = (CompProperties_TargetableBody)props;
                    if (p != null && p.RaceProps.body.defName.Equals(bodyProps.targetDef))
                    {
                        return this.BaseTargetValidator(x.Thing);
                    }
                    return false;
                })
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }
    }
}
