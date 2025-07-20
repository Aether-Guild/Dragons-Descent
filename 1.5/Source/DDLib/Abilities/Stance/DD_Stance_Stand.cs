using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using RimWorld.Planet;
using VFECore.Abilities;


namespace DD

{

    public class DD_Stance_Stand : Stance_Busy
    {

        public DD_Stance_Stand(int ticks, LocalTargetInfo focusTarg, Verb verb)
            : base(1000, focusTarg, verb)

        {

        }
    }
}

