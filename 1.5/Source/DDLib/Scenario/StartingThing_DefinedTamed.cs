using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class TamingFactionExtension : DefModExtension
    {
        public FactionDef tamingFaction;
    }

    public class ScenPart_StartingThing_DefinedTamed : ScenPart_StartingThing_Defined
    {
        public bool HasFaction => def.HasModExtension<TamingFactionExtension>() && def.GetModExtension<TamingFactionExtension>().tamingFaction != null;
        public FactionDef FactionDef => HasFaction ? def.GetModExtension<TamingFactionExtension>().tamingFaction : null;
        public Faction Faction => HasFaction ? FactionUtility.DefaultFactionFrom(def.GetModExtension<TamingFactionExtension>().tamingFaction) : null;

        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            foreach (string entry in base.GetSummaryListEntries(tag))
            {
                yield return entry + (HasFaction ? " (" + FactionDef.LabelCap + ")" : (TaggedString)"");
            }
        }

        protected override IEnumerable<ThingDef> PossibleThingDefs()
        {
            return base.PossibleThingDefs().Where(def => def.HasComp(typeof(CompHatcher)));
        }

        public override IEnumerable<Thing> PlayerStartingThings()
        {
            foreach (Thing thing in base.PlayerStartingThings())
            {
                if(HasFaction)
                {
                    Thing t = thing;
                    if (t is MinifiedThing mthing)
                    {
                        t = mthing.InnerThing;
                    }

                    CompHatcher hatcher = t.TryGetComp<CompHatcher>();
                    if (hatcher != null)
                    {
                        hatcher.hatcheeFaction = Faction;
                    }
                }
                
                yield return thing;
            }
        }
    }
}
