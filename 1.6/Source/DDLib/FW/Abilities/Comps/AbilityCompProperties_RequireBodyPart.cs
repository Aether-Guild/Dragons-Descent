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
    public class AbilityCompProperties_RequireBodyPart : CompProperties_AbilityEffect
    {
        public BodyPartDef bodyPart;
        public bool missing;
        public IntRange partCount = IntRange.one;
        public string disableMessageKey = "Ability_RequiresBodyPart";
    }
}
