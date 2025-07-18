using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public interface IRitual
    {
        RitualDef Def { get; }
        Faction ActivatingFaction { get; set; }
        Faction TargetedFaction { get; }
    }

    public interface ISingleTargetRitual
    {
        TargetInfo Target { get; set; }
    }

    public interface IWorldTargetRitual
    {
        WorldObject WorldTarget { get; set; }
    }

    public interface IMultipleTargetRitual
    {
        Map TargetMap { get; set; }
        IEnumerable<TargetInfo> AllTargets { get; }
    }

    public interface ITickingRitual
    {
        int Duration { get; }
        int DurationRemaining { get; }
        float DurationPercent { get; }

        bool Active { get; }
        TickerType TickerType { get; }

        void Tick();
        void TickRare();
        void TickLong();
    }
}
