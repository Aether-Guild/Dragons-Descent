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
    public abstract class Ritual_TargetingTicking : Ritual_Targeting, ITickingRitual
    {
        private TimeKeeper duration = new TimeKeeper();
        private bool active = false;

        public int Duration => Def.GetModExtension<RitualTickingModExtension>().GetDuration(ActivationCount);
        public int DurationRemaining => duration.Remaining;
        public float DurationPercent => duration.RemainingPercent;

        public bool Active => active;
        public TickerType TickerType => Def.GetModExtension<RitualTickingModExtension>()?.tickerType ?? TickerType.Normal;

        public override bool CanActivate(float level) => duration.Expired && !Active && base.CanActivate(level);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref duration, "duration");
            Scribe_Values.Look(ref active, "active", false);
            if (duration == null)
            {
                duration = new TimeKeeper();
            }
        }

        public override void DoActivation()
        {
            active = true;
            duration.Update(Duration);
        }

        public override void DoDeactivation()
        {
            active = false;
        }

        public virtual void DoTick()
        {
            if (duration.Expired && Active)
            {
                Deactivate();
                return;
            }

            if (!Target.IsValid)
            {
                return;
            }

            if (Target.HasThing)
            {
                if (Target.ThingDestroyed)
                {
                    return;
                }
            }
            else
            {
                if (Target.Map == null || Target.Map.Index < 0)
                {
                    return;
                }

                if (!Target.Cell.IsValid)
                {
                    return;
                }
            }

            ApplyRitual(Target);
            if (Def.moteOnTick != null && Target.HasThing)
            {
                MoteMaker.MakeAttachedOverlay(Target.Thing, Def.moteOnTick, Vector3.zero, Def.moteOnTickScale);
            }
            if (Def.fleckOnTick != null && Target.HasThing)
            {
                FleckMaker.AttachedOverlay(Target.Thing, Def.fleckOnTick, Vector3.zero, Def.moteOnTickScale);
            }
        }

        public void Tick()
        {
            if (TickerType == TickerType.Normal)
            {
                DoTick();
            }
        }

        public void TickRare()
        {
            if (TickerType == TickerType.Rare)
            {
                DoTick();
            }
        }

        public void TickLong()
        {
            if (TickerType == TickerType.Long)
            {
                DoTick();
            }
        }

        public override void Reset(bool resetActivationCount = false)
        {
            if (Active)
            {
                DoDeactivation();
            }
            base.Reset(resetActivationCount);
            duration.Clear();
        }
    }
}
