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
    public class ThinkNode_ConditionalExitTimedOutOnce : ThinkNode_ConditionalExitTimedOut
    {
        protected override bool Satisfied(Pawn pawn)
        {
            if (!pawn.jobs.jobQueue.Select(qj => qj.job.def).Contains(JobDefOf.Goto) && pawn.Faction == null)
            {
                return base.Satisfied(pawn);
            }
            return false;
        }
    }
}
