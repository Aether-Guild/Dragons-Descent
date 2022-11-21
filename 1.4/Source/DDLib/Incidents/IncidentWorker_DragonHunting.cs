using System;
using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;

namespace DD
{
    public class SpawnHerdIncidentExtension : TimedSpawnExtension
    {
        public bool debug = false;
        public const float StoryTellerPointThreshold = 1000f;

        public float hungerPercentage = 1f;

        public float baseHerdNutrition = 100f;
        public float maxWildness = 1f;
        public float minPawnNutrition = 0f;

        public PawnKindDef PickRandomAnimalKind(Map map)
        {
            IEnumerable<PawnKindDef> defs = DefDatabase<PawnKindDef>.AllDefs;

            defs = defs.Where(def => def != null && def.RaceProps != null && def.RaceProps.meatDef != null && def.RaceProps.meatDef.IsNutritionGivingIngestible); //Filter out pawns that do not spawn meat.
            defs = defs.Where(def =>
            { //Make sure the nutrition provided from a pawn would be greater than minPawnNutrition and less than the baseHerdNutrition.
                float nutrition = def.race.race.meatDef.GetStatValueAbstract(StatDefOf.Nutrition) * def.race.GetStatValueAbstract(StatDefOf.MeatAmount);
                return minPawnNutrition <= nutrition && nutrition <= baseHerdNutrition;
            });
            defs = defs.Where(def => def.RaceProps.Animal && def.RaceProps.herdAnimal && !def.RaceProps.predator); //Filter out pawns that are not animals, are not herd animals, and are predators.
            defs = defs.Where(def => def.RaceProps.wildness <= maxWildness); //Filter out pawns that have more wildness than the threshold.
            defs = defs.Where(def => map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(def.race)); //Filter out pawns that don't like the current temperature/season.

            if (debug)
            {
                string msg = "[Debug]: Viable herd types: (Sorted from least likely to most likely) (Value is # of spawns per dragon)\n";
                foreach (PawnKindDef def in defs.OrderBy(def => def.GetAnimalPointsToHuntOrSlaughter()))
                {
                    msg += string.Format("{0}[{1}]: ({4}<incident.baseHerdNutrition> / ({2}<thingdef.meatAmount> * {3}<thingdef.meatDef.nutrition>) = {5}<herdSize>\n", def.LabelCap, def.GetAnimalPointsToHuntOrSlaughter(), def.race.GetStatValueAbstract(StatDefOf.MeatAmount), def.RaceProps.meatDef.GetStatValueAbstract(StatDefOf.Nutrition), baseHerdNutrition, CalculateHerdSize(map, def)); ;
                }
              //  Log.Message(msg);
            }

            return defs.TryRandomElementByWeight(def => def.GetAnimalPointsToHuntOrSlaughter(), out PawnKindDef kind) ? kind : null;
        }

        public int CalculateHerdSize(Map map, PawnKindDef def)
        {
            float herdNutrition = baseHerdNutrition;
            float pawnNutrition = def.race.GetStatValueAbstract(StatDefOf.MeatAmount) * def.RaceProps.meatDef.GetStatValueAbstract(StatDefOf.Nutrition); //Calculate the nutrition amount(optimally) that a single pawn yield through items when butchered.

            return Mathf.Max(3, Mathf.RoundToInt(herdNutrition / pawnNutrition)); //Minimum of 3.
        }
    }

    public class IncidentWorker_DragonHunting : TrackedIncidentWorker
    {
        public const int MaxCandidateCount = 10;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!this.def.HasModExtension<SpawnIncidentExtension>())
            {
                //Doesn't have the settings for the incident defined.
                return false;
            }

            if (!this.def.HasModExtension<SpawnIncidentExtension>())
            {
                //Doesn't have the settings for the incident defined.
                return false;
            }

            Map map = (Map)parms.target;

            if (!IncidentPawnPool.Any(map))
            {
                //Doesn't have a pawn pool to spawn pawns from.
                return false;
            }

            SpawnIncidentExtension settings = this.def.GetModExtension<SpawnIncidentExtension>();
            SpawnHerdIncidentExtension herdSettings = this.def.GetModExtension<SpawnHerdIncidentExtension>();

            if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
            {
                //No toxic fallout
                return false;
            }

            if (!PickEntryCell(map).IsValid || herdSettings.PickRandomAnimalKind(map) == null)
            {
                //Settings are valid.
                return false;
            }

