using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityBodyPartCondition : AbilityCondition
    {
        public BodyPartDef bodyPart;
        public bool missing;

        private IEnumerable<BodyPartRecord> GetBodyParts(Pawn pawn) => pawn.health.hediffSet.GetNotMissingParts();

        public override bool IsSatisfied(Pawn pawn)
        {
            if (missing)
            {
                //Missing => None of the body parts contain specified body part.
                return GetBodyParts(pawn).All(part => part.def != bodyPart);
            }
            else
            {
                //Not missing => Any body part contains the specified body part.
                return GetBodyParts(pawn).Any(part => part.def == bodyPart);
            }
        }

        public override bool IsFulfilled(Pawn pawn)
        {
            if (missing)
            {
                //Missing => None of the body parts contain specified body part.
                return GetBodyParts(pawn).All(part => part.def != bodyPart);
            }
            else
            {
                //Not missing => Any body part contains the specified body part.
                return GetBodyParts(pawn).Any(part => part.def == bodyPart);
            }
        }

        public override string ConditionString => !missing ? "ConditionBodyPart".Translate(bodyPart.LabelCap.Named("BODY_PART")) : "ConditionBodyPart_Missing".Translate(bodyPart.LabelCap.Named("BODY_PART"));
    }
}
