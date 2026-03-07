using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public abstract class TrackedIncidentWorker : IncidentWorker
    {
        protected abstract bool TryExecuteWorkerSub(IncidentParms parms);

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool state = TryExecuteWorkerSub(parms);
            if (state)
            {
                if (parms.target is Map map)
                {
                    map.GetComponent<MapComponent_Tracker>().IncidentWatcher.NotifyActivated(def);
                }
            }
            return state;
        }
    }

    public class IncidentWatcher : TrackerComponent
    {
        private Map map;
        private Dictionary<IncidentDef, TimeKeeper> lastActivated = new Dictionary<IncidentDef, TimeKeeper>();
        private TimeKeeper cooldownTimer;

        public bool CooledDown => cooldownTimer.Expired;
        //public override int NextTick => !lastActivated.EnumerableNullOrEmpty() ? lastActivated.Min(entry => entry.Value.ticks) : 0;

        public IncidentWatcher(Map map)
        {
            this.map = map;
            this.lastActivated = new Dictionary<IncidentDef, TimeKeeper>();
            this.cooldownTimer = new TimeKeeper();
        }

        public override void Init()
        {
            if (lastActivated == null)
            {
                lastActivated = new Dictionary<IncidentDef, TimeKeeper>();
            }

            foreach (IncidentDef def in DefDatabase<IncidentDef>.AllDefsListForReading.Where(def => typeof(TrackedIncidentWorker).IsAssignableFrom(def.workerClass) && def.HasModExtension<TrackedIncidentExtension>()))
            {
                if (!lastActivated.ContainsKey(def))
                {
                    lastActivated[def] = new TimeKeeper();
                    NotifyActivated(def); //Start counting from 'now' if we're initializing.
                }
            }
        }

        public override void Tick()
        {
            if (!CooledDown)
            {
                return;
            }
            
            //Select an incident and trigger it. (Sort the relevant incidents by their chance, and find one to trigger that can be triggered.
            bool foundAnEventToTrigger = false;
            foreach ((IncidentDef incident, TimeKeeper timeKeeper) in lastActivated) {
                if (timeKeeper.Expired) {


                    var incidentParms = StorytellerUtility.DefaultParmsNow(incident.category, map);
                    if (incident.Worker.CanFireNow(incidentParms
                            )) {
                        if (incident.Worker.TryExecute(incidentParms))
                        {
                            TrackedIncidentExtension ext = incident.GetModExtension<TrackedIncidentExtension>();
                            cooldownTimer.Update(ext.CooldownTicks);
                            foundAnEventToTrigger = true;
                            break;
                        }
                    }
                }
            }

            if (!foundAnEventToTrigger) {
                //If an incident isn't found try again in roughly 1.25 - 2 in game hours
                cooldownTimer.Update(Rand.Range(3000,5000));
            }
        }

        public void NotifyActivated(IncidentDef def)
        {
            if (def.HasModExtension<TrackedIncidentExtension>())
            {
                TrackedIncidentExtension ext = def.GetModExtension<TrackedIncidentExtension>();

                if (!lastActivated.ContainsKey(def))
                {
                    lastActivated[def] = new TimeKeeper();
                }
                lastActivated[def].Update(ext.RefireTicks);
            }
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_Collections.Look(ref lastActivated, "lastActivated", LookMode.Def, LookMode.Deep);
            Scribe_Deep.Look(ref cooldownTimer, "cooldown");

            if (lastActivated == null)
            {
                lastActivated = new Dictionary<IncidentDef, TimeKeeper>();
            }

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Init();
            }
        }
    }
}
