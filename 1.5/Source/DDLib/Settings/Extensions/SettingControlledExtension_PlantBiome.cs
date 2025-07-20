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
}
