using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DD
{
    public class DominantProperties
    {
        public bool notify = true;
        public Pawn donor;
        public CompProperties_EggLayer props;

        public Pawn Donor => donor;
        public CompProperties_EggLayer Props => props;

        public DominantProperties(Pawn donor, CompProperties_EggLayer props)
        {
            this.donor = donor;
            this.props = props;
        }
    }
}
