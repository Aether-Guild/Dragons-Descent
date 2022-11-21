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
    public class Building_StorageBed : Building_Bed, ISlotGroupParent, IStoreSettingsParent, IHaulDestination
    {
        public StorageSettings settings;

        public SlotGroup slotGroup;

        private List<IntVec3> cachedOccupiedCells;

        public bool StorageTabVisible => true;

        public bool IgnoreStoredThingsBeauty => def.building.ignoreStoredThingsBeauty;

        public Building_StorageBed()
        {
            slotGroup = new SlotGroup(this);
        }

        public SlotGroup GetSlotGroup()
        {
            return slotGroup;
        }

        public virtual IEnumerable<IntVec3> AllSlotCells()
        {
            foreach (IntVec3 item in GenAdj.CellsOccupiedBy(this))
            {
                yield return item;
            }
        }

        public List<IntVec3> AllSlotCellsList()
        {
            if (cachedOccupiedCells == null)
            {
                cachedOccupiedCells = AllSlotCells().ToList();
            }
            return cachedOccupiedCells;
        }

        public StorageSettings GetStoreSettings()
        {
            return settings;
        }

        public StorageSettings GetParentStoreSettings()
        {
            return def.building.fixedStorageSettings;
        }

        public string SlotYielderLabel()
        {
            return LabelCap;
        }

        public bool Accepts(Thing t)
        {
            return settings.AllowedToAccept(t);
        }

        public override void PostMake()
        {
            base.PostMake();
            settings = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                settings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            cachedOccupiedCells = null;
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref settings, "settings", this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            foreach (Gizmo item in StorageSettingsClipboard.CopyPasteGizmosFor(settings))
            {
                yield return item;
            }
        }

        public void Notify_ReceivedThing(Thing newItem)
        {
        }

        public void Notify_LostThing(Thing newItem)
        {
        }

        public void Notify_SettingsChanged() {
        }
    }
}
