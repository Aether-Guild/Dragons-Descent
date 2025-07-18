using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    public class CompRecordInitializer : ThingComp
    {
        public CompProperties_RecordInitializer Props => (CompProperties_RecordInitializer)props;

        public Pawn SelfPawn => (Pawn)parent;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (respawningAfterLoad)
            {
                //Process only when spawning. Not when respawning.
                return;
            }

            if(!Props.init.NullOrEmpty())
            {
                foreach (RecordInitializer init in Props.init)
                {
                    float value = init.data.Evaluate(SelfPawn.ageTracker.AgeBiologicalYearsFloat);

                    switch (init.recordDef.type)
                    {
                        case RecordType.Float:
                            break;
                        case RecordType.Time:
                        case RecordType.Int:
                            value = Mathf.RoundToInt(value);
                            break;
                    }

                    SelfPawn.records.AddTo(init.recordDef, value);
                }
            }
        }
    }
}
