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

    public class SettingControlledExtension_AnimalBiome : SettingControlledExtension<ThingDef>
    {
        //Used as a flag.
        private List<AnimalBiomeRecord> wildBiomes;
        private FieldInfo field = AccessTools.DeclaredField(typeof(BiomeDef), "cachedAnimalCommonalities");

        private Dictionary<PawnKindDef, float> GetAnimalCommonalities(BiomeDef def) => field.GetValue(def) as Dictionary<PawnKindDef, float>;
        private void UnsetAnimalCommonalities(BiomeDef def) => field.SetValue(def, null);

        protected override void DoInit(ThingDef def)
        {
            wildBiomes = def.race.wildBiomes;
        }

        protected override void DoEnable(ThingDef def)
        {
            def.race.wildBiomes = wildBiomes;
        }

        protected override void DoDisable(ThingDef def)
        {
            def.race.wildBiomes = null;
        }

        protected override void Refresh()
        {
            PawnKindDef refreshDef = DefDatabase<PawnKindDef>.AllDefsListForReading.First();
            foreach (AnimalBiomeRecord record in wildBiomes)
            {
                UnsetAnimalCommonalities(record.biome);
                record.biome.CommonalityOfAnimal(refreshDef);
            }
        }
    }

    public class SettingControlledExtension_PlantBiome : SettingControlledExtension<ThingDef>
    {
        //Used as a flag.
        private List<PlantBiomeRecord> wildBiomes;

        private static FieldInfo field = AccessTools.DeclaredField(typeof(BiomeDef), "cachedPlantCommonalities");

        private Dictionary<ThingDef, float> GetPlantCommonalities(BiomeDef def) => field.GetValue(def) as Dictionary<ThingDef, float>;
        private void UnsetPlantCommonalities(BiomeDef def) => field.SetValue(def, null);

        protected override void DoInit(ThingDef def)
        {
            wildBiomes = def.plant.wildBiomes;
        }

        protected override void DoEnable(ThingDef def)
        {
            def.plant.wildBiomes = wildBiomes;
        }

        protected override void DoDisable(ThingDef def)
        {
            def.plant.wildBiomes = null;
        }

        protected override void Refresh()
        {
            ThingDef refreshDef = DefDatabase<ThingDef>.AllDefsListForReading.First();
            foreach (PlantBiomeRecord record in wildBiomes)
            {
                UnsetPlantCommonalities(record.biome);
                record.biome.CommonalityOfPlant(refreshDef);
            }
        }
    }

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
