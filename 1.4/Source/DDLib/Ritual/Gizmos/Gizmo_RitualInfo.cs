using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DD
{
    [StaticConstructorOnStartup]
    public class Gizmo_RitualInfo : Gizmo
    {
        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public RitualTracker rituals;

        public Gizmo_RitualInfo(Map map)
        {
            rituals = map.GetComponent<MapComponent_Tracker>().Rituals;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        //public override bool GroupsWith(Gizmo other)
        //{
        //    return other is Gizmo_RitualInfo;
        //}
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect insetRect = rect.ContractedBy(6f);

            Widgets.DrawWindowBackground(rect);

            if (rituals != null)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(insetRect, "Favor");

                Rect barRect = insetRect;
                barRect.yMin = insetRect.y + insetRect.height / 2f;

                float fillPercent = rituals.Current / rituals.Max;
                Widgets.FillableBar(barRect, fillPercent, FullBarTex, EmptyBarTex, doBorder: false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(barRect, (rituals.Current).ToString("F0") + " / " + (rituals.Max).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(insetRect, "Rituals\nUnsupported");
                Text.Anchor = TextAnchor.UpperLeft;
            }

            return new GizmoResult(GizmoState.Clear);
        }
    }
}
