        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            GUI.DrawTexture(new Rect(topLeft.x, topLeft.y, 100f, 20f), TexUI.GrayTextBG);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                foreach (VEF.Abilities.AbilityDef abilityDef in DefDatabase<VEF.Abilities.AbilityDef>.AllDefs)
                {
                    abilityComp.GiveAbility(abilityDef);
                    DebugActionsUtility.DustPuffFrom(abilityComp.pawn);
                }
                return new GizmoResult(GizmoState.Interacted);
            }

            return new GizmoResult(GizmoState.Clear);
        }
