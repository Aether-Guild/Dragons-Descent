using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public abstract class HediffCompProperties_BaseRegen : HediffCompProperties
    {
        public virtual bool ShouldKeepWhileFighting(HediffDef def)
        {
            if (def.HasModExtension<RegenHediffModExtension>())
            {
                return def.GetModExtension<RegenHediffModExtension>().keepWhileFighting;
            }

            return false;
        }
    }
}
