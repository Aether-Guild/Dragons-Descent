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
        public static readonly bool HuntingPawnsDefault = true;

        private Dictionary<string, bool> savedWildSpawns = new Dictionary<string, bool>();
        private Dictionary<string, bool> savedIncidents = new Dictionary<string, bool>();
        private Dictionary<string, float> savedSpawnNamedChance = new Dictionary<string, float>();

        public bool IsLoaded => Mod != null && Mod.Content != null;

        public IEnumerable<ThingDef> WildSpawns => Mod.Content.AllDefs.OfType<ThingDef>().Where(def => def.HasModExtension<SettingControlledExtension_AnimalBiome>() || def.HasModExtension<SettingControlledExtension_PlantBiome>());
        public IEnumerable<IncidentDef> IncidentDefs => Mod.Content.AllDefs.OfType<IncidentDef>().Where(def => def.HasModExtension<SettingControlledExtension_IncidentChance>());
        public IEnumerable<GenusDef> SpawnNamedGenusDefs => DefDatabase<GenusDef>.AllDefsListForReading;



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

        //public bool IsAllowedToHuntPawns(ThingDef def)
        //{
        //    if (!savedHuntingPawns.EnumerableNullOrEmpty() && savedHuntingPawns.ContainsKey(def.defName))
        //    {
        //        return savedHuntingPawns[def.defName];
        //    }
        //    if (WildSpawns.Contains(def))
        //    {
        //        return HuntingPawnsSettingsDefault;
        //    }
        //    return false;
        //}

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

        public void SetSpawnNamedChance(GenusDef def, float value)
        {
            savedSpawnNamedChance[def.defName] = value;
        }

        public bool ShouldSpawnNamed(Pawn pawn)
        {
            if (!pawn.def.HasModExtension<GenusMarkerExtension>())
                return false;
            var bestSpawnChance = pawn.def.modExtensions.OfType<GenusMarkerExtension>().Max(marker => GetSpawnNamedChance(marker.genus));
            if (bestSpawnChance == 0)
                return false;
            else if (bestSpawnChance == 1)
                return true;
            else
                return Rand.Chance(bestSpawnChance);
        }

        public float GetSpawnNamedChance(GenusDef def)
        {
            if (savedSpawnNamedChance.ContainsKey(def.defName))
            {
                return savedSpawnNamedChance[def.defName];
            }
            else
            {
                return def.spawnNamedByDefaultChance;
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

            }
        }

        public void Reset()
        {
            savedWildSpawns.Clear();
            savedIncidents.Clear();
            savedSpawnNamedChance.Clear();
        }

        public override void ExposeData()
        {
            base.ExposeData();


            Scribe_Collections.Look(ref savedWildSpawns, "WildSpawns", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref savedIncidents, "Incidents", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref savedSpawnNamedChance, "SpawnNamedChance", LookMode.Value, LookMode.Value);



            if (savedWildSpawns == null)
            {
                savedWildSpawns = new Dictionary<string, bool>();
            }
            if (savedIncidents == null)
            {
                savedIncidents = new Dictionary<string, bool>();
            }
            if (savedSpawnNamedChance == null)
            {
                savedSpawnNamedChance = new Dictionary<string, float>();
            }
        }
    }
}
