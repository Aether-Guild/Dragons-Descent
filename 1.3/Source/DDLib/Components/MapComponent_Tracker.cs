using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public abstract class TrackerComponent : IExposable
    {
        public abstract void ExposeData();

        public virtual void Init() { }
        public virtual void Tick() { }

        public virtual int NextTick => Find.TickManager.TicksGame + 1;
    }

    public class MapComponent_Tracker : MapComponent
    {
        private RitualTracker rituals;
        private FerociousPawnTracker ferocious;
        private IncidentWatcher incidents;

        public MapComponent_Tracker(Map map) : base(map)
        {
            rituals = new RitualTracker(map);
            ferocious = new FerociousPawnTracker(map);
            incidents = new IncidentWatcher(map);
        }

        public RitualTracker Rituals => rituals;
        public FerociousPawnTracker Ferocious => ferocious;
        public IncidentWatcher IncidentWatcher => incidents;

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            rituals.Tick();
            ferocious.Tick();

            //Only process this in player's maps.
            if(map.ParentFaction != null && map.ParentFaction.IsPlayer)
            {
                if (incidents.CooledDown && incidents.NextTick <= Find.TickManager.TicksGame)
                {
                    incidents.Tick();
                }
            }
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            incidents.Init();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref rituals, "rituals", map);
            Scribe_Deep.Look(ref ferocious, "ferocious", map);
            Scribe_Deep.Look(ref incidents, "incidents", map);

            if (rituals == null)
            {
                rituals = new RitualTracker(map);
            }

            if (ferocious == null)
            {
                ferocious = new FerociousPawnTracker(map);
            }

            if(incidents == null)
            {
                incidents = new IncidentWatcher(map);
            }
        }
    }
}