            return true;
        }

        protected override bool TryExecuteWorkerSub(IncidentParms parms)
        {
            if (!this.def.HasModExtension<SpawnIncidentExtension>() || !this.def.HasModExtension<SpawnIncidentExtension>())
            {
                //Doesn't have a pawn pool to spawn pawns from OR Doesn't have the settings for the incident defined.
                return false;
            }

            Map map = (Map)parms.target;

            if (!IncidentPawnPool.Any(map))
            {
                //Doesn't have a pawn pool to spawn pawns from.
                return false;
            }

            SpawnHerdIncidentExtension herdSettings = this.def.GetModExtension<SpawnHerdIncidentExtension>();
            SpawnIncidentExtension hunterSettings = this.def.GetModExtension<SpawnIncidentExtension>();

            PawnSpawnEntry hunterEntry = IncidentPawnPool.PickEntry(map, hunterSettings);
            if (hunterEntry == null)
            {
                //Can't select a hunter.
                return false;
            }

            List<Pawn> pawns = new List<Pawn>(), spawnedPawns = new List<Pawn>();
            int hunterCount = hunterSettings.CalculateSpawnCount(map, hunterEntry);

            for (int hunter = 0; hunter < hunterCount; hunter++)
            {
                //Entry locations
                IntVec3 herdLoc = PickEntryCell(map);
                if (!herdLoc.IsValid)
                {
                    //Map blocked from all sides.
                    return false;
                }
                IEnumerable<IntVec3> candidateHunterLocs = GenerateHunterLocationCandidates(map, MaxCandidateCount).Where(cell => cell.IsValid && map.reachability.CanReach(cell, herdLoc, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors));
                if (candidateHunterLocs.EnumerableNullOrEmpty())
                {
                    //Can't spawn hunter.
                    continue;
                }
                IntVec3 hunterSpawnLoc = candidateHunterLocs.MaxBy(cell => cell.DistanceTo(herdLoc));

                //Hunter
                IncidentPawnPool.RerollEntry(map, hunterSettings, ref hunterEntry);
                Pawn hunterPawn = PawnGenerator.GeneratePawn(hunterEntry.kindDef);
                GenSpawn.Spawn(hunterPawn, CellFinder.RandomClosewalkCellNear(hunterSpawnLoc, map, 12), map, Rot4.Random);

                hunterPawn.needs.food.CurLevelPercentage = hunterSettings.hungerPercentage;

                if (hunterSettings.HasExitTick)
                {
                    hunterPawn.mindState.exitMapAfterTick = hunterSettings.ExitTick;
                }

                spawnedPawns.Add(hunterPawn);

                //Herd
                PawnKindDef herdKindDef = herdSettings.PickRandomAnimalKind(map);
                if (herdKindDef == null)
                {
                    //Can't pick a herd kind.
                    return false;
                }
                int herdSize = herdSettings.CalculateHerdSize(map, herdKindDef);
                for (int herd = 0; herd < herdSize; herd++)
                {
                    Pawn herdPawn = PawnGenerator.GeneratePawn(herdKindDef);
                    GenSpawn.Spawn(herdPawn, CellFinder.RandomClosewalkCellNear(herdLoc, map, 12), map, Rot4.Random);

                    herdPawn.needs.food.CurLevelPercentage = herdSettings.hungerPercentage;

                    if (herdSettings.HasExitTick)
                    {
                        herdPawn.mindState.exitMapAfterTick = herdSettings.ExitTick;
                    }

                    spawnedPawns.Add(herdPawn);
                }

                //Try to find a spot near the center of the map (outside the colony) to wander in.
                if (RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(herdLoc, map, 10f, out IntVec3 dest))
                {
                    spawnedPawns.ForEach(pawn => pawn.mindState.forcedGotoPosition = dest);
                }

                pawns.AddRange(spawnedPawns);
                spawnedPawns.Clear();
            }

            SendStandardLetter("LetterLabelDragonsHunting".Translate(hunterEntry.kindDef.label).CapitalizeFirst(), "LetterDragonsHunting".Translate(), LetterDefOf.NeutralEvent, parms, new LookTargets(pawns.AsEnumerable().Reverse()));
            return true;
        }

        private IEnumerable<IntVec3> GenerateHunterLocationCandidates(Map map, int count)
        {
            List<IntVec3> candidates = new List<IntVec3>();
            for (int i = 0; i < count; i++)
            {
                candidates.Add(PickEntryCell(map));
            }
            return candidates;
        }

        private IntVec3 PickEntryCell(Map map) => RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f) ? cell : IntVec3.Invalid;
    }
}
