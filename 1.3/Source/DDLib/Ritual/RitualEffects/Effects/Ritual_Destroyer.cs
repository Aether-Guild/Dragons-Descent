using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace DD
{
  /*   public class Ritual_Destroyer : Ritual_WorldTargeting
    {
        protected bool DefaultDestroy => Def.GetModExtension<RitualWorldObjectModExtension>() is RitualWorldObjectModExtension ext ? ext.defaultDestroy : true;

        protected WorldObjectDef GetReplacementDef(WorldObjectDef def)
        {
            if (Def.GetModExtension<RitualWorldObjectModExtension>() is RitualWorldObjectModExtension ext)
            {
                return ext.worldObjects.Where(entry => entry.from == def).Select(entry => entry.to).RandomElementWithFallback();
            }
            else
            {
                return null;
            }
        }

        public override void ApplyRitual(WorldObject target)
        {
            int tile = target.Tile;
            Faction faction = target.Faction;

            WorldObjectDef replacement = GetReplacementDef(target.def);
            if(replacement != null || DefaultDestroy)
            {
                //If replacement exists, or if (it doesn't and destroy by default)
                if(target is MapParent mp && mp.HasMap)
                {
                    //All pawns are killed, and map is destroyed.
                    mp.Map.mapPawns.AllPawnsSpawned.ToList().ForEach(p => p.Kill(null));
                    Current.Game.DeinitAndRemoveMap(mp.Map);
                }
                //Destroy world object.
                target.Destroy();
                if(replacement != null)
                {
                    //Spawn replacement, if it exists.
                    WorldObject t = WorldObjectMaker.MakeWorldObject(replacement);
                    t.Tile = tile;
                    t.SetFaction(faction);
                    Find.WorldObjects.Add(t);
                }
            }
        }
    } */
}
