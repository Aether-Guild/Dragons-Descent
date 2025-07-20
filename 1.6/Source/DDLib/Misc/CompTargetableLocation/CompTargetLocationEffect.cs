using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class CompTargetLocationEffect : ThingComp
    {
        public abstract void DoEffectOn(Pawn user, IntVec3 cell);
    }
}
