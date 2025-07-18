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
    public class Graphic_FlickerEffect : Graphic_Collection
    {
        private const int BaseTicksPerFrameChange = 15;

        private const int ExtraTicksPerFrameChange = 10;

        private const float MaxOffset = 0.05f;
       // public override Material MatSingle => subGraphics[(Find.TickManager.TicksGame / 10) % subGraphics.Length].MatSingle;

        public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            if (thingDef == null)
            {
                Log.ErrorOnce("Fire DrawWorker with null thingDef: " + loc, 3427324);
                return;
            }
            if (subGraphics == null)
            {
                Log.ErrorOnce("Graphic_Flicker has no subgraphics " + thingDef, 358773632);
                return;
            }
            int num = Find.TickManager.TicksGame;
            if (thing != null)
            {
                num += Mathf.Abs(thing.thingIDNumber ^ 0x80FD52);
            }
            int num2 = num / 15;
            int num3 = Mathf.Abs(num2 ^ ((thing?.thingIDNumber ?? 0) * 391)) % subGraphics.Length;
            float num4 = 1f;
            CompFireOverlayBase compFireOverlayBase = null;
            if (thing is DD_Fire fire)
            {
                num4 = fire.fireSize;
            }
            else if (thing != null)
            {
                compFireOverlayBase = thing.TryGetComp<CompFireOverlayBase>();
                if (compFireOverlayBase != null)
                {
                    num4 = compFireOverlayBase.FireSize;
                }
                else
                {
                    compFireOverlayBase = thing.TryGetComp<CompDarklightOverlay>();
                    if (compFireOverlayBase != null)
                    {
                        num4 = compFireOverlayBase.FireSize;
                    }
                }
            }
            if (num3 < 0 || num3 >= subGraphics.Length)
            {
                Log.ErrorOnce("Fire drawing out of range: " + num3, 7453435);
                num3 = 0;
            }
            Graphic graphic = subGraphics[num3];
            float num5 = ((compFireOverlayBase == null) ? Mathf.Min(num4 / 1.2f, 1.2f) : num4);
            Vector3 vector = GenRadial.RadialPattern[num2 % GenRadial.RadialPattern.Length].ToVector3() / GenRadial.MaxRadialPatternRadius;
            vector *= 0.05f;
            Vector3 pos = loc + vector * num4;
            if (compFireOverlayBase != null)
            {
                pos += compFireOverlayBase.Props.offset;
            }
            Vector3 s = new Vector3(num5, 1f, num5);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, graphic.MatSingle, 0);
        }

        public override string ToString()
        {
            return "Flicker(subGraphic[0]=" + subGraphics[0].ToString() + ", count=" + subGraphics.Length + ")";
        }
    }
}
