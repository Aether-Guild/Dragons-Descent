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
        private static readonly string[] CompatibilityPatchDefaultFightingJobs = { "KFM_KillTarget", "HuntTrained", "HuntTrainedAssist", "HuntForMe_CatGift" };

        private Dictionary<string, bool> savedWildSpawns = new Dictionary<string, bool>();
        private Dictionary<string, bool> savedIncidents = new Dictionary<string, bool>();
        private List<string> fightingJobDefNames = new List<string>(CompatibilityPatchDefaultFightingJobs);

        public bool IsLoaded => Mod != null && Mod.Content != null;

        public bool KFM_IgnoreRange { get; set; } = CompatibilityPatchSettingsDefault;
        public bool HFM_IgnoreRange { get; set; } = CompatibilityPatchSettingsDefault;
        public bool ARA_VerbCheck { get; set; } = CompatibilityPatchSettingsDefault;
        public bool RW_RoyaltyErrors { get; set; } = CompatibilityPatchSettingsDefault && !ModsConfig.RoyaltyActive && Prefs.DevMode;
        public IEnumerable<ThingDef> WildSpawns => Mod.Content.AllDefs.OfType<ThingDef>().Where(def => def.HasModExtension<SettingControlledExtension_AnimalBiome>() || def.HasModExtension<SettingControlledExtension_PlantBiome>());
        public IEnumerable<IncidentDef> IncidentDefs => Mod.Content.AllDefs.OfType<IncidentDef>().Where(def => def.HasModExtension<SettingControlledExtension_IncidentChance>());
        public IEnumerable<JobDef> FightingJobs => fightingJobDefNames.Select(defName => DefDatabase<JobDef>.GetNamedSilentFail(defName)).OfType<JobDef>();
        public List<string> FightingJobDefs => fightingJobDefNames;

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

                if(KFM_IgnoreRange || HFM_IgnoreRange || ARA_VerbCheck || RW_RoyaltyErrors)
                {
                    CompatibilityPatcher.Patch();
                }
            }
        }

        public void Reset()
        {
            KFM_IgnoreRange = CompatibilityPatchSettingsDefault;
            HFM_IgnoreRange = CompatibilityPatchSettingsDefault;
            ARA_VerbCheck = CompatibilityPatchSettingsDefault;
            RW_RoyaltyErrors = CompatibilityPatchSettingsDefault && !ModsConfig.RoyaltyActive && Prefs.DevMode;
            savedWildSpawns.Clear();
            savedIncidents.Clear();
            fightingJobDefNames = new List<string>(CompatibilityPatchDefaultFightingJobs);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            bool KFM_IR = KFM_IgnoreRange;
            bool HFM_IR = HFM_IgnoreRange;
            bool ARA_VC = ARA_VerbCheck;
            bool RW_RE = RW_RoyaltyErrors;

            Scribe_Values.Look(ref KFM_IR, "Comp_KFM_IgnoreRange", CompatibilityPatchSettingsDefault);
            Scribe_Values.Look(ref HFM_IR, "Comp_HFM_IgnoreRange", CompatibilityPatchSettingsDefault);
            Scribe_Values.Look(ref ARA_VC, "Comp_ARA_VerbCheck", CompatibilityPatchSettingsDefault);
            Scribe_Values.Look(ref RW_RE, "Comp_RW_RoyaltyErrors", CompatibilityPatchSettingsDefault && !ModsConfig.RoyaltyActive && Prefs.DevMode);
            Scribe_Collections.Look(ref savedWildSpawns, "WildSpawns", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref savedIncidents, "Incidents", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref fightingJobDefNames, "Comp_JobDef_Fighting", LookMode.Value);

            KFM_IgnoreRange = KFM_IR;
            HFM_IgnoreRange = HFM_IR;
            ARA_VerbCheck = ARA_VC;
            RW_RoyaltyErrors = RW_RE;

            if (savedWildSpawns == null)
            {
                savedWildSpawns = new Dictionary<string, bool>();
            }
            if (savedIncidents == null)
            {
                savedIncidents = new Dictionary<string, bool>();
            }
            if (fightingJobDefNames == null)
            {
                fightingJobDefNames = new List<string>(CompatibilityPatchDefaultFightingJobs);
            }
        }
    }
}
