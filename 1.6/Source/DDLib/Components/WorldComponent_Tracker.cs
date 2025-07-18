using RimWorld.Planet;
using Verse;

namespace DD
{

    public class WorldComponent_Tracker : WorldComponent
    {

        private float current;
        public float Current { get => current; set => current = value; }

        public WorldComponent_Tracker(World world) : base(world) { }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref current, "current", 0);
        }
    }
}