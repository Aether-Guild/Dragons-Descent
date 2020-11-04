using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class Ritual_TargetingTicking : Ritual_Targeting, ITickingRitual
    {
        private TimeKeeper timer = new TimeKeeper();
        private bool active = false;

        public override int Duration => Def.GetModExtension<RitualTickingModExtension>().GetDuration(ActivationCount);
        public override int DurationRemaining => timer.Remaining;

        public override bool Active => active;
        public override TickerType TickerType => Def.GetModExtension<RitualTickingModExtension>()?.tickerType ?? TickerType.Normal;

        public IEnumerable<Pawn> AllTargetedPawns => throw new NotSupportedException();

        public override bool IsReady => true; //Allow activation without needing targets in range.

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
}
