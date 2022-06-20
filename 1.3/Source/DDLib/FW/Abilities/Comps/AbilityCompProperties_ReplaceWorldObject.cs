using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityCompProperties_ReplaceWorldObject : CompProperties_AbilityEffect
    {
        public int attackedGoodwillChange;
        public int otherGoodwillChange;

        public WorldObjectDef searchWorldObjectDef;
        public WorldObjectDef replacementWorldObjectDef;

        public DamageDef killDamageDef;
    }
}
