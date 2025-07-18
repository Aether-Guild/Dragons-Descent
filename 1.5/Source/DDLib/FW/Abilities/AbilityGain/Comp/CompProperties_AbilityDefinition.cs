using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class CompProperties_AbilityDefinition : CompProperties
    {
        public string gizmoLabel;
        public string gizmoDesc;
        public string gizmoIconPath;

        public List<AbilityDefinitionEntry> abilities;
    }
}
