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
    public class JobGiver_PatientGoToBed : ThinkNode
    {
        public bool respectTimetable = true;

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            if(!HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn) && !pawn.health.hediffSet.HasImmunizableNotImmuneHediff())
            {
                return ThinkResult.NoJob;
            }

            if (RestUtility.DisturbancePreventsLyingDown(pawn))
            {
                return ThinkResult.NoJob;
            }

            if(respectTimetable && RestUtility.TimetablePreventsLayDown(pawn))
            {
                return ThinkResult.NoJob;
            }

            Thing thing = RestUtility.FindPatientBedFor(pawn);
            if (thing == null)
            {
                return ThinkResult.NoJob;
            }

            return new ThinkResult(JobMaker.MakeJob(JobDefOf.LayDown, thing), this);
        }
    }
}
