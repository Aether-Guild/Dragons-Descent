using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DD
{
    public class CompProperties_Ritual : CompProperties
    {
        public List<RitualDef> rituals;

        public CompProperties_Ritual()
        {
            compClass = typeof(CompRitualAltar);
        }
    }
}
