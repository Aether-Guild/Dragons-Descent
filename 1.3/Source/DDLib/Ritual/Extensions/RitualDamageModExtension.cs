using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class RitualDamageModExtension : DefModExtension
    {
        public ThingDef bombardmentThingDef;
        public FloatRange bombardmentInterval = FloatRange.One;
        public FloatRange bombardmentRadius = new FloatRange(10f, 15);
    }
}
