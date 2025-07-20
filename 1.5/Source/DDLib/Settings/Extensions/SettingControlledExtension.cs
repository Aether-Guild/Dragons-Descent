using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public abstract class SettingControlledExtension<D> : DefModExtension where D : Def
    {
        //Master flag.
        private bool Init = false;

        public void Enable(D def)
        {
            if (!Init)
            {
                Init = true;
                DoInit(def);
            }
            DoEnable(def);
            Refresh();
        }
        public void Disable(D def)
        {
            if (!Init)
            {
                Init = true;
                DoInit(def);
            }
            DoDisable(def);
            Refresh();
        }

        protected abstract void DoInit(D def);
        protected abstract void DoEnable(D def);
        protected abstract void DoDisable(D def);
        protected virtual void Refresh() { }
    }
}
