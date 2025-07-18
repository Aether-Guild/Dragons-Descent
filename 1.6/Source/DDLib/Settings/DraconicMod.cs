using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class DraconicMod : Mod
    {
        DraconicSettings settings;
        private DraconicSettingsGUI gui;
        private Vector2 scrollPosition = Vector2.zero;
        private float totalHeight = 999999f;

        public DraconicMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<DraconicSettings>();
            gui = new DraconicSettingsGUI(settings);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            settings.Apply();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            gui.DoGUI(inRect, ref scrollPosition, ref totalHeight);
        }

        public override string SettingsCategory()
        {
            return "Settings_DragonCategory".Translate();
        }
    }
}
