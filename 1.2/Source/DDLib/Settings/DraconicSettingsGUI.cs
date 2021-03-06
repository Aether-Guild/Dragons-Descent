﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class DraconicSettingsGUI
    {
        private const string Format_WildSpawn_Title = "Setting_WildSpawn";
        private const string Format_WildSpawn_Label = "Setting_WildSpawn_Title";
        private const string Format_WildSpawn_Desc = "Setting_WildSpawn_Description";

        private const string Format_IncidentSpawn_Title = "Setting_EventSpawn";
        private const string Format_IncidentSpawn_Label = "Setting_EventSpawn_Title";
        private const string Format_IncidentSpawn_Desc = "Setting_EventSpawn_Description";

        private const string Format_Compat_KFM_IR_Label = "Setting_Compatibility_KFM_Range_Title";
        private const string Format_Compat_KFM_IR_Desc = "Setting_Compatibility_KFM_Range_Description";
        private const string Format_Compat_HFM_IR_Label = "Setting_Compatibility_HFM_Range_Title";
        private const string Format_Compat_HFM_IR_Desc = "Setting_Compatibility_HFM_Range_Description";

        private const string Format_Compat_Title = "Setting_Compatibility";
        private const string Format_Compat_Warning = "Setting_Compatibility_Warning";
        private const string Format_Compat_ARA_VC_Label = "Setting_Compatibility_ARA_VerbCheck_Title";
        private const string Format_Compat_ARA_VC_Desc = "Setting_Compatibility_ARA_VerbCheck_Description";

        private const string Format_Select = "Setting_Select";
        private const string Format_Deselect = "Setting_Deselect";
        private const string Format_Reset = "Setting_Reset";

        private const float TitleHeight = 32f;
        private const float GapSize = 12f;
        private const float EntryHeight = 24f;
        private const float TextEntryHeight = 20f + (1f/3f);
        private const float ButtonHeight = 32f;
        private const float FooterHeight = 18f;
        private const float BottomDelta = 32f;
        private const float GapSizeMini = 3f;

        [TweakValue("_DD.GUI.Delta")]
        private static float Delta = 0f;

        private DraconicSettings settings;

        public DraconicSettingsGUI(DraconicSettings settings)
        {
            this.settings = settings;
        }

        public void DoGUI(Rect inRect, ref Vector2 scrollPosition, ref Rect viewRect)
        {
            TextAnchor anchor = Text.Anchor;

            Rect windowRect = inRect.AtZero();
            windowRect.yMax -= BottomDelta;
            Listing_Standard window = new Listing_Standard();
            window.Begin(inRect);
            window.BeginScrollView(windowRect, ref scrollPosition, ref viewRect);
            DoGUI_WildSpawnPanel(window);
            window.Gap(GapSize);
            DoGUI_IncidentPanel(window);
            window.Gap(GapSize);
            DoGUI_CompatibilityPatchesPanel(window);
            window.Gap(GapSize);
            if (window.ButtonText(Format_Reset.Translate()))
            {
                settings.Reset();
            }
            window.EndScrollView(ref viewRect);
            window.End();

            Text.Anchor = anchor;
        }

        private void DoGUI_WildSpawnPanel(Listing_Standard window)
        {
            float panelHeight = TitleHeight + GapSize + (settings.WildSpawns.Count() * EntryHeight) + Delta;

            Listing_Standard panel = window.BeginSection(panelHeight);

            Listing_Standard titlePanel = panel.BeginSection(TitleHeight);
            titlePanel.ColumnWidth /= 3;
            Text.Font = GameFont.Medium;
            titlePanel.Label(Format_WildSpawn_Title.Translate());
            Text.Font = GameFont.Small;
            titlePanel.NewColumn();
            if (titlePanel.ButtonText(Format_Select.Translate()))
            {
                foreach (ThingDef def in settings.WildSpawns)
                {
                    settings.SetAllowedToSpawn(def, true);
                }
            }
            titlePanel.NewColumn();
            if (titlePanel.ButtonText(Format_Deselect.Translate()))
            {
                foreach (ThingDef def in settings.WildSpawns)
                {
                    settings.SetAllowedToSpawn(def, false);
                }
            }
            panel.EndSection(titlePanel);

            foreach (ThingDef def in settings.WildSpawns)
            {
                bool value = settings.IsAllowedToSpawn(def);
                bool updatedValue = value;

                panel.CheckboxLabeled(Format_WildSpawn_Label.Translate(def.label.CapitalizeFirst()), ref updatedValue, Format_WildSpawn_Desc.Translate(def.label.CapitalizeFirst()));

                if (updatedValue != value) {
                    settings.SetAllowedToSpawn(def, updatedValue);
                }
            }
            window.EndSection(panel);
        }

        private void DoGUI_IncidentPanel(Listing_Standard window)
        {
            float panelHeight = TitleHeight + GapSize + (settings.IncidentDefs.Count() * EntryHeight) + Delta;

            Listing_Standard panel = window.BeginSection(panelHeight);

            Listing_Standard titlePanel = panel.BeginSection(TitleHeight);
            titlePanel.ColumnWidth /= 3;
            Text.Font = GameFont.Medium;
            titlePanel.Label(Format_IncidentSpawn_Title.Translate());
            Text.Font = GameFont.Small;
            titlePanel.NewColumn();
            if (titlePanel.ButtonText(Format_Select.Translate()))
            {
                foreach (IncidentDef def in settings.IncidentDefs)
                {
                    settings.SetIncidentEnabled(def, true);
                }
            }
            titlePanel.NewColumn();
            if (titlePanel.ButtonText(Format_Deselect.Translate()))
            {
                foreach (IncidentDef def in settings.IncidentDefs)
                {
                    settings.SetIncidentEnabled(def, false);
                }
            }
            panel.EndSection(titlePanel);

            foreach (IncidentDef def in settings.IncidentDefs)
            {
                bool value = settings.IsIncidentEnabled(def);
                bool updatedValue = value;

                panel.CheckboxLabeled(Format_IncidentSpawn_Label.Translate(def.label.CapitalizeFirst()), ref updatedValue, Format_IncidentSpawn_Desc.Translate(def.label.CapitalizeFirst()));
                
                if(updatedValue != value)
                {
                    settings.SetIncidentEnabled(def, updatedValue);
                }
            }
            window.EndSection(panel);
        }

        private void DoGUI_CompatibilityPatchesPanel(Listing_Standard window)
        {
            float panelHeight = TitleHeight + GapSize + EntryHeight + EntryHeight + EntryHeight + FooterHeight + Delta;

            Listing_Standard panel = window.BeginSection(panelHeight);

            Listing_Standard titlePanel = panel.BeginSection(TitleHeight);
            titlePanel.ColumnWidth /= 3;
            Text.Font = GameFont.Medium;
            titlePanel.Label(Format_Compat_Title.Translate());
            Text.Font = GameFont.Small;
            titlePanel.NewColumn();
            if (titlePanel.ButtonText(Format_Select.Translate()))
            {
                settings.KFM_IgnoreRange = true;
                settings.HFM_IgnoreRange = true;
                settings.ARA_VerbCheck = true;
            }
            titlePanel.NewColumn();
            if (titlePanel.ButtonText(Format_Deselect.Translate()))
            {
                settings.KFM_IgnoreRange = false;
                settings.HFM_IgnoreRange = false;
                settings.ARA_VerbCheck = false;
            }
            panel.EndSection(titlePanel);

            bool KFM_IR = settings.KFM_IgnoreRange;
            bool HFM_IR = settings.HFM_IgnoreRange;
            bool ARA_VC = settings.ARA_VerbCheck;

            panel.CheckboxLabeled(Format_Compat_KFM_IR_Label.Translate(), ref KFM_IR, Format_Compat_KFM_IR_Desc.Translate());
            panel.CheckboxLabeled(Format_Compat_HFM_IR_Label.Translate(), ref HFM_IR, Format_Compat_HFM_IR_Desc.Translate());
            panel.CheckboxLabeled(Format_Compat_ARA_VC_Label.Translate(), ref ARA_VC, Format_Compat_ARA_VC_Desc.Translate());

            settings.KFM_IgnoreRange = KFM_IR;
            settings.HFM_IgnoreRange = HFM_IR;
            settings.ARA_VerbCheck = ARA_VC;

            Text.Font = GameFont.Tiny;
            panel.Label(Format_Compat_Warning.Translate());
            Text.Font = GameFont.Small;
            window.EndSection(panel);
        }
    }
}
