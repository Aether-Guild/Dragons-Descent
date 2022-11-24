using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class CompRitualAltar : ThingComp
    {
        private const int ActivationCountIncrement = 10;

        public CompProperties_Ritual Props => props as CompProperties_Ritual;

        private Gizmo_RitualInfo gRInfo;
        private CompGlower compGlow;

        private bool active = true;
        public bool Active
        {
            get => active;
            set
            {
                if (active != value)
                {
                    SetAvailable(value);
                }
                active = value;
            }
        }

        private void SetAvailable(bool state)
        {
            Building_WorkTable building = parent as Building_WorkTable;
            if (building != null)
            {
                building.BillStack.Bills.ForEach(bill => bill.suspended = !state);
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            RitualTracker rituals = parent.Map.GetComponent<MapComponent_Tracker>().Rituals;
            Active = !rituals.Full;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            compGlow = parent.GetComp<CompGlower>();

            gRInfo = new Gizmo_RitualInfo(parent.Map);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return gRInfo;

            if(parent.DestroyedOrNull() || parent.Map == null || parent.Map.Index < 0)
            {
                yield break;
            }

            RitualTracker rituals = parent.Map.GetComponent<MapComponent_Tracker>().Rituals;
            if (rituals != null)
            {
                foreach (RitualDef def in Props.rituals)
                {
                    yield return new Command_RitualEffect(parent, rituals, def);
                }

                //if (Prefs.DevMode)
                //{
                //    yield return new Command_Action()
                //    {
                //        defaultLabel = "Debug: Add Favor",
                //        action = () =>
                //        {
                //            rituals.Current++;
                //        }
                //    };
                //    yield return new Command_Action()
                //    {
                //        defaultLabel = "Debug: Fill Favor",
                //        action = () =>
                //        {
                //            rituals.Current = rituals.Max;
                //        }
                //    };
                //    yield return new Command_Action()
                //    {
                //        defaultLabel = "Debug: Increment Activation Count",
                //        action = () =>
                //        {
                //            foreach (Ritual ritual in rituals)
                //            {
                //                ritual.IncrementCount();
                //            }
                //        }
                //    };
                //    yield return new Command_Action()
                //    {
                //        defaultLabel = "Debug: +" + ActivationCountIncrement + " Activation Count",
                //        action = () =>
                //        {
                //            foreach (Ritual ritual in rituals)
                //            {
                //                for (int i = 0; i < ActivationCountIncrement; i++)
                //                {
                //                    ritual.IncrementCount();
                //                }
                //            }
                //        }
                //    };
                //    yield return new Command_Action()
                //    {
                //        defaultLabel = "Debug: Reset Activation Count",
                //        action = () =>
                //        {
                //            foreach (Ritual ritual in rituals)
                //            {
                //                ritual.Reset(true);
                //            }
                //        }
                //    };
                //    yield return new Command_Action()
                //    {
                //        defaultLabel = "Debug: Finish Cooldowns",
                //        action = () =>
                //        {
                //            foreach (Ritual ritual in rituals)
                //            {
                //                ritual.Reset();
                //            }
                //        }
                //    };
                //}
            }
        }
    }
}
