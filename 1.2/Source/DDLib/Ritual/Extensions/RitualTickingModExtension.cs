using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class RitualTickingModExtension : DefModExtension
    {
        public SimpleCurve duration;
        public TickerType tickerType;

        public int GetDuration(int activationCount) => GenTicks.SecondsToTicks(duration.Evaluate(activationCount));
    }
}
