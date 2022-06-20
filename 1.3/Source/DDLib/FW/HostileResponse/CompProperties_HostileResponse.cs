using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class CompProperties_HostileResponse : CompProperties
    {
        public bool debug = false;
        public bool controllableGizmo = true;
        public HostilityResponseType initialHostility = HostilityResponseType.Aggressive;
        public List<HostileResponseOption> options = new List<HostileResponseOption>();
        public MentalStateDef friendlyFireMentalState;
    }
}
