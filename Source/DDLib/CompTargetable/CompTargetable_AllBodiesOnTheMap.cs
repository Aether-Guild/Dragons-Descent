using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace DD
{
    public class CompTargetable_AllBodiesOnTheMap : CompTargetable
    {
        protected override bool PlayerChoosesTarget => false;

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
                    if (p != null && p.RaceProps.body == bodyProps.targetDef)
                    {
                        return this.BaseTargetValidator(x.Thing);
                    }
                    return false;
                })
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            if (parent.MapHeld != null)
            {
                TargetingParameters tp = GetTargetingParameters();
                foreach (Pawn pawn in parent.MapHeld.mapPawns.AllPawnsSpawned)
                {
                    if (tp.CanTarget(pawn))
                    {
                        yield return pawn;
                    }
                }
            }
        }
    }
}
