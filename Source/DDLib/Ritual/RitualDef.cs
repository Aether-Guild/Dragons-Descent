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
}
