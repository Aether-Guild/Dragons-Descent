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
    public class Graphic_Animation : Graphic_Collection
    {
        public bool westFlipped, eastFlipped;
        public float drawRotatedAngleOffset;

        private int TPS = 0;
        private int TPS_Expire = 0;

        private const int DefaultTicksPerFrameChange = 15;

        public override bool WestFlipped => westFlipped;
        public override bool EastFlipped => eastFlipped;

        public override float DrawRotatedExtraAngleOffset => drawRotatedAngleOffset;

        private int TicksPerFrame
        {
            get
            {
                if (data is AnimationGraphicData adata)
                {
                    int currentTick = Find.TickManager.TicksGame;
                    if (currentTick >= TPS_Expire)
                    {
                        TPS = adata.ticksPerFrame.RandomInRange;
                        TPS_Expire = currentTick + TPS;
                    }

                    return TPS;
                }
                else
                {
                    return DefaultTicksPerFrameChange;
                }
            }
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            if (subGraphics == null || subGraphics.Length <= 0)
            {
                Log.ErrorOnce("Graphic_Animation has no frames " + thingDef, 358773632 + thing.thingIDNumber);
                return;
            }

            int frameNumber = Find.TickManager.TicksGame / TicksPerFrame;
            int frameIndex = Mathf.Abs(frameNumber + thing.thingIDNumber) % subGraphics.Length;
            subGraphics[frameIndex].DrawWorker(loc, rot, thingDef, thing, extraRotation);
            
            if(ShadowGraphic != null)
            {
                ShadowGraphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
            }
        }
    }
}
