using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class Ritual_AoE : Ritual
    {
        public virtual IEnumerable<Pawn> AllTargetedPawns => Map.mapPawns.AllPawnsSpawned.Where(p => !p.DestroyedOrNull() && Def.targetingParams.CanTarget(p));

        public override bool Active => false;
        public override TickerType TickerType => TickerType.Never;

        public override bool IsReady => AllTargetedPawns.Any();

        public override void DoActivation()
        {
            foreach (Pawn pawn in AllTargetedPawns)
            {
                ApplyRitual(pawn);
            }
            Deactivate();
        }
        public override void DoDeactivation() { }

        public abstract void ApplyRitual(Pawn target);
    }
}
