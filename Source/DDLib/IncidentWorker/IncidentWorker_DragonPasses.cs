using System;
using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace DD
{
    public class GameTime
    {
        public int ticks = 0;
        public float seconds = 0;
        public float hours = 0;
        public float days = 0;
        public float seasons = 0;
        public float quadrums = 0;
        public float years = 0;

        public int Ticks
        {
            get
            {
                int aggregate = ticks;

                aggregate += GenTicks.SecondsToTicks(seconds);
                aggregate += Mathf.RoundToInt(hours * GenDate.TicksPerHour);
                aggregate += Mathf.RoundToInt(days * GenDate.TicksPerDay);
                aggregate += Mathf.RoundToInt(seasons * GenDate.TicksPerSeason);
                aggregate += Mathf.RoundToInt(quadrums * GenDate.TicksPerQuadrum);
                aggregate += Mathf.RoundToInt(years * GenDate.TicksPerYear);

                return aggregate;
            }
        }
    }


    public class PawnPoolExtension : DefModExtension
    {
        public List<Entry> pawnPool;

        public float MinimumCombatPower => pawnPool.Min(e => e.CombatPower);

        public Entry PickEntry(Map map, PassingIncidentExtension settings)
        {
            Entry entry = null;
            IEnumerable<Entry> pawnPoolSelection = pawnPool;

            if (!Rand.Chance(settings.chancesSpawnRandom))
            {
                //Only consider dragons that'll be comfortable in the area and in its allowed biome
                pawnPoolSelection = pawnPoolSelection.Where(e => e.CanSpawn(map) && map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(e.thingDef));
            }

            //If there's still any pawns in the pool.
            if (pawnPoolSelection.Any())
            {
                //Actually pick one of the pawns in the pool.
                entry = pawnPoolSelection.RandomElement();
            }

            return entry;
        }

        public class Entry
        {
            public ThingDef thingDef;
            public PawnKindDef kindDef;
            public FloatRange temperature = new FloatRange(-1000f, 1000f);

            public float CombatPower => Mathf.Max(kindDef.combatPower * thingDef.race.baseBodySize, 1);

            public bool CanSpawn(Map map) => temperature.Includes(map.mapTemperature.OutdoorTemp);
        }
    }

    public class PassingIncidentExtension : DefModExtension
    {
        public float chancesSpawnRandom = 0;

        public int minSpawn, maxSpawn;

        public GameTime minExit, maxExit;

        public int MinExit_Ticks => minExit.Ticks;
        public int MaxExit_Ticks => maxExit.Ticks;
    }

    public class IncidentWorker_DragonPasses : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!this.def.HasModExtension<PawnPoolExtension>())
            {
                //Doesn't have a pawn pool to spawn pawns from.
                return false;
            }

            if (!this.def.HasModExtension<PassingIncidentExtension>())
            {
                //Doesn't have the settings for the incident defined.
                return false;
            }

            PawnPoolExtension pool = this.def.GetModExtension<PawnPoolExtension>();
            PassingIncidentExtension settings = this.def.GetModExtension<PassingIncidentExtension>();

            Map map = (Map)parms.target;
            IntVec3 intVec;

            if (!map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
            {
                //No toxic fallout
                if (TryFindEntryCell(map, out intVec))
                {
                    //Map not blocked from all sides
                    return pool.PickEntry(map, settings) != null;
                }
            }

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!this.def.HasModExtension<PawnPoolExtension>() || !this.def.HasModExtension<PassingIncidentExtension>())
            {
                //Doesn't have a pawn pool to spawn pawns from OR Doesn't have the settings for the incident defined.
                return false;
            }

            PawnPoolExtension pool = this.def.GetModExtension<PawnPoolExtension>();
            PassingIncidentExtension settings = this.def.GetModExtension<PassingIncidentExtension>();

            Map map = (Map)parms.target;
            IntVec3 intVec;

            if (!TryFindEntryCell(map, out intVec))
            {
                //Can't find an entry point to the map.
                return false;
            }

            PawnPoolExtension.Entry entry = pool.PickEntry(map, settings);
            if (entry == null)
            {
                //No pawns to pick from?
                return false;
            }

            PawnKindDef def = entry.kindDef;

            //Number of pawns to spawn
            int count = GenMath.RoundRandom(StorytellerUtility.DefaultThreatPointsNow(map) / entry.CombatPower); //How capable the colony of fending off this incident (if it were a threat)
            count = Mathf.Clamp(count, settings.minSpawn, Rand.RangeInclusive(settings.minSpawn, settings.maxSpawn)); //Clamp the number above to between the minimum number of spawns and maximum number of spawns.

            //Number of ticks to stay
            int ticksToStayOnMap = Rand.RangeInclusive(settings.MinExit_Ticks, settings.MaxExit_Ticks);

            //Try to find a spot near the center of the map (outside the colony) to wander in.
            IntVec3 invalid = IntVec3.Invalid;
            if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(intVec, map, 10f, out invalid))
            {
                //Couldn't find any. Will wander anywhere.
                invalid = IntVec3.Invalid;
            }

            //Actually spawn the pawn.
            Pawn pawn = null;
            for (int i = 0; i < count; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);

                pawn = PawnGenerator.GeneratePawn(def, null);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);

                pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + ticksToStayOnMap;

                if (invalid.IsValid)
                {
                    pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, map, 10);
                }
            }

            base.SendStandardLetter("LetterLabelDragonsPasses".Translate(def.label).CapitalizeFirst(), "LetterDragonsPasses".Translate(def.label), LetterDefOf.NeutralEvent, parms, pawn, Array.Empty<NamedArgument>());
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
        }
    }
}
