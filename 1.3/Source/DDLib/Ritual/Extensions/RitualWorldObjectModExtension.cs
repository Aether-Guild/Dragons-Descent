using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class WorldObjectReplacmentEntry
    {
        public WorldObjectDef from, to;
    }

    public class RitualWorldObjectModExtension : DefModExtension
    {
        public bool defaultDestroy = true;
        public List<WorldObjectReplacmentEntry> worldObjects = new List<WorldObjectReplacmentEntry>();
    }
}
