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
    public class CarryCapacityExtension : DefModExtension
    {
        public float? constant = null;
        public float offset = 0f;
        public float factor = 1f;
        public FloatRange? cap = null;
    }
}
