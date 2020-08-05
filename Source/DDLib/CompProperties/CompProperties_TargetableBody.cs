using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DD
{
    //Used with CompTargetable_SingleBody
    public class CompProperties_TargetableBody : CompProperties_Targetable
    {
        public List<BodyDef> targetDefs = new List<BodyDef>();
    }
}
