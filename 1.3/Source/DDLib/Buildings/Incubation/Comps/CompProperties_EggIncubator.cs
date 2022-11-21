using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class CompProperties_EggIncubator : CompProperties
    {
        public bool debug = false;
        public FloatRange basePercentageDailyProgress;
        public Vector3 eggDrawOffset = Vector3.zero;

        public GameTime factionCheckInterval, factionChangeInterval;

        public string gizmoLabel;
        public string gizmoDesc;
        public string gizmoIconPath;

        public virtual float GetProgressIncrease(CompEggIncubator incubator) => basePercentageDailyProgress.RandomInRange * incubator.BaseProgress;

        public Gizmo CreateGizmo(CompEggIncubator compIncubator)
        {
            if (compIncubator is CompEggIncubator_Container compContainer)
            {
                Command_Action action_ExtractEgg = new Command_Action();
                action_ExtractEgg.defaultLabel = gizmoLabel;
                action_ExtractEgg.defaultDesc = gizmoDesc;
                if (!gizmoIconPath.NullOrEmpty())
                {
                    action_ExtractEgg.icon = ContentFinder<Texture2D>.Get(gizmoIconPath);
                }
                action_ExtractEgg.action = () => compContainer.PlaceEggOnGround(compIncubator.parent.Map);
                return action_ExtractEgg;
            }
            return null;
        }
    }

    public class CompProperties_EggIncubatorNestBonus : CompProperties_EggIncubator
    {
        public FloatRange bonusPercentageDailyProgress;

        public virtual bool ShouldApplyBonus(CompEggIncubator incubator)
        {
            Building_Bed bed = incubator.parent as Building_Bed;

            if (bed != null)
            {
                //A pawn is in this bed.
                return !bed.CurOccupants.EnumerableNullOrEmpty();
            }

            return false;
        }

        public override float GetProgressIncrease(CompEggIncubator incubator) => base.GetProgressIncrease(incubator) + (ShouldApplyBonus(incubator) ? bonusPercentageDailyProgress.RandomInRange * incubator.BaseProgress : 0f);
    }
}
