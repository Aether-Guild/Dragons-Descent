using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace DD
{
    //Used to specify the target body type.
    public class CompProperties_TargetableBody : CompProperties_Targetable
    {
        public List<BodyDef> targetDefs = new List<BodyDef>();
    }

    //-- SINGLE TARGET --
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

    //For Targets old enough to reproduce
    public class CompTargetable_Reproductive : CompTargetable
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
                    if (p != null && p.ageTracker.CurLifeStage.reproductive)
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
                    if (p != null && p.RaceProps != null && bodyProps.targetDefs.Contains(p.RaceProps.body))
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

    //For Targets with a specific body defs AND old enough to reproduce
    public class CompTargetable_SingleReproductiveBody : CompTargetable
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
                    if (p != null && p.ageTracker != null && p.RaceProps != null && p.ageTracker.CurLifeStage.reproductive && bodyProps.targetDefs.Contains(p.RaceProps.body))
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

    //For Targets marked with the 'MatingTargetExtension' DefModExtension.
    public class CompTargetable_MatingTarget : CompTargetable
    {
        protected override bool PlayerChoosesTarget => true;

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetBuildings = false,
                validator = x => x.HasThing && x.Thing is Pawn && x.Thing.def.HasModExtension<MatingTargetExtension>() && this.BaseTargetValidator(x.Thing)
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }
    }

    //-- MULTIPLE TARGET --
    //For all pawns of a specific body def on map.
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
                    if (p != null && p.RaceProps != null && bodyProps.targetDefs.Contains(p.RaceProps.body))
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

        //For all pawns marked as mating target on map.
        public class CompTargetable_AllMatingTargetsOnTheMap : CompTargetable
        {
            protected override bool PlayerChoosesTarget => false;

            protected override TargetingParameters GetTargetingParameters()
            {
                return new TargetingParameters
                {
                    canTargetPawns = true,
                    canTargetBuildings = false,
                    validator = x => x.HasThing && x.Thing is Pawn && x.Thing.def.HasModExtension<MatingTargetExtension>() && this.BaseTargetValidator(x.Thing)
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
}
