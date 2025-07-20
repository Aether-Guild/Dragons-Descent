using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class Ritual_MapTargeting : Ritual, IMultipleTargetRitual
    {
        private Map targetMap;

        public Map TargetMap { get => targetMap; set => targetMap = value; }

        public override Faction TargetedFaction => targetMap.ParentFaction;

        public virtual IEnumerable<TargetInfo> AllTargets => targetMap.mapPawns.AllPawnsSpawned.Where(p => !p.DestroyedOrNull() && Def.targetingParams.CanTarget(p)).Select(p => new TargetInfo(p));

        public override bool IsReady => base.IsReady && targetMap != null && targetMap.Index >= 0;

        public override void DoActivation()
        {
            foreach (TargetInfo target in AllTargets)
            {
                ApplyRitual(target);
            }
            Deactivate();
        }
        public override void DoDeactivation() { }

        public abstract void ApplyRitual(TargetInfo target);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref targetMap, "targetMap");
        }

        public override void Reset(bool resetActivationCount = false)
        {
            base.Reset(resetActivationCount);
            targetMap = null;
        }
    }
}
