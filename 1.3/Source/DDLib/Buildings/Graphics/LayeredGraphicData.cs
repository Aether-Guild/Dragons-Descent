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

    [HarmonyPatch(typeof(ThingWithComps), "Draw")]
    public static class DD_ThingWithComps_Draw
    {
        private static Dictionary<ThingWithComps, GraphicsLayerExtension> cachedExtensions = new Dictionary<ThingWithComps, GraphicsLayerExtension>();
        public static void Postfix(ref ThingWithComps __instance)
        {
            if (!cachedExtensions.TryGetValue(__instance, out var extension))
            {
                cachedExtensions[__instance] = __instance.def.GetModExtension<GraphicsLayerExtension>();
            }
            if (extension != null)
            {
                extension.Draw(__instance);
            }
        }

    }
}
