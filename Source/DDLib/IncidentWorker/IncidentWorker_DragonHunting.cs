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

        public PawnKindDef PickRandomAnimalKind(Map map)
        {
            IEnumerable<PawnKindDef> defs = DefDatabase<PawnKindDef>.AllDefs;

            defs = defs.Where(def => def != null && def.RaceProps != null && def.RaceProps.meatDef != null && def.RaceProps.meatDef.IsNutritionGivingIngestible); //Filter out pawns that do not spawn meat.
            defs = defs.Where(def => def.race.race.meatDef.GetStatValueAbstract(StatDefOf.Nutrition) <= baseHerdNutrition); //Make sure the nutrition total is less than
            defs = defs.Where(def => def.RaceProps.Animal && def.RaceProps.herdAnimal && !def.RaceProps.predator); //Filter out pawns that are not animals, are not herd animals, and are predators.
            defs = defs.Where(def => def.RaceProps.wildness <= maxWildness); //Filter out pawns that have more wildness than the threshold.
            defs = defs.Where(def => map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(def.race)); //Filter out pawns that don't like the current temperature/season.

            if (debug)
            {
                string msg = "[Debug]: Viable herd types: (Sorted from least likely to most likely)\n";
                foreach (PawnKindDef def in defs.OrderBy(def => def.GetAnimalPointsToHuntOrSlaughter()))
                {
                    msg += string.Format("{0}[{1}]: ({4}<incident.baseHerdNutrition> * (1 + ({5}<storyteller.threat.points / {6}<storyteller.factor>)) / ({2}<thingdef.meatAmount> * {3}<thingdef.meatDef.nutrition>) = {7}<herdSize>\n", def.LabelCap, def.GetAnimalPointsToHuntOrSlaughter(), def.race.GetStatValueAbstract(StatDefOf.MeatAmount), def.RaceProps.meatDef.GetStatValueAbstract(StatDefOf.Nutrition), baseHerdNutrition, StorytellerUtility.DefaultThreatPointsNow(map), StoryTellerPointThreshold, CalculateHerdSize(map, def)); ;
                }
                Log.Message(msg);
            }

            return defs.TryRandomElementByWeight(def => def.GetAnimalPointsToHuntOrSlaughter(), out PawnKindDef kind) ? kind : null;
        }

        public int CalculateHerdSize(Map map, PawnKindDef def)
        {
            float herdNutrition = baseHerdNutrition; //Scale base nutrition up by point threshold
            herdNutrition *= 1 + (StorytellerUtility.DefaultThreatPointsNow(map) / StoryTellerPointThreshold); //Scale base nutrition up by point threshold
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

            if(!PickEntryCell(map).IsValid || herdSettings.PickRandomAnimalKind(map) == null)
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
            List<Pawn> pawns = new List<Pawn>();

            if (!IncidentPawnPool.Any(map))
            {
                //Doesn't have a pawn pool to spawn pawns from.
                return false;
            }

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
                return false;
            }
            IntVec3 hunterLoc = candidateHunterLocs.MaxBy(cell => cell.DistanceTo(herdLoc));

            SpawnHerdIncidentExtension herdSettings = this.def.GetModExtension<SpawnHerdIncidentExtension>();
            SpawnIncidentExtension hunterSettings = this.def.GetModExtension<SpawnIncidentExtension>();

            PawnSpawnEntry hunterEntry = IncidentPawnPool.PickEntry(map, hunterSettings);
            PawnKindDef herdKindDef = herdSettings.PickRandomAnimalKind(map);
            if (hunterEntry == null)
            {
                //Can't select a hunter.
                return false;
            }
            if (herdKindDef == null)
            {
                //Can't pick a herd kind.
                return false;
            }

            //Herd
            int herdSize = herdSettings.CalculateHerdSize(map, herdKindDef);
            for (int i = 0; i < herdSize; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(herdLoc, map, 12);
                Pawn pawn = PawnGenerator.GeneratePawn(herdKindDef);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random);

                pawn.needs.food.CurLevelPercentage = herdSettings.hungerPercentage;

                if (herdSettings.HasExitTick)
                {
                    pawn.mindState.exitMapAfterTick = herdSettings.ExitTick;
                }

                pawns.Add(pawn);
            }

            //Hunter
            int hunterCount = hunterSettings.CalculateSpawnCount(map, hunterEntry);
            for (int i = 0; i < hunterCount; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(hunterLoc, map, 12);
                Pawn pawn = PawnGenerator.GeneratePawn(hunterEntry.kindDef);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random);

                pawn.needs.food.CurLevelPercentage = hunterSettings.hungerPercentage;

                if (hunterSettings.HasExitTick)
                {
                    pawn.mindState.exitMapAfterTick = hunterSettings.ExitTick;
                }

                pawns.Add(pawn);
            }


            //Try to find a spot near the center of the map (outside the colony) to wander in.
            if (RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(herdLoc, map, 10f, out IntVec3 dest))
            {
                foreach (Pawn pawn in pawns)
                {
                    pawn.mindState.forcedGotoPosition = dest;
                }
            }

            SendStandardLetter("LetterLabelDragonsHunting".Translate(hunterEntry.kindDef.label).CapitalizeFirst(), "LetterDragonsHunting".Translate(hunterEntry.kindDef.label.Named("DRAGON"), herdKindDef.label.Named("ANIMAL")), LetterDefOf.NeutralEvent, parms, new LookTargets(pawns.AsEnumerable().Reverse()));
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
