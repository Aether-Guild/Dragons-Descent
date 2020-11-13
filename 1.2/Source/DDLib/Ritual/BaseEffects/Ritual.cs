using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class Ritual : IExposable
    {
        private RitualDef def;
        private Map map;

        private int activationCount = 0;
        private TimeKeeper cooldown = new TimeKeeper();

        public int ActivationCount => activationCount;
        public bool CooledDown => cooldown.Expired;
        public int CooldownRemaining => cooldown.Remaining;
        public Map Map => map;

        public string Label => def.label;
        public string LabelCap => def.LabelCap;

        public RitualDef Def => def;

        public virtual void Setup(Map map, RitualDef def)
        {
            this.map = map;
            this.def = def;
            cooldown.Update(def.InitialCooldown);
        }

        public virtual float Cost => Def.cost.Evaluate(activationCount);
        public virtual int Cooldown => GenTicks.SecondsToTicks(Def.cooldown.Evaluate(activationCount));
        public abstract bool Active { get; }
        public abstract TickerType TickerType { get; }
        public virtual int Duration => 0;
        public virtual int DurationRemaining => Duration;

        public abstract void DoActivation();
        public abstract void DoDeactivation();

        protected virtual void PreActivation() { }
        protected virtual void PreDeactivation() { }

        protected virtual void PostActivation() { }
        protected virtual void PostDeactivation() { }

        public virtual void Tick() { }
        public virtual void TickRare() { }
        public virtual void TickLong() { }

        public virtual bool IsReady => true;

        public void Activate()
        {
            PreActivation();
            DoActivation();
            IncrementCount();
            PostActivation();
        }

        public void Deactivate()
        {
            PreActivation();
            DoDeactivation();
            cooldown.Update(Cooldown);
            PostDeactivation();
        }

        public void IncrementCount()
        {
            activationCount++;
        }

        public virtual bool CanActivate(float level) => !Active && CooledDown && level >= Cost;

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_Defs.Look(ref def, "def");
            Scribe_Deep.Look(ref cooldown, "cd", 0);
            Scribe_Values.Look(ref activationCount, "count", 0);
        }

        public virtual void Reset(bool resetActivationCount = false)
        {
            cooldown.Clear();
            if (resetActivationCount)
            {
                activationCount = 0;
            }
        }
    }
}
