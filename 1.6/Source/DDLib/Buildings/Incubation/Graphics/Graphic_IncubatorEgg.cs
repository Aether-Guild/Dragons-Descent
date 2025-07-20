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
    public class Graphic_IncubatorEgg : Graphic
    {
        public Graphic[] layers;

        public override void Init(GraphicRequest req)
        {
            data = req.graphicData;
            path = req.path;
            color = req.color;
            colorTwo = req.colorTwo;
            drawSize = req.drawSize;

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
            CompEggIncubator comp = thing.TryGetComp<CompEggIncubator>();
            if(comp == null)
            {
                //Error. Doesn't have an egg incubator comp.
                return;
            }

            if(data is IncubatorGraphicData idata)
            {
                if(!idata.drawRotations.Contains(rot))
                {
                    //with this rotation, egg shouldn't be visible.
                    return;
                }

                extraRotation += idata.eggRotation;
            }

            Thing egg = comp.Egg;
            if(egg == null)
            {
                //No eggs to draw.
                return;
            }
            else
            {
                //Draw the egg.
                egg.Graphic.Draw(loc + data.drawOffset, rot, thing, extraRotation);
            }
        }
    }
}
