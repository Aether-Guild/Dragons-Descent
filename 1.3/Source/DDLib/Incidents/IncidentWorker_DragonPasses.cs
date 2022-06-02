using System;
using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace DD
{
    public static class IncidentPawnPool
    {
        private static IEnumerable<PawnKindDef> IncidentPawns => DefDatabase<PawnKindDef>.AllDefsListForReading.Where(def => def.HasModExtension<IncidentSpawnConditionExtension>());
        private static IEnumerable<PawnKindDef> GetIncidentPawns(Map map) => IncidentPawns.Where(def => map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(def.race) && def.GetModExtension<IncidentSpawnConditionExtension>().temperature.Includes(map.mapTemperature.OutdoorTemp));

        public static bool Any(Map map) => GetIncidentPawns(map).Any();

        public static PawnSpawnEntry PickEntry(Map map, SpawnIncidentExtension settings)
        {
            PawnSpawnEntry entry = null;
            IEnumerable<PawnKindDef> selection;

            if (Rand.Chance(settings.chancesSpawnRandom))
            {
                //Plain random selection
                selection = IncidentPawns;
            }
            else
            {
                //Only consider dragons that'll be comfortable in the area and in its allowed biome
                selection = GetIncidentPawns(map);
            }

            //If there's still any pawns in the pool.
            if (selection.Any())
            {
                //Actually pick one of the pawns in the pool.
                entry = new PawnSpawnEntry()
                {
                    kindDef = selection.RandomElement()
                };
            }

            return entry;
        }

        public static bool RerollEntry(Map map, SpawnIncidentExtension settings, ref PawnSpawnEntry entry)
        {
            IEnumerable<PawnKindDef> selection;

            if (Rand.Chance(settings.chancesSpawnRandom))
            {
                //Plain random selection
                selection = IncidentPawns;
            }
            else
            {
                //Only consider dragons that'll be comfortable in the area and in its allowed biome, AND with combat power of the same magnitude.
                PawnSpawnEntry e = entry;
                selection = GetIncidentPawns(map).Where(def => Mathf.Approximately(Mathf.Round(Mathf.Log10(def.combatPower)), Mathf.Round(Mathf.Log10(e.CombatPower))));
            }

            //If there's still any pawns in the pool.
            if (selection.Any())
            {
                //Actually pick one of the pawns in the pool.
                entry = new PawnSpawnEntry()
                {
                    kindDef = selection.RandomElement()
                };
                return true;
            }
            return false;
        }
    }

    public class PawnSpawnEntry
    {
        public PawnKindDef kindDef;

        public ThingDef ThingDef => kindDef.race;
        public PawnKindDef KindDef => kindDef;
        public FloatRange Temperature => kindDef.GetModExtension<IncidentSpawnConditionExtension>().temperature;
        public float CombatPower => Mathf.Max(KindDef.combatPower * ThingDef.race.baseBodySize, 1);

        public bool CanSpawn(Map map) => Temperature.Includes(map.mapTemperature.OutdoorTemp);
    }

    public class TimedSpawnExtension : DefModExtension
    {
        public GameTime minExit, maxExit;

        public bool HasExitTick => minExit != null || maxExit != null;
        public int ExitTick
        {
            get
            {
                int ticks = Find.TickManager.TicksGame;

                if (minExit != null)
                {
                    ticks += maxExit != null ? Rand.RangeInclusive(minExit.Ticks, maxExit.Ticks) : minExit.Ticks;
                }
                else
                {
                    if (maxExit != null)
                    {
                        ticks += maxExit.Ticks;
                    }
                    else
                    {
                        ticks = -1;
                    }
                }

                return ticks;
            }
        }
    }

    public class SpawnIncidentExtension : TimedSpawnExtension
    {
        public float chancesSpawnRandom = 0;

        public float hungerPercentage = 1f;

        public IntRange spawnCount;

        public int CalculateSpawnCount(Map map, PawnSpawnEntry entry)
        {
            int count = GenMath.RoundRandom(StorytellerUtility.DefaultThreatPointsNow(map) / entry.CombatPower); //How capable the colony of fending off this incident (if it were a threat)
            count = Mathf.Clamp(count, spawnCount.TrueMin, spawnCount.RandomInRange); //Clamp the number above to between the minimum number of spawns and maximum number of spawns.
            return count;
        }
    }

    public class IncidentWorker_DragonPasses : TrackedIncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!this.def.HasModExtension<SpawnIncidentExtension>())
            {
                //Doesn't have the settings for the incident defined.
                return false;
            }

            SpawnIncidentExtension settings = this.def.GetModExtension<SpawnIncidentExtension>();

            Map map = (Map)parms.target;

            if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
            {
                //No toxic fallout
                return false;
            }

            if (!TryFindEntryCell(map, out IntVec3 _) || IncidentPawnPool.PickEntry(map, settings) == null)
            {
                //Settings invalid
                return false;
            }

            return true;
        }

        protected override bool TryExecuteWorkerSub(IncidentParms parms)
        {
            if (!this.def.HasModExtension<SpawnIncidentExtension>())
            {
                //Doesn't have the settings for the incident defined.
                return false;
            }

            SpawnIncidentExtension settings = this.def.GetModExtension<SpawnIncidentExtension>();

            Map map = (Map)parms.target;
            IntVec3 intVec;

            if (!IncidentPawnPool.Any(map))
            {
                //Doesn't have a pawn pool to spawn pawns from.
                return false;
            }

            if (!TryFindEntryCell(map, out intVec))
            {
                //Can't find an entry point to the map.
                return false;
            }

            PawnSpawnEntry entry = IncidentPawnPool.PickEntry(map, settings);
            if (entry == null)
            {
                //No pawns to pick from?
                return false;
            }

            PawnKindDef def = entry.kindDef;

            //Number of pawns to spawn
            int count = settings.CalculateSpawnCount(map, entry);

            //Try to find a spot near the center of the map (outside the colony) to wander in.
            IntVec3 invalid = IntVec3.Invalid;
            if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(intVec, map, 10f, out invalid))
            {
                //Couldn't find any. Will wander anywhere.
                invalid = IntVec3.Invalid;
            }

            //Actually spawn the pawn.
            List<Pawn> spawnedPawns = new List<Pawn>();
            for (int i = 0; i < count; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10);

                Pawn pawn = PawnGenerator.GeneratePawn(def);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random);

                pawn.needs.food.CurLevelPercentage = settings.hungerPercentage;

                if (settings.HasExitTick)
                {
                    pawn.mindState.exitMapAfterTick = settings.ExitTick;
                }

                if (invalid.IsValid)
                {
                    pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, map, 10);
                }

                spawnedPawns.Add(pawn);
            }

            SendStandardLetter("LetterLabelDragonsPasses".Translate(def.label).CapitalizeFirst(), "LetterDragonsPasses".Translate(def.label.Named("DRAGON")), LetterDefOf.NeutralEvent, parms, spawnedPawns);
            return true;
        }

        private bool TryFindEntryCell(Map map, out IntVec3 cell)
        {
            return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
        }
    }
}
