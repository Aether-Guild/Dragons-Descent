using RimWorld;
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
    public class RitualTracker : TrackerComponent, IEnumerable<Ritual>
    {
        private Map map;
        private float current = 0;
        private int tick = 0;

        private Dictionary<string, Ritual> rituals = new Dictionary<string, Ritual>();

        public float Current { get => current; set => current = Mathf.Clamp(value, 0, Max); }

        public float Max => rituals.EnumerableNullOrEmpty() ? 1f : rituals.Values.Max(r => r.Cost);

        public bool Usable => !rituals.EnumerableNullOrEmpty() && rituals.Values.Any(r => Current >= r.Cost);
        public bool Full => Current >= Max;

        public IEnumerable<Ritual> AllRituals => rituals.Values.Where(r => r != null);
        public IEnumerable<ITickingRitual> AllActiveRituals => AllRituals.OfType<ITickingRitual>().Where(r => r.Active);
        public IEnumerable<ITickingRitual> AllTickingRituals => AllActiveRituals.Where(r => r is ITickingRitual).OfType<ITickingRitual>();
        public IEnumerable<ITickingRitual> AllNormalTickingRituals => AllActiveRituals.Where(r => r.TickerType == TickerType.Normal);
        public IEnumerable<ITickingRitual> AllRareTickingRituals => AllActiveRituals.Where(r => r.TickerType == TickerType.Rare);
        public IEnumerable<ITickingRitual> AllLongTickingRituals => AllActiveRituals.Where(r => r.TickerType == TickerType.Long);

        public RitualTracker(Map map)
        {
            this.map = map;
        }

        public bool CanActivate(RitualDef def) => this[def].CanActivate(current);

        public void Activate(RitualDef def)
        {
            if (!this[def].IsReady)
            {
                Messages.Message("RitualNoTargetMessage".Translate(), MessageTypeDefOf.CautionInput);
                return;
            }

            if (this[def].CanActivate(current))
            {
                this[def].Activate();
                Current -= this[def].Cost;
            }
        }

        public void Deactivate(RitualDef def)
        {
            if (this[def] is ITickingRitual r && r.Active)
            {
                this[def].Deactivate();
            }
        }

        public Ritual this[RitualDef def] => GetRitual(def);

        public Ritual GetRitual(RitualDef def)
        {
            if (!rituals.ContainsKey(def.defName) || rituals[def.defName] == null)
            {
                Ritual ritual = (Ritual)Activator.CreateInstance(def.ritualClass);
                ritual.Setup(map, def);
                rituals[def.defName] = ritual;
            }
            return rituals[def.defName];
        }

        public override void Tick()
        {
            tick++;

            foreach (ITickingRitual ritual in AllNormalTickingRituals)
            {
                ritual.Tick();
            }

            if (tick % GenTicks.TickLongInterval == 0)
            {
                foreach (ITickingRitual ritual in AllRareTickingRituals)
                {
                    ritual.TickRare();
                }
            }

            if (tick % GenTicks.TickLongInterval == 0)
            {
                foreach (ITickingRitual ritual in AllLongTickingRituals)
                {
                    ritual.TickLong();
                }
            }

            if (tick >= GenTicks.TickLongInterval)
            {
                //Cleanup: Discard null entries.
                rituals.RemoveAll(e => e.Key == null || e.Value == null);
                tick = 0;
            }
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_Values.Look(ref current, "current", 0);
            Scribe_Values.Look(ref tick, "tick", 0);
            Scribe_Collections.Look(ref rituals, "rituals", LookMode.Value, LookMode.Deep);

            if (rituals == null)
            {
                rituals = new Dictionary<string, Ritual>();
            }
            rituals.RemoveAll(e => e.Key == null || e.Value == null);
        }

        public IEnumerator<Ritual> GetEnumerator() => rituals.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => rituals.Values.GetEnumerator();
    }
}
