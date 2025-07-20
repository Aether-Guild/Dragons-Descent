//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Verse;
//using Verse.Sound;
//using VFECore.Abilities;

//namespace DD
//{
//    public class AbilityButtonGizmo : Gizmo
//    {
//        private readonly CompAbilities abilityComp;

//        public AbilityButtonGizmo(CompAbilities abilityComp)
//        {
//            this.abilityComp = abilityComp;
//        }

//        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)

//        {
//            GUI.DrawTexture(new Rect(topLeft.x, topLeft.y, 100f, 20f), TexUI.GrayTextBG);

//            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
//            {
//                foreach (VFECore.Abilities.AbilityDef abilityDef in DefDatabase<VFECore.Abilities.AbilityDef>.AllDefs)
//                {
//                    abilityComp.GiveAbility(abilityDef);
//                    DebugActionsUtility.DustPuffFrom(abilityComp.pawn);
//                }
//            }
//        }

//        public override float GetWidth(float maxWidth)
//        {
//            return 100f;
//        }
//    }
//}