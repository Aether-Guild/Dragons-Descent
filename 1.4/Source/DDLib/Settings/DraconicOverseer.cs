using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public static class DraconicOverseer
    {
        private static DraconicMod mod;
        private static DraconicSettings settings;

        public static DraconicMod Mod
        {
            get
            {
                if (mod == null)
                {
                    mod = LoadedModManager.GetMod<DraconicMod>();
                }
                return mod;
            }
        }
        public static DraconicSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = Mod.GetSettings<DraconicSettings>();
                }
                return settings;
            }
        }
    }
}
