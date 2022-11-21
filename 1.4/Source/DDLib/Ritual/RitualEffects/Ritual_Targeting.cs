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
    public abstract class Ritual_Targeting : Ritual, ISingleTargetRitual
    {
        private TargetInfo target;

        public override Faction TargetedFaction
        {
            get
            {
                if (target.HasThing && !target.ThingDestroyed)
                {
                    return target.Thing.Faction;
                }

                if (target.Map != null)
                {
                    return target.Map.ParentFaction;
                }

                return null;
            }
        }

        public TargetInfo Target { get => target; set => target = value; }

        public override bool IsReady => base.IsReady && target.IsValid;

        public override void DoActivation()
        {
            ApplyRitual(target);
            Deactivate();
        }
        public override void DoDeactivation() { }

        public abstract void ApplyRitual(TargetInfo target);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_TargetInfo.Look(ref target, "target");
        }

        public override void Reset(bool resetActivationCount = false)
        {
            base.Reset(resetActivationCount);
            target = null;
        }
    }
}
