using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class GraphicLayerData
    {
        public GraphicData graphicData;
        public AltitudeLayer layer;

        public void Draw(Thing thing)
        {
            IntVec3 position = thing.Position;
            Rot4 rotation = thing.Rotation;
            IntVec2 size = thing.def.Size;

            graphicData.GraphicColoredFor(thing).Draw(GenThing.TrueCenter(position, rotation, size, layer.AltitudeFor()), rotation, thing);
        }
    }

    public class GraphicsLayerExtension : DefModExtension
    {
        public List<GraphicLayerData> layers;

        public void Draw(Thing thing)
        {
            foreach (GraphicLayerData layer in layers)
            {
                layer.Draw(thing);
            }
        }
    }

    [HarmonyPatch(typeof(Building), "Draw")]
    public static class DD_Building_Draw
    {
        public static void Postfix(ref Thing __instance)
        {
            if (__instance.def.HasModExtension<GraphicsLayerExtension>())
            {
                __instance.def.GetModExtension<GraphicsLayerExtension>().Draw(__instance);
            }
        }
    }
}
