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

        public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            if (thingDef == null)
            {
                Log.ErrorOnce("Effect DrawWorker with null thingDef: " + loc, 3427324);
                return;
            }

            if (subGraphics == null)
            {
                Log.ErrorOnce("Graphic_FlickerEffect has no subgraphics " + thingDef, 358773632);
                return;
            }

            int tick = Find.TickManager.TicksGame;
            if (thing != null)
            {
                tick += Mathf.Abs(thing.thingIDNumber ^ 0x80FD52);
            }

            int num2 = tick / BaseTicksPerFrameChange;
            int index = Mathf.Abs(num2 ^ ((thing?.thingIDNumber ?? 0) * 391)) % subGraphics.Length;
            float size = 1f;

            Effect effect = thing as Effect;
            if (effect != null)
            {
                size = effect.CurrentSize();
            }

            if (index < 0 || index >= subGraphics.Length)
            {
                Log.ErrorOnce("Effect drawing out of range: " + index, 7453435);
                index = 0;
            }

            Graphic graphic = subGraphics[index];//.GetColoredVersion(Shader, new Color(Rand.ValueSeeded(thing.thingIDNumber), Rand.ValueSeeded(thing.thingIDNumber+1), Rand.ValueSeeded(thing.thingIDNumber)+2), new Color(Rand.ValueSeeded(thing.thingIDNumber+3), Rand.ValueSeeded(thing.thingIDNumber+4), Rand.ValueSeeded(thing.thingIDNumber+5)));
            float num5 = Mathf.Min(size / 1.2f, 1.2f);
            Vector3 a = GenRadial.RadialPattern[num2 % GenRadial.RadialPattern.Length].ToVector3() / GenRadial.MaxRadialPatternRadius;

            a *= MaxOffset;

            Vector3 pos = loc + a * size;
            Vector3 s = new Vector3(num5, 1f, num5);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, graphic.MatSingle, 0);
        }

        public override string ToString()
        {
            return "FlickerEffect(subGraphic[0]=" + subGraphics[0].ToString() + ", count=" + subGraphics.Length + ")";
        }
    }
}
