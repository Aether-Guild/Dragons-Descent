using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public interface ITickingRitual
    {

    }

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
            cooldown.Update(Cooldown);
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

    public abstract class Ritual_Targeting : Ritual
    {
        private Pawn target;

        public Pawn Target { get => target; set => target = value; }

        public override bool Active => false;
        public override TickerType TickerType => TickerType.Never;

        public override void DoActivation()
        {
            ApplyRitual(target);
            Deactivate();
        }
        public override void DoDeactivation() { }

        public abstract void ApplyRitual(Pawn target);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref target, "target");
        }
    }

    public abstract class Ritual_AoE : Ritual
    {
        private BodyDef bodyDef;

        public BodyDef TargetBodyDef { get => bodyDef; set => bodyDef = value; }
        public virtual IEnumerable<Pawn> AllTargetedPawns => Map.mapPawns.AllPawnsSpawned.Where(p => !p.DestroyedOrNull() && p.Map == Map).Where(pawn => (bodyDef == null && (pawn.RaceProps == null || pawn.RaceProps.body == null)) || (pawn.RaceProps != null && pawn.RaceProps.body == bodyDef));

        public override bool Active => false;
        public override TickerType TickerType => TickerType.Never;

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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref bodyDef, "targetBodyDef");
        }
    }

    public abstract class Ritual_TargetingTicking : Ritual_Targeting, ITickingRitual
    {
        private TimeKeeper timer = new TimeKeeper();
        private bool active = false;

        public override int Duration => Def.GetModExtension<RitualTickingModExtension>().GetDuration(ActivationCount);
        public override int DurationRemaining => timer.Remaining;

        public override bool Active => active;
        public override TickerType TickerType => Def.GetModExtension<RitualTickingModExtension>()?.tickerType ?? TickerType.Normal;

        public override bool CanActivate(float level) => timer.Expired && base.CanActivate(level);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref timer, "timer");
            Scribe_Values.Look(ref active, "active", false);
            if (timer == null)
            {
                timer = new TimeKeeper();
            }
        }

        public override void DoActivation()
        {
            active = true;
            timer.Update(Duration);
        }

        public override void DoDeactivation()
        {
            active = false;
        }

        public virtual void DoTick()
        {
            if (!timer.Expired && !Target.DestroyedOrNull() && Target.Map == Map)
            {
                ApplyRitual(Target);
            }
            else
            {
                if (Active)
                {
                    Deactivate();
                }
            }
        }

        public override void Tick()
        {
            if (TickerType == TickerType.Normal)
            {
                DoTick();
            }
        }

        public override void TickRare()
        {
            if (TickerType == TickerType.Rare)
            {
                DoTick();
            }
        }

        public override void TickLong()
        {
            if (TickerType == TickerType.Long)
            {
                DoTick();
            }
        }

        public override void Reset(bool resetActivationCount = false)
        {
            base.Reset(resetActivationCount);
            timer.Clear();
        }
    }

    public abstract class Ritual_AoETicking : Ritual_AoE, ITickingRitual
    {
        private TimeKeeper timer = new TimeKeeper();
        private bool active = false;

        public override int Duration => Def.GetModExtension<RitualTickingModExtension>().GetDuration(ActivationCount);
        public override int DurationRemaining => timer.Remaining;

        public override bool CanActivate(float level) => timer.Expired && base.CanActivate(level);

        public override bool Active => active;
        public override TickerType TickerType => Def.GetModExtension<RitualTickingModExtension>()?.tickerType ?? TickerType.Normal;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref timer, "timer");
            Scribe_Values.Look(ref active, "active", false);
            if (timer == null)
            {
                timer = new TimeKeeper();
            }
        }

        public override void DoActivation()
        {
            active = true;
            timer.Update(Duration);
        }

        public override void DoDeactivation()
        {
            active = false;
        }

        public virtual void DoTick()
        {
            if (!timer.Expired)
            {
                foreach (Pawn target in AllTargetedPawns)
                {
                    ApplyRitual(target);
                }
            }
            else
            {
                if (Active)
                {
                    Deactivate();
                }
            }
        }

        public override void Tick()
        {
            if(TickerType == TickerType.Normal)
            {
                DoTick();
            }
        }

        public override void TickRare()
        {
            if (TickerType == TickerType.Rare)
            {
                DoTick();
            }
        }

        public override void TickLong()
        {
            if (TickerType == TickerType.Long)
            {
                DoTick();
            }
        }

        public override void Reset(bool resetActivationCount = false)
        {
            base.Reset(resetActivationCount);
            timer.Clear();
        }
    }
}
