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
    public class TrackedIncidentExtension : DefModExtension
    {
        public FloatRange maxRefireDays;
        public FloatRange watcherCooldownDays;

        public int RefireTicks => Mathf.RoundToInt(maxRefireDays.RandomInRange * GenDate.TicksPerDay);
        public int CooldownTicks => Mathf.RoundToInt(watcherCooldownDays.RandomInRange * GenDate.TicksPerDay);
    }
}
