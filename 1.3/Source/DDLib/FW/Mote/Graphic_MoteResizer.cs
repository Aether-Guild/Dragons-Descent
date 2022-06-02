using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    [StaticConstructorOnStartup]
    public class Graphic_MoteResizer : Graphic_Single
    {
        protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            Mote_ParticleController moteController = (Mote_ParticleController)thing;
            float alpha = moteController.Alpha;

            if (alpha > 0f)
            {
                Color color = Color * moteController.instanceColor;
                color.a *= alpha;

                Vector3 exactScale = moteController.exactScale;
                exactScale.x *= data.drawSize.x;
                exactScale.z *= data.drawSize.y;

                Material matSingle = MatSingle;

                Matrix4x4 matrix = new Matrix4x4();

                foreach (SubMote_Particle mote in moteController.SubMotes)
                {
                    Color c = color;
                    c.a *= mote.Alpha;

                    Vector3 scale = exactScale;
                    scale.x *= mote.scaleDelta;
                    scale.z *= mote.scaleDelta;

                    matrix.SetTRS(mote.current, Quaternion.AngleAxis(mote.RotationAngle, Vector3.up), scale);

                    propertyBlock.SetColor(ShaderPropertyIDs.Color, c);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0, null, 0, propertyBlock);
                }
            }
        }

        public override string ToString()
        {
            return "MoteResizer(path=" + path + ", shader=" + Shader + ", color=" + color + ", colorTwo=unsupported)";
        }
    }
}
