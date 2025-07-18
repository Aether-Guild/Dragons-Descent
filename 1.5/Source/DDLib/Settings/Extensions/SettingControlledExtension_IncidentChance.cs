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
    public class SettingControlledExtension_IncidentChance : SettingControlledExtension<IncidentDef>
    {
        //Used as a flag.
        private float baseChance;
        private float baseChanceWithRoyalty;
        private int minDifficulty;
        private float minRefireDays;
        private float minThreatPoints;
        private int minPopulation;

        protected override void DoInit(IncidentDef def)
        {
            baseChance = def.baseChance;
            baseChanceWithRoyalty = def.baseChanceWithRoyalty;
            minDifficulty = def.minDifficulty;
            minRefireDays = def.minRefireDays;
            minThreatPoints = def.minThreatPoints;
            minPopulation = def.minPopulation;
        }

        protected override void DoEnable(IncidentDef def)
        {
            def.baseChance = baseChance;
            def.baseChanceWithRoyalty = baseChanceWithRoyalty;
            def.minDifficulty = minDifficulty;
            def.minRefireDays = minRefireDays;
            def.minThreatPoints = minThreatPoints;
            def.minPopulation = minPopulation;
        }

        protected override void DoDisable(IncidentDef def)
        {
            def.baseChance = 0;
            def.baseChanceWithRoyalty = 0;
            def.minDifficulty = int.MaxValue;
            def.minRefireDays = float.MaxValue;
            def.minThreatPoints = float.MaxValue;
            def.minPopulation = int.MaxValue;
        }
    }
}
