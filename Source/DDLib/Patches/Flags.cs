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
    public class LegacyModExtension : DefModExtension
    {
        public bool allowStackLimitExceed;
        public bool hasOwnership;
    }

    public class AmbrosiaTargetExtension : DefModExtension
    {
        //Used as a flag
    }

    public class MatingTargetExtension : DefModExtension
    {
        //Used as a flag
    }

    public class RitualTargetExtension : DefModExtension
    {
        //Used as a flag
    }

    public class BreedingPoolExtension : DefModExtension
    {
        public string pool;
    }

    public class TrackedIncidentExtension : DefModExtension
    {
        public FloatRange maxRefireDays;
        public FloatRange watcherCooldownDays;

        public int RefireTicks => Mathf.RoundToInt(maxRefireDays.RandomInRange * GenDate.TicksPerDay);
        public int CooldownTicks => Mathf.RoundToInt(watcherCooldownDays.RandomInRange * GenDate.TicksPerDay);
    }

    public class IncidentSpawnConditionExtension : DefModExtension
    {
        public FloatRange temperature = new FloatRange(-1000f, 1000f);
    }
}
