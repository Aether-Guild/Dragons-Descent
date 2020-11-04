using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public interface ITickingRitual
    {
        Pawn Target { get; }
        IEnumerable<Pawn> AllTargetedPawns { get; }
    }
}
