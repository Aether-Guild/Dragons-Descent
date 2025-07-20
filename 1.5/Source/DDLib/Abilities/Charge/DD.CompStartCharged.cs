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
    public class DD_CompStartCharged : ThingComp
    {
        public DD_CompProperties_StartCharged Props => (DD_CompProperties_StartCharged)this.props;



        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CompPowerBattery comp = this.parent.GetComp<CompPowerBattery>();

            comp.SetStoredEnergyPct(1f);

        }

    }
}
