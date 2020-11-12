using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityComp_Base : CompAbilityEffect
    {
        public virtual bool CanCast => true;
        public virtual Command Gizmo => null;

        public virtual void PostTick() { }
        public virtual void PostExposeData() { }
        public virtual void PostReset() { }
    }
}
