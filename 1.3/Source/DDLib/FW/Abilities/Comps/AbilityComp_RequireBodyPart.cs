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
    public class AbilityComp_RequireBodyPart : AbilityComp_Base
    {
        public AbilityCompProperties_RequireBodyPart VProps => props as AbilityCompProperties_RequireBodyPart;

        private static IEnumerable<BodyPartRecord> CollectRecords(BodyPartRecord record, BodyPartDef part, List<BodyPartRecord> records = null)
        {
            if (records == null)
            {
                records = new List<BodyPartRecord>();
            }

            if (record.def == part)
            {
                records.Add(record);
            }

            if (!record.parts.NullOrEmpty())
            {
                record.parts.ForEach(rec => CollectRecords(rec, part, records));
            }
            return records;
        }

        //Get all missing parts if we're evaluating missing parts. Non-missing otherwise.
        private IEnumerable<BodyPartRecord> GetIntendedBodyParts(Pawn pawn) => CollectRecords(pawn.def.race.body.corePart, VProps.bodyPart).Where(rec => VProps.missing == pawn.health.hediffSet.PartIsMissing(rec));

        private int CountBodyParts => GetIntendedBodyParts(parent.pawn).Count(part => part.def == VProps.bodyPart);

        public bool ConditionSatisfied => CountBodyParts < VProps.partCount.TrueMax;

        public override bool CanCast => ConditionSatisfied;

        public override bool GizmoDisabled(out string reason)
        {
            if (ConditionSatisfied)
            {
                return base.GizmoDisabled(out reason);
            }
            else
            {
                if (!VProps.disableMessageKey.NullOrEmpty())
                {
                    reason = VProps.disableMessageKey.Translate(parent.pawn.LabelCap.Named("PAWN"), CountBodyParts.Named("COUNT"), VProps.bodyPart.LabelCap.Named("BODYPART"));
                }
                else
                {
                    reason = null;
                }
                return true;
            }
        }
    }
}
