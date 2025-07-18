using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;


namespace DD

{
    using Verse;

    public class DD_CompProperties_StartCharged : CompProperties
    {


        public DD_CompProperties_StartCharged()
        {
            this.compClass = typeof(DD_CompStartCharged);
        }
    }
}

