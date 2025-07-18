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
            if (wildBiomes != null)
            {
                PawnKindDef refreshDef = DefDatabase<PawnKindDef>.AllDefsListForReading.First();
                foreach (AnimalBiomeRecord record in wildBiomes)
                {
                    UnsetAnimalCommonalities(record.biome);
                    record.biome.CommonalityOfAnimal(refreshDef);
                }
            }
        }
    }
}
