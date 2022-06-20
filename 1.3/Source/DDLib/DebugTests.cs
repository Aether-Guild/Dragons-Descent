using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public static class DDDebugTests
    {
        public const int WildBiomeCount = 100;

        [DebugAction("DD.AutoTests", "BiomeSpawn Summary x100", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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

        [DebugAction("DD.Effects", "Try Apply Effect...", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ApplyEffect()
        {
            IEnumerable<DebugMenuOption> options = DefDatabase<EffectDef>.AllDefsListForReading.OrderBy(def => def.defName).Select(def => new DebugMenuOption(def.LabelCap, DebugMenuOptionMode.Tool, () => {
                IntVec3 cell = UI.MouseCell();
                Map map = Find.CurrentMap;

                List<TargetInfo> targets = new List<TargetInfo>()
                {
                    new TargetInfo(cell, map)
                };

                List<Thing> things = cell.GetThingList(map).ToList();
                if (!things.NullOrEmpty())
                {
                    targets.AddRange(things.OfType<TargetInfo>());
                }

                foreach(TargetInfo target in targets)
                {
                    EffectUtils.TryAffect(new EffectInfo(def, def.effectSize.Average), target);
                }
            }));

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }

        [DebugAction("DD.Effects", "Force Apply Effect...", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnEffect()
        {
            IEnumerable<DebugMenuOption> options = DefDatabase<EffectDef>.AllDefsListForReading.OrderBy(def => def.defName).Select(def => new DebugMenuOption(def.LabelCap, DebugMenuOptionMode.Tool, () => {
                IntVec3 cell = UI.MouseCell();
                Map map = Find.CurrentMap;

                List<TargetInfo> targets = new List<TargetInfo>()
                {
                    new TargetInfo(cell, map)
                };

                List<Thing> things = cell.GetThingList(map).ToList();
                if (!things.NullOrEmpty())
                {
                    targets.AddRange(things.OfType<TargetInfo>());
                }

                foreach (TargetInfo target in targets)
                {
                    EffectUtils.ApplyEffect(target, def, def.effectSize.Average);
                }
            }));

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }

        [DebugAction("DD.Effects", "Remove Effect...", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void RemoveEffect()
        {
            List<DebugMenuOption> options = new List<DebugMenuOption>()
            {
                new DebugMenuOption("All", DebugMenuOptionMode.Tool, () => {
                    List<Thing> things = UI.MouseCell().GetThingList(Find.CurrentMap);
                    if(!things.NullOrEmpty())
                    {
                        DefDatabase<EffectDef>.AllDefsListForReading.ForEach(def =>
                        {
                            List<Thing> targets = things.Where(thing => thing.def == def.thingDef).ToList();
                            targets.AddRange(things.Where(thing => thing.HasAttachment(def.thingDef)).Select(thing => thing.GetAttachment(def.thingDef)));
                            things.ForEach(e => e.Destroy());
                        });
                    }
                    
                })
            };

            options.AddRange(DefDatabase<EffectDef>.AllDefsListForReading.OrderBy(def => def.defName).Select(def => new DebugMenuOption(def.LabelCap, DebugMenuOptionMode.Tool, () =>
            {
                IntVec3 cell = UI.MouseCell();
                if (cell.IsValid)
                {
                    List<Thing> things = cell.GetThingList(Find.CurrentMap);
                    if (!things.NullOrEmpty())
                    {
                        List<Thing> targets = things.Where(thing => thing.def == def.thingDef).ToList();
                        targets.AddRange(things.Where(thing => thing.HasAttachment(def.thingDef)).Select(thing => thing.GetAttachment(def.thingDef)));

                        foreach (Thing target in targets)
                        {
                            target.Destroy();
                        }
                    }
                }
            })));

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }

        public class FloatMenuOption_Size : FloatMenuOption
        {
            private Thing thing;
            private Vector2 size;
            private string xbuf, ybuf;

            public FloatMenuOption_Size(Thing thing) : base(thing.LabelCap, () => { })
            {
                this.thing = thing;
                this.size = Vector2.zero;
            }

            public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
            {
                Widgets.DrawWindowBackground(rect);
                rect.width /= 3;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect, thing.LabelCap);
                rect.x += rect.width;
                Widgets.TextFieldNumeric(rect, ref size.x, ref xbuf, 0);
                rect.x += rect.width;
                Widgets.TextFieldNumeric(rect, ref size.y, ref ybuf, 0);
                if(size.x != 0 && size.y != 0)
                {
                    Resize(thing, size);
                }
                return false;
            }

            private static void Resize(Thing thing, Vector2 size)
            {
                Graphic g = thing.Graphic;
                
                g.drawSize = size;
                if (g is Graphic_MultiAnimation gma)
                {
                    thing.def.graphicData.drawSize = size;
                    gma.data.drawSize = size;
                    foreach (Graphic ga in gma.animations)
                    {
                        ga.drawSize = size;
                    }
                }

                if(thing is Pawn pawn)
                {
                    pawn.Drawer.renderer.graphics.nakedGraphic.drawSize = size;

                    if (pawn.Drawer.renderer.graphics.desiccatedHeadGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.desiccatedHeadGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.desiccatedHeadStumpGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.desiccatedHeadStumpGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.dessicatedGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.dessicatedGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.hairGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.hairGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.headGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.headGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.headStumpGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.headStumpGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.packGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.packGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.rottingGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.rottingGraphic.drawSize = size;
                    }
                    if (pawn.Drawer.renderer.graphics.skullGraphic != null)
                    {
                        pawn.Drawer.renderer.graphics.skullGraphic.drawSize = size;
                    }
                }

                if (thing is Corpse corpse)
                {
                    corpse.InnerPawn.Drawer.renderer.graphics.nakedGraphic.drawSize = size;

                    if (corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadStumpGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.desiccatedHeadStumpGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.dessicatedGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.dessicatedGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.hairGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.hairGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.headGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.headGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.headStumpGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.headStumpGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.packGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.packGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.rottingGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.rottingGraphic.drawSize = size;
                    }
                    if (corpse.InnerPawn.Drawer.renderer.graphics.skullGraphic != null)
                    {
                        corpse.InnerPawn.Drawer.renderer.graphics.skullGraphic.drawSize = size;
                    }
                }
            }
        }

        [DebugAction("DD.Texture", "Change DrawSize", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ChangeDrawSize()
        {
            IntVec3 cell = UI.MouseCell();
            Map map = Find.CurrentMap;

            if(cell.IsValid && cell.InBounds(map))
            {
                IEnumerable<Thing> things = GenRadial.RadialDistinctThingsAround(cell, map, 2f, true);
                if (!things.EnumerableNullOrEmpty())
                {
                    List<FloatMenuOption> options = things.Where(thing => thing.Graphic != null).Select(thing => (FloatMenuOption)new FloatMenuOption_Size(thing)).ToList();

                    Find.WindowStack.Add(new FloatMenu(options));
                }
            }
        }
    }
}
