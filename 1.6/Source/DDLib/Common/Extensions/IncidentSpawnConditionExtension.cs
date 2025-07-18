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
    public class IncidentSpawnConditionExtension : DefModExtension
    {
        public FloatRange temperature = new FloatRange(-1000f, 1000f);
    }
}
