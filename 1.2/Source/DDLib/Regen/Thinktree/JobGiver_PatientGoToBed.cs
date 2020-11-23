using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    public class JobGiver_RegenPatientGoToBed : JobGiver_PatientGoToBed
    {
        public float timeout = 60f;

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            ThinkResult result = base.TryIssueJobPackage(pawn, jobParams);

            if (result.IsValid)
            {
                if(HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn))
                {
                    //Just let surgery through ffs.
                    return result;
                }

                if (HealthUtils.CanRegen(pawn, timeout) && HealthUtils.ShouldRegen(pawn) && pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicRegeneration))
                {
                    //Do nothing if has regen active.
                    result = ThinkResult.NoJob;
                }
                else if (!HealthUtils.ShouldBeDoctored(pawn))
                {
                    //Do nothing if shouldn't be doctored.
                    result = ThinkResult.NoJob;
                }
            }

            return result;
        }
    }
}
