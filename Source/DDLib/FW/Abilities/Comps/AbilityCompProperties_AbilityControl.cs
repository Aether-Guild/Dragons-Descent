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
    public class AbilityCompProperties_AbilityControl : CompProperties_AbilityEffect
    {
        public bool autoUse = false;
        public bool abilityControllable = true;

        public string gizmoOnText = "Breath Enabled";
        public string gizmoOffText = "Breath Disabled";
        public string gizmoOnIconPath;
        public string gizmoOffIconPath;
        public string gizmoDesc = "Toggle Dragon Breath Usage";
    }
}
