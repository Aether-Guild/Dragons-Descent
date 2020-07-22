using RimWorld;
using Verse;
using Verse.AI;

namespace DD
{
    public static class MateUtils
    {
        private static PawnGenerationRequest GenerateRequestFromPawn(Pawn pawn, bool copyFaction, bool copyGender, float? age = null)
        {
            PawnGenerationRequest request = default(PawnGenerationRequest);
            request.Context = PawnGenerationContext.NonPlayer;

            request.KindDef = pawn.kindDef;

            request.Tile = pawn.Tile;

            //Don't use a recycled pawn.
            request.ForceGenerateNewPawn = true;

            //Set age
            if (age.HasValue)
            {
                request.FixedBiologicalAge = age.Value;
                request.FixedChronologicalAge = age.Value;
            }
            else
            {
                request.FixedBiologicalAge = pawn.ageTracker.AgeBiologicalYearsFloat;
                request.FixedChronologicalAge = pawn.ageTracker.AgeChronologicalYearsFloat;
            }

            if (copyFaction)
            {
                request.Faction = pawn.Faction;
            }
            if (copyGender)
            {
                request.FixedGender = pawn.gender;
            }

            return request;
        }

        private static Gender MatchMake(Pawn pawn, PawnGenerationRequest request)
        {
            //Avoid having the new pawn be related to any other colonist.
            request.CanGeneratePawnRelations = false;
            request.ColonistRelationChanceFactor = 0;
            request.RelationWithExtraPawnChanceFactor = 0;
            request.ExtraPawnForExtraRelationChance = null;

            if (pawn.story != null && pawn.story.traits != null)
            {
                //Has traits
                if (pawn.story.traits.HasTrait(TraitDefOf.Gay))
                {
                    //If gay, we need to have same gender or else the new pawn won't be compatible.
                    request.AllowGay = true;
                    request.FixedGender = pawn.gender;
                }
                else if (pawn.story.traits.HasTrait(TraitDefOf.Bisexual))
                {
                    //If bi, roll a dice, since either gender would work.
                    if (Rand.Chance(0.5f))
                    {
                        request.AllowGay = true;
                        request.FixedGender = pawn.gender;
                    }
                    else
                    {
                        request.AllowGay = false;
                        request.FixedGender = pawn.gender.Opposite();
                    }
                }
            }
            else
            {
                //No traits, so just being opposite genders should be enough.
                request.FixedGender = pawn.gender.Opposite();
            }

            return request.FixedGender.Value;
        }

        private static Pawn Spawn(PawnGenerationRequest request, IntVec3 location, Map map)
        {
            Pawn newPawn = PawnGenerator.GeneratePawn(request);

            IntVec3 cell;
            if(!RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f))
            {
                //Couldn't find entry cell
                cell = location;
            }

            //Actually spawn it on screen.
            GenSpawn.Spawn(newPawn, cell, map);

            cell = CellFinder.RandomClosewalkCellNear(location, map, 10);
            if (!cell.IsValid)
            {
                cell = location;
            }

            //Move to the location where the mate call happened.
            newPawn.mindState.forcedGotoPosition = cell;

            return newPawn;
        }

        public static void CloneTraits(Pawn pawn1, Pawn pawn2)
        {
            if (pawn2.story != null && pawn2.story.traits != null)
            {
                //Copy over traits if any.
                pawn2.story.traits.allTraits.Clear();
                pawn1.story.traits.allTraits.ForEach(trait => pawn2.story.traits.GainTrait(new Trait(trait.def, trait.Degree, true)));
            }
        }

        public static void MutualBoostRelationship(Pawn pawn1, Pawn pawn2, ThoughtDef thoughtDef, int count)
        {
            if (pawn1.needs.mood != null)
            {
                //If possible, have them give good thoughts about one another.
                for (int i = 0; i < count; i++)
                {
                    pawn1.needs.mood.thoughts.memories.TryGainMemory(thoughtDef, pawn2);
                    pawn2.needs.mood.thoughts.memories.TryGainMemory(thoughtDef, pawn1);
                }
            }
            else
            {
                Pawn male = null, female = null;

                switch (pawn1.gender)
                {
                    case Gender.Male:
                        male = pawn1;
                        switch (pawn2.gender)
                        {
                            case Gender.Male:
                                return;
                            case Gender.Female:
                                female = pawn2;
                                break;
                        }
                        break;
                    case Gender.Female:
                        female = pawn1;
                        switch (pawn2.gender)
                        {
                            case Gender.Male:
                                male = pawn2;
                                break;
                            case Gender.Female:
                                return;
                        }
                        break;
                    default:
                        return;
                }

                if (male != null && female != null)
                {
                    for (int i = 0; i < count; i++)
                    {
                        PawnUtility.Mated(male, female);
                    }
                }
            }
        }

        public static Pawn SpawnMate(Pawn pawn, bool spawnTamed, float? age = null)
        {
            PawnGenerationRequest request = GenerateRequestFromPawn(pawn, spawnTamed, false, age);
            Gender gender = MatchMake(pawn, request);

            Pawn newPawn = Spawn(request, pawn.DutyLocation(), pawn.Map);
            CloneTraits(pawn, newPawn);
            newPawn.gender = gender;

            return newPawn;
        }

        public static bool FertileMateTarget(Pawn male, Pawn female)
        {
            if (PawnUtility.FertileMateTarget(male, female))
            {
                //Additional checks.
                CompCrossbredEggLayer layer = female.TryGetComp<CompCrossbredEggLayer>();
                if (layer != null)
                {
                    return !layer.FullyFertilized;
                }
            }
            return false;
        }

        public static void Mated(Pawn male, Pawn female)
        {
            if (female.ageTracker.CurLifeStage.reproductive)
            {
                CompCrossbredEggLayer compEggLayer = female.TryGetComp<CompCrossbredEggLayer>();
                if (compEggLayer != null)
                {
                    compEggLayer.FertilizeWithTracking(male);
                }
                else
                {
                    PawnUtility.Mated(male, female);
                }
            }
        }
    }
}
