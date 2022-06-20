using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class DraconicSettings : ModSettings
    {
        public static readonly bool WildSpawnSettingsDefault = true;
        public static readonly bool IncidentSettingsDefault = true;
        public static readonly bool CompatibilityPatchSettingsDefault = true;

        private Dictionary<string, bool> savedWildSpawns = new Dictionary<string, bool>();
        private Dictionary<string, bool> savedIncidents = new Dictionary<string, bool>();
        private Dictionary<string, bool> savedAlwaysSpawnsNamed = new Dictionary<string, bool>();

        public bool IsLoaded => Mod != null && Mod.Content != null;

        //public bool KFM_IgnoreRange { get; set; } = CompatibilityPatchSettingsDefault;
        //public bool HFM_IgnoreRange { get; set; } = CompatibilityPatchSettingsDefault;
        //public bool ARA_VerbCheck { get; set; } = CompatibilityPatchSettingsDefault;
        public IEnumerable<ThingDef> WildSpawns => Mod.Content.AllDefs.OfType<ThingDef>().Where(def => def.HasModExtension<SettingControlledExtension_AnimalBiome>() || def.HasModExtension<SettingControlledExtension_PlantBiome>());
        public IEnumerable<IncidentDef> IncidentDefs => Mod.Content.AllDefs.OfType<IncidentDef>().Where(def => def.HasModExtension<SettingControlledExtension_IncidentChance>());
        //public IEnumerable<ThingDef> WildSpawns => DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.HasModExtension<SettingControlledExtension_AnimalBiome>() || def.HasModExtension<SettingControlledExtension_PlantBiome>());
        //public IEnumerable<IncidentDef> IncidentDefs => DefDatabase<IncidentDef>.AllDefsListForReading.Where(def => def.HasModExtension<SettingControlledExtension_IncidentChance>());
        public IEnumerable<GenusDef> SpawnNamedGenusDefs => DefDatabase<GenusDef>.AllDefsListForReading;

        //public bool ShouldRunCompatPatch => KFM_IgnoreRange | HFM_IgnoreRange;

        public bool IsAllowedToSpawn(ThingDef def)
        {
            if (!savedWildSpawns.EnumerableNullOrEmpty() && savedWildSpawns.ContainsKey(def.defName))
            {
                return savedWildSpawns[def.defName];
            }
            if (WildSpawns.Contains(def))
            {
                return WildSpawnSettingsDefault;
            }
            return false;
        }

        public void SetAllowedToSpawn(ThingDef def, bool value)
        {
            if(WildSpawns.Contains(def))
            {
                savedWildSpawns[def.defName] = value;
            }
        }

        public bool IsIncidentEnabled(IncidentDef def)
        {
            if (savedIncidents.ContainsKey(def.defName))
            {
                return savedIncidents[def.defName];
            }
            if (IncidentDefs.Contains(def))
            {
                return IncidentSettingsDefault;
            }
            return false;
        }

        public void SetIncidentEnabled(IncidentDef def, bool value)
        {
            if(IncidentDefs.Contains(def))
            {
                savedIncidents[def.defName] = value;
            }
        }

        public void SetAlwaysSpawnsNamed(GenusDef def, bool value)
        {
            savedAlwaysSpawnsNamed[def.defName] = value;
        }

        public bool ShouldAlwaysSpawnNamed(Pawn pawn)
        {
            if (!pawn.def.HasModExtension<GenusMarkerExtension>())
                return false;
            return pawn.def.modExtensions.OfType<GenusMarkerExtension>().Any(ex => DraconicOverseer.Settings.ShouldAlwaysSpawnNamed(ex.genus));
        }

        public bool ShouldAlwaysSpawnNamed(GenusDef def)
        {
            if (savedAlwaysSpawnsNamed.ContainsKey(def.defName))
            {
                return savedAlwaysSpawnsNamed[def.defName];
            }
            else
            {
                return def.spawnsNamedByDefault;
            }
        }

        public void Apply()
        {
            if (IsLoaded)
            {
                foreach (ThingDef def in WildSpawns)
                {
                    if(IsAllowedToSpawn(def))
                    {
                        def.GetModExtension<SettingControlledExtension_AnimalBiome>()?.Enable(def);
                        def.GetModExtension<SettingControlledExtension_PlantBiome>()?.Enable(def);
                    } else
                    {
                        def.GetModExtension<SettingControlledExtension_AnimalBiome>()?.Disable(def);
                        def.GetModExtension<SettingControlledExtension_PlantBiome>()?.Disable(def);
                    }
                }

                foreach (IncidentDef def in IncidentDefs)
                {
                    if(IsIncidentEnabled(def))
                    {
                        def.GetModExtension<SettingControlledExtension_IncidentChance>()?.Enable(def);
                    } else
                    {
                        def.GetModExtension<SettingControlledExtension_IncidentChance>()?.Disable(def);
                    }
                }

                //if(ShouldRunCompatPatch)
                //{
                //    CompatibilityPatcher.Patch();
                //}
            }
        }

        public void Reset()
        {
            //KFM_IgnoreRange = CompatibilityPatchSettingsDefault;
            //HFM_IgnoreRange = CompatibilityPatchSettingsDefault;
            //ARA_VerbCheck = CompatibilityPatchSettingsDefault;
            savedWildSpawns.Clear();
            savedIncidents.Clear();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            //bool KFM_IR = KFM_IgnoreRange;
            //bool HFM_IR = HFM_IgnoreRange;
            //bool ARA_VC = ARA_VerbCheck;

            //Scribe_Values.Look(ref KFM_IR, "Comp_KFM_IgnoreRange", CompatibilityPatchSettingsDefault);
            //Scribe_Values.Look(ref HFM_IR, "Comp_HFM_IgnoreRange", CompatibilityPatchSettingsDefault);
            //Scribe_Values.Look(ref ARA_VC, "Comp_ARA_VerbCheck", CompatibilityPatchSettingsDefault);
            Scribe_Collections.Look(ref savedWildSpawns, "WildSpawns", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref savedIncidents, "Incidents", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref savedAlwaysSpawnsNamed, "AlwaysSpawnsNamed", LookMode.Value, LookMode.Value);

            //KFM_IgnoreRange = KFM_IR;
            //HFM_IgnoreRange = HFM_IR;
            //ARA_VerbCheck = ARA_VC;

            if (savedWildSpawns == null)
            {
                savedWildSpawns = new Dictionary<string, bool>();
            }
            if (savedIncidents == null)
            {
                savedIncidents = new Dictionary<string, bool>();
            }
            if (savedAlwaysSpawnsNamed == null)
            {
                savedAlwaysSpawnsNamed = new Dictionary<string, bool>();
            }
        }
    }
}
