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
    public class HediffComp_DisableTaming : HediffComp
    {
        public override bool CompShouldRemove => false;

        public override string CompDebugString()
        {
            return "taming=false";
        }
    }
}
