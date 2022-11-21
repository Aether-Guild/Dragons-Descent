using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class EffectTerrainCondition : EffectCondition
    {
        public List<TerrainDef> terrainDefs;

        public override bool ConditionIsSatisfied(EffectInfo effect, Thing thing) => false;
        public override bool ConditionIsSatisfied(EffectInfo effect, Map map, IntVec3 cell) => terrainDefs.Contains(cell.GetTerrain(map));
    }
}
