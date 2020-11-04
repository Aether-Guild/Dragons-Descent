using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public static class DDDebugTests
    {
        public const float WildBiomeCount = 1000f;

        [DebugAction("DD.AutoTests", "BiomeSpawn Summary x1000", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void BiomeSpawn1k()
        {
            Map map = Find.CurrentMap;
            IntVec3 cell = UI.MouseCell();

            Dictionary<ThingDef, float> histogram = new Dictionary<ThingDef, float>();

            if (cell.IsValid && !cell.Impassable(map))
            {
                WildAnimalSpawner spawner = map.wildAnimalSpawner;
                List<Thing> originalThings = map.spawnedThings.ToList();

                //Spawning
                Log.Message("Data captured, spawning begins.");
                for (int i = 0; i < WildBiomeCount; i++)
                {
                    if (!spawner.SpawnRandomWildAnimalAt(cell))
                    {
                        Log.Error("Failed at " + (i + 1));
                        break;
                    }
                }

                //Counting
                foreach (Thing thing in map.spawnedThings.Where(t => !originalThings.Contains(t)).ToList())
                {
                    if (histogram.ContainsKey(thing.def))
                    {
                        histogram[thing.def]++;
                    }
                    else
                    {
                        histogram.Add(thing.def, 1);
                    }

                    thing.Destroy();
                }
                int count = histogram.Values.Sum(val => (int)val);
                Log.Message("Successfully spawned " + count + " animals at " + cell);

                //Printing
                string text = "Spawn Results:\n\n";
                foreach (ThingDef def in histogram.Keys)
                {
                    text += def.LabelCap + "," + histogram[def] + "," + (histogram[def] / count).ToStringPercent() + "\n";
                }
                Log.Message(text.TrimEndNewlines());
            }
        }
    }
}
