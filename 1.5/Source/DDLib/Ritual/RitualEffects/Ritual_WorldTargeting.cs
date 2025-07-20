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
    public abstract class Ritual_WorldTargeting : Ritual, IWorldTargetRitual
    {
        private WorldObject worldObj;

        public WorldObject WorldTarget { get => worldObj; set => worldObj = value; }

        public override Faction TargetedFaction => worldObj.Faction;

        public override bool IsReady => base.IsReady && worldObj != null;

        public override void DoActivation()
        {
            ApplyRitual(worldObj);
            Deactivate();
        }
        public override void DoDeactivation() { }

        public abstract void ApplyRitual(WorldObject target);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref worldObj, "worldObj");
        }

        public override void Reset(bool resetActivationCount = false)
        {
            base.Reset(resetActivationCount);
            worldObj = null;
        }
    }
}
