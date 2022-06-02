using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class Graphic_MultiAnimation : Graphic
    {
        private static string[] SubFolders = new string[] {
            "/north",
            "/east",
            "/south",
            "/west"
        };

        public Graphic_Animation[] animations;

        public override Material MatSingle => MatSouth;

        public override Material MatNorth => animations[Rot4.North.AsInt].MatSingle;
        public override Material MatEast => animations[Rot4.East.AsInt].MatSingle;
        public override Material MatSouth => animations[Rot4.South.AsInt].MatSingle;
        public override Material MatWest => animations[Rot4.West.AsInt].MatSingle;

        public override bool ShouldDrawRotated
        {
            get
            {
                if (data != null && !data.drawRotated)
                {
                    return false;
                }
                if (!(MatEast == MatNorth))
                {
                    return MatWest == MatNorth;
                }
                return true;
            }
        }

        private bool PathExists(string path)
        {
            return LoadedModManager.RunningMods.Any(mod => mod.GetContentHolder<Texture2D>().contentList.Any(entry => entry.Key.StartsWith(path)));
        }

        public override void Init(GraphicRequest req)
        {
            data = req.graphicData;
            if (req.path.NullOrEmpty())
            {
                throw new ArgumentNullException("folderPath");
            }
            if (req.shader == null)
            {
                throw new ArgumentNullException("shader");
            }

            path = req.path;
            color = req.color;
            colorTwo = req.colorTwo;
            drawSize = req.drawSize;

            animations = new Graphic_Animation[SubFolders.Length];
            for(int i = 0; i < animations.Length; i++)
            {
                string subPath = path + SubFolders[i];
                if(PathExists(subPath))
                {
                    animations[i] = GraphicDatabase.Get<Graphic_Animation>(subPath, req.shader, drawSize, Color, ColorTwo, data) as Graphic_Animation;
                }
            }

            if (animations[Rot4.North.AsInt] == null)
            {
                //If North is not set, try to set North to South, or East or West.
                animations[Rot4.North.AsInt] = (animations[Rot4.South.AsInt].GetCopy(drawSize) ?? animations[Rot4.East.AsInt].GetCopy(drawSize) ?? animations[Rot4.West.AsInt].GetCopy(drawSize)) as Graphic_Animation;
            }
            if (animations[Rot4.South.AsInt] == null)
            {
                //If South is not set, try to set South to North.
                animations[Rot4.South.AsInt] = (animations[Rot4.North.AsInt].GetCopy(drawSize)) as Graphic_Animation;
            }
            if (animations[Rot4.East.AsInt] == null)
            {
                //If East is not set, try to set East to West or North.
                animations[Rot4.East.AsInt] = (animations[Rot4.West.AsInt].GetCopy(drawSize) ?? animations[Rot4.North.AsInt].GetCopy(drawSize)) as Graphic_Animation;
            }
            if (animations[Rot4.West.AsInt] == null)
            {
                //If West is not set, try to set West to East or North.
                animations[Rot4.West.AsInt] = (animations[Rot4.East.AsInt].GetCopy(drawSize) ?? animations[Rot4.North.AsInt].GetCopy(drawSize)) as Graphic_Animation;
            }
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            if (animations == null || animations.Length <= 0)
            {
                Log.ErrorOnce("Graphic_MultiAnimation has no animations " + thingDef, 358773632 + thing.thingIDNumber);
                return;
            }

            animations[thing.Rotation.AsInt].DrawWorker(loc, rot, thingDef, thing, extraRotation);
        }
    }
}
