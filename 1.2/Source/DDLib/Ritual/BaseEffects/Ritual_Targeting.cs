using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class Ritual_Targeting : Ritual
    {
        private Pawn target;

        public Pawn Target { get => target; set => target = value; }

        public override bool Active => false;
        public override TickerType TickerType => TickerType.Never;

        public override bool IsReady => target != null;

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
}
