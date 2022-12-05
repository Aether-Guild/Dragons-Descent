using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    [StaticConstructorOnStartup]
    public class Dialog_AbilityBaseInfo : Window
    {
        public static readonly Texture2D TextureSearch = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/OpenInspector");

        public const float Padding = 10f;
        public const float InnerPadding = 5f;
        public const float EntryPadding = 5f;

        public const float LineMargin = 10f;

        public const float TextHeight = 20f;
        public const float TextLabelWidth = 90f;
        public const float TextAreaHeight = TextHeight * 3;

        public const float WidthMaster = 300f;

        public const int BorderThickness = 1;

        private static Vector2 Size_Icon_Search = new Vector2(20f, 20f);
        private static Vector2 Size_Icon_Check = new Vector2(20f, 20f);
        private static Vector2 Size_Icon_Ability = new Vector2(120f, 120f);
        private static Vector2 Size_Icon_Pawn = new Vector2(20f, 20f);

        public override Vector2 InitialSize => new Vector2(1000f, 750f);

        private Vector2 Scroll_Position_Defs;
        private Vector2 Scroll_Position_Desc;
        private Vector2 Scroll_Position_Gain;
        private Vector2 Scroll_Position_Loss;
        private Vector2 Scroll_Position_Data;

        private AbilityDef abilityDef = null;
        private IEnumerable<Pawn> pawns;

        private string searchCriteria = "";

        private IEnumerable<Ability_Base> AllAbilities
        {
            get
            {
                IEnumerable<Ability_Base> abilities = pawns.SelectMany(pawn => pawn.abilities.abilities.OfType<Ability_Base>());
                if (!searchCriteria.NullOrEmpty())
                {
                    abilities = abilities.Where(ability => ability.def.LabelCap.RawText.ToLower().Contains(searchCriteria.ToLower()));
                }
                return abilities;
            }
        }

        private IEnumerable<AbilityDef> AbilityDefs => AllAbilities.Select(ability => ability.def).Distinct().OrderBy(def => def.LabelCap.RawText);
        private IEnumerable<Ability_Base> GetAbilities(AbilityDef def) => AllAbilities.Where(ability => ability.def == def);
        private IEnumerable<Pawn> GetPawnsWithAbility(AbilityDef def) => pawns.Where(pawn => pawn.abilities.GetAbility(def) != null);

        public Dialog_AbilityBaseInfo(IEnumerable<Pawn> pawns)
        {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;

            this.pawns = pawns;
        }

        public override void DoWindowContents(Rect inRect)
        {
            TextAnchor anchor = Text.Anchor;
            GameFont font = Text.Font;

            inRect = inRect.AtZero();
            inRect.height -= Widgets.CloseButtonSize * 1.75f;

            Rect Rect_Pane_Master = new Rect(inRect.x, inRect.y, WidthMaster, inRect.height);
            Rect Rect_Pane_Detail = new Rect(Rect_Pane_Master.x + Rect_Pane_Master.width, Rect_Pane_Master.y, inRect.width - Rect_Pane_Master.width, inRect.height);

            Rect Rect_Region_Search = new Rect(Rect_Pane_Master.x, Rect_Pane_Master.y, Rect_Pane_Master.width, Padding + Size_Icon_Search.y + Padding);
            Rect Rect_Region_List = new Rect(Rect_Pane_Master.x, Rect_Region_Search.yMax, Rect_Pane_Master.width, Rect_Pane_Master.height - Rect_Region_Search.height);

            //Search
            DoSearchBox(Rect_Region_Search);

            //AbilityDef List
            DoDefList(Rect_Region_List);

            //Ability Data
            if (abilityDef != null)
            {
                DoAbilityData(Rect_Pane_Detail);
            }

            Text.Anchor = anchor;
            Text.Font = font;
        }

        private void DoSearchBox(Rect inRect)
        {
            inRect = inRect.ContractedBy(Padding);

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Tiny;

            Rect Rect_Icon_Search = new Rect(inRect.position, Size_Icon_Search);

            Widgets.DrawTextureFitted(Rect_Icon_Search, TextureSearch, 1f);
            searchCriteria = Widgets.TextField(new Rect(new Vector2(Rect_Icon_Search.xMax + InnerPadding, Rect_Icon_Search.y), new Vector2(inRect.width - (Rect_Icon_Search.xMax + InnerPadding), TextHeight)), searchCriteria);
        }

        public void DoDefList(Rect inRect)
        {
            IEnumerable<AbilityDef> defs = AbilityDefs;

            foreach(Pawn p in pawns)
            {
                defs = defs.Concat(defs);
            }

            inRect = inRect.ContractedBy(Padding);

            Widgets.DrawRectFast(inRect, Color.black);
            Widgets.DrawBox(inRect, BorderThickness);

            inRect = inRect.ContractedBy(BorderThickness + 1);

            Vector2 Size_Entry_Def = new Vector2(inRect.width, Padding + TextHeight + Padding);

            Rect Rect_List_Defs = new Rect(inRect.position, new Vector2(inRect.width, Size_Entry_Def.y * defs.Count()));

            Vector2 pos = new Vector2(inRect.x, inRect.y);

            if(Rect_List_Defs.height >= inRect.height)
            {
                Size_Entry_Def.x -= GUI.skin.verticalScrollbar.fixedWidth + 2f;
                Rect_List_Defs.width -= GUI.skin.verticalScrollbar.fixedWidth + 2f;
            }

            Widgets.BeginScrollView(inRect, ref Scroll_Position_Defs, Rect_List_Defs);

            foreach (AbilityDef def in defs)
            {
                DoDefEntry(new Rect(pos, Size_Entry_Def), def);

                // Calculate next entry
                pos.y += Size_Entry_Def.y;
            }

            Widgets.EndScrollView();
        }

        public void DoDefEntry(Rect inRect, AbilityDef def)
        {
            IEnumerable<Ability_Base> abilities = GetAbilities(def);
            inRect = inRect.ContractedBy(EntryPadding);

            if (Widgets.ButtonTextSubtle(inRect, (def.LabelCap + def.LabelCap + def.LabelCap + def.LabelCap + def.LabelCap + def.LabelCap + def.LabelCap).Truncate(inRect.width - InnerPadding - Size_Icon_Check.x), textLeftMargin: InnerPadding))
            {
                abilityDef = def;
            }

            if (def.comps.Any(comp => comp.compClass == typeof(AbilityComp_AbilityControl)) && abilities.All(ability => ability.AbilityControl.Controllable))
            {
                Texture2D texture;

                if (abilities.All(ability => ability.AbilityControl.Status) || abilities.All(ability => !ability.AbilityControl.Status))
                {
                    //All true or all false
                    if (abilities.First().AbilityControl.Status)
                    {
                        //True
                        texture = Widgets.CheckboxOnTex;
                    }
                    else
                    {
                        //False
                        texture = Widgets.CheckboxOffTex;
                    }
                }
                else
                {
                    //Mixed
                    texture = Widgets.CheckboxPartialTex;
                }

                Rect r = inRect.RightPartPixels(InnerPadding + Size_Icon_Check.x);
                r.height = TextHeight;
                Widgets.DrawTextureFitted(r.CenteredOnYIn(inRect), texture, 1f);
            }

            if (def == abilityDef)
            {
                Widgets.DrawHighlight(inRect);
            }
        }

        private void DoAbilityData(Rect inRect)
        {
            IEnumerable<Pawn> pawns = GetPawnsWithAbility(abilityDef);

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Tiny;

            inRect = inRect.ContractedBy(Padding);

            Widgets.DrawBox(inRect, BorderThickness);
            inRect = inRect.ContractedBy(BorderThickness + 1f);

            Rect topRect = inRect.TopPartPixels(TextHeight + TextAreaHeight + TextHeight + (Padding * 3));
            Rect midRect = inRect.BottomPartPixels(inRect.height - topRect.height);
            Rect lowRect = midRect.BottomPartPixels(midRect.height - (TextAreaHeight * 3) - (Padding * 3));
            midRect.height = inRect.height - topRect.height - lowRect.height;

            DoAbilityInfo(topRect);
            Widgets.DrawLine(new Vector2(topRect.xMin + LineMargin, topRect.yMax), new Vector2(topRect.xMax - LineMargin, topRect.yMax), Color.white, BorderThickness);
            DoAbilityConditions(midRect, pawns);
            Widgets.DrawLine(new Vector2(midRect.xMin + LineMargin, midRect.yMax), new Vector2(midRect.xMax - LineMargin, midRect.yMax), Color.white, BorderThickness);
            DoAbilityPawns(lowRect, pawns);
        }

        private void DoAbilityInfo(Rect inRect)
        {
            inRect = inRect.ContractedBy(Padding);

            Rect dataRect = inRect;
            if(abilityDef.uiIcon != null)
            {
                dataRect = inRect.LeftPartPixels(inRect.width - Size_Icon_Ability.y);
                Rect iconRect = inRect.RightPartPixels(inRect.height);

                Widgets.DrawBoxSolid(iconRect, Color.grey);
                Widgets.DrawBox(iconRect, BorderThickness);

                iconRect = iconRect.ContractedBy(BorderThickness + 1);

                Widgets.DrawTextureFitted(iconRect, abilityDef.uiIcon, 1f);
            }

            Rect rect = new Rect(dataRect);

            rect.height = TextHeight;
            DoLabeledField(rect, "AbilitySetting_Label_Name".Translate(), abilityDef.LabelCap);
            rect.y += TextHeight;

            rect.height = TextAreaHeight;
            DoLabeledAreaField(rect, "AbilitySetting_Label_Desc".Translate(), abilityDef.description, ref Scroll_Position_Desc);
            rect.y += TextAreaHeight;

            rect.width /= 2;
            rect.height = TextHeight;
            if(abilityDef.verbProperties != null)
            {
                DoLabeledField(rect, "AbilitySetting_Label_Range".Translate(), "AbilitySetting_Range_Format".Translate(abilityDef.verbProperties.minRange.Named("MIN"), abilityDef.verbProperties.range.Named("MAX")));
            }

            rect.x += rect.width;
            if(abilityDef.comps.OfType<AbilityCompProperties_AbilityControl>().Any())
            {
                DoLabeledCheckField(rect, "AbilitySetting_Label_AIUse".Translate(), abilityDef.comps.OfType<AbilityCompProperties_AbilityControl>().Any(comp => comp.autoUse));
            }
        }

        private void DoAbilityConditions(Rect inRect, IEnumerable<Pawn> pawns)
        {
            inRect = inRect.ContractedBy(Padding);

            string gainConditions = "";
            //IEnumerable<AbilityDefinitionEntry> entries = pawns.Select(pawn => GetAbilityDefinition(pawn, abilityDef))

            //DoLabeledAreaField(inRect.TopHalf(), "AbilitySetting_Label_Gain".Translate(), )
        }

        //private AbilityDefinitionEntry GetAbilityDefinition(Pawn pawn, AbilityDef def)
        //{
        //    CompAbilityDefinition comp = pawn.GetComp<CompAbilityDefinition>();
        //    if (comp != null)
        //    {
        //        comp.Definitions.First(entry => entry.abilityDef == def);
        //    }
        //    pawn.def.modExtensions.OfType<AbilityDefinitionExtension>().Where(ext => ext.abilities.)
        //}

        private void DoAbilityPawns(Rect inRect, IEnumerable<Pawn> pawns)
        {
            Widgets.DrawBoxSolid(inRect, Color.blue);
        }

        private void DoLabeledField(Rect inRect, string label, string value)
        {
            Rect labelRect = new Rect(inRect.x, inRect.y, TextLabelWidth, inRect.height);
            Widgets.Label(labelRect, label);

            Rect fieldRect = new Rect(inRect.x + labelRect.width, inRect.y, inRect.width - labelRect.width, inRect.height);
            Widgets.DrawBoxSolid(fieldRect, Color.black);
            Widgets.DrawBox(fieldRect, BorderThickness);
            fieldRect = fieldRect.ContractedBy(BorderThickness + 1f);

            Widgets.Label(fieldRect, value);
        }

        private void DoLabeledAreaField(Rect inRect, string label, string value, ref Vector2 scrollPosition)
        {
            Rect labelRect = new Rect(inRect.x, inRect.y, TextLabelWidth, inRect.height);
            Widgets.Label(labelRect, label);

            Rect fieldRect = new Rect(inRect.x + labelRect.width, inRect.y, inRect.width - labelRect.width, inRect.height);
            Widgets.DrawBoxSolid(fieldRect, Color.black);
            Widgets.DrawBox(fieldRect, BorderThickness);
            fieldRect = fieldRect.ContractedBy(BorderThickness + 1f);

            Widgets.LabelScrollable(fieldRect, value, ref scrollPosition);
        }

        private void DoLabeledCheckField(Rect inRect, string label, bool value)
        {
            Rect labelRect = new Rect(inRect.x, inRect.y, TextLabelWidth, inRect.height);
            Widgets.Label(labelRect, label);

            Rect rect = inRect.RightPartPixels(Size_Icon_Check.x);
            rect.x -= InnerPadding;

            Widgets.DrawTextureFitted(rect, value? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex, 1f);
        }
    }
}
