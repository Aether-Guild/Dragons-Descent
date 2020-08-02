using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class CompProperties_AssignableToPawn_Body : CompProperties_AssignableToPawn
    {
        public BodyDef bodyDef;

        public CompProperties_AssignableToPawn_Body()
        {
            compClass = typeof(CompAssignableToPawn_Nest);
        }
    }
}
