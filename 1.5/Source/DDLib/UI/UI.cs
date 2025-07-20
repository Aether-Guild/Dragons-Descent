using HarmonyLib;
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
    public class UIDef : Def
    {
        public UIComponent main;
    }

    public abstract class UIComponent
    {
        public bool drawBackground = true;

        public float padding = 1f;

        public void DoUI(Rect rect)
        {
            if (drawBackground)
            {
                Widgets.DrawWindowBackground(rect);
            }
            DoCompUI(rect.ContractedBy(padding));
        }

        public abstract void Reset();

        protected abstract void DoCompUI(Rect rect);
    }

    public abstract class UIContainer : UIComponent
    {
        public List<UIComponent> layoutComps;
        public UILayoutManager layoutManager;

        protected override void DoCompUI(Rect rect)
        {
            if (layoutManager != null && !layoutComps.NullOrEmpty())
            {
                foreach (Pair<UIComponent, Rect> comp in layoutManager.DoLayout(layoutComps, rect))
                {
                    comp.First.DoUI(comp.Second);
                }
            }
        }

        public override void Reset()
        {
            foreach (UIComponent comp in layoutComps)
            {
                comp.Reset();
            }
        }
    }

    public abstract class UILayoutManager
    {
        public abstract IEnumerable<Pair<UIComponent, Rect>> DoLayout(IEnumerable<UIComponent> comps, Rect rect);
    }


}
