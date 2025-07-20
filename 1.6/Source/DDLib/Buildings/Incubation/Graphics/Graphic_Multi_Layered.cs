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
    public class Graphic_Multi_Layered : Graphic_Multi
    {
        public Graphic[] layers;

        public override void Init(GraphicRequest req)
        {
            base.Init(req);

            if (req.graphicData is LayeredGraphicData ldata)
            {
                if (!ldata.dataLayers.NullOrEmpty())
                {
                    layers = ldata.dataLayers.Select(d => GraphicDatabase.Get(d.graphicClass, d.texPath, d.shaderType?.Shader ?? ShaderTypeDefOf.Cutout.Shader, d.drawSize, d.color, d.colorTwo, d, d.shaderParameters)).ToArray();
                }
            }
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            if (!layers.NullOrEmpty())
            {
                Vector3 iterLoc = loc;
                foreach (Graphic g in layers)
                {
                    iterLoc += Altitudes.AltIncVect;
                    g.DrawWorker(iterLoc, rot, thingDef, thing, extraRotation);
                }
            }
        }
    }
}
