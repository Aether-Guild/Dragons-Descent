using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class CompScaleable : CompHasGatherableBodyResource
    {
        protected override int GatherResourcesIntervalDays => Props.scaleIntervalDays;

        protected override int ResourceAmount => Props.scaleAmount;

        protected override ThingDef ResourceDef => Props.scaleDef;

        protected override string SaveKey => "looseScales";

        public CompProperties_Scaleable Props => (CompProperties_Scaleable)props;

        protected override bool Active
        {
            get
            {
                if (!base.Active)
                {
                    return false;
                }

                Pawn pawn = parent as Pawn;
                if (pawn != null && pawn.ageTracker.CurLifeStageIndex < pawn.RaceProps.lifeStageAges.FirstIndexOf(stage => stage.def == Props.minScaleableLifeStage))
                {
                    //Not null, and current life stage is greater than or equal to the min life stage set in XML
                    return false;
                }
                return true;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (!Active)
            {
                return null;
            }

            return "LooseScales".Translate() + ": " + base.Fullness.ToStringPercent();
        }
    }
}
