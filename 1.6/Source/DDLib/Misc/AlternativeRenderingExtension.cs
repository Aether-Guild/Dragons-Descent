using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AlternativeRenderingExtension : DefModExtension
    {

        public Data? pathReplacement = null;
        public IntRange? ageRange = null;

        public struct Data
        {
            public string from;
            public string to;
        }
    }
}
