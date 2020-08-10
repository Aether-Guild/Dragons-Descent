using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class RitualDef : Def
    {
        public Type ritualClass;

        public SimpleCurve cost;
        public SimpleCurve cooldown;

        public FloatRange initialCooldown;

        public int InitialCooldown => GenTicks.SecondsToTicks(initialCooldown.RandomInRange);
    }

    public class RitualTickingModExtension : DefModExtension
    {
        public SimpleCurve duration;
        public TickerType tickerType;

        public int GetDuration(int activationCount) => GenTicks.SecondsToTicks(duration.Evaluate(activationCount));
    }

    public class RitualNeedsModExtension : DefModExtension
    {
        public FloatRange? food, rest, comfort, joy, mood, drugDesire;
    }

    public class RitualHediffModExtension : DefModExtension
    {
        public List<HediffDef> hediffs = new List<HediffDef>();
    }
}
