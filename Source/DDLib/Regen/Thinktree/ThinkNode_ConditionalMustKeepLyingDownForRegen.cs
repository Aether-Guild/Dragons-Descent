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
    public class ThinkNode_ConditionalMustKeepLyingDownForRegen : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return HealthUtils.ShouldLieDown(pawn);
        }
    }
}
