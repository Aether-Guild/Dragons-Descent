using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD;

public class JobGiver_GetFood_Dragon : ThinkNode_JobGiver
{
    private HungerCategory minCategory;

    private float maxLevelPercentage = 1f;

    public bool forceScanWholeMap;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        JobGiver_GetFood_Dragon obj = (JobGiver_GetFood_Dragon)base.DeepCopy(resolve);
        obj.minCategory = minCategory;
        obj.maxLevelPercentage = maxLevelPercentage;
        obj.forceScanWholeMap = forceScanWholeMap;
        return obj;
    }

    public override float GetPriority(Pawn pawn)
    {
        Need_Food food = pawn.needs.food;
        if (food == null)
        {
            return 0f;
        }
        if ((int)pawn.needs.food.CurCategory < 3 && FoodUtility_Dragon.ShouldBeFedBySomeone(pawn))
        {
            return 0f;
        }
        if ((int)food.CurCategory < (int)minCategory)
        {
            return 0f;
        }
        if (food.CurLevelPercentage > maxLevelPercentage)
        {
            return 0f;
        }
        if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
        {
            return 9.5f;
        }
        return 0f;
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
        Need_Food food = pawn.needs.food;
        if (food == null || (int)food.CurCategory < (int)minCategory || food.CurLevelPercentage > maxLevelPercentage)
        {
            return null;
        }
        FoodPreferability foodPreferability = FoodPreferability.Undefined;
        bool flag;
        if (pawn.AnimalOrWildMan())
        {
            flag = true;
        }
        else
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition);
            flag = firstHediffOfDef != null && firstHediffOfDef.Severity > 0.4f;
        }
        if (pawn.IsMutant && pawn.mutant.Def.allowEatingCorpses)
        {
            flag = true;
            foodPreferability = FoodPreferability.DesperateOnly;
        }
        bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;
        bool allowCorpse = flag;
        FoodPreferability minPrefOverride = foodPreferability;
        if (!FoodUtility_Dragon.TryFindBestFoodSourceFor(pawn, pawn, desperate, out var foodSource, out var foodDef, canRefillDispenser: true, canUseInventory: true, canUsePackAnimalInventory: true, allowForbidden: false, allowCorpse, allowSociallyImproper: false, pawn.IsWildMan(), forceScanWholeMap, ignoreReservations: false, calculateWantedStackCount: false, allowVenerated: false, minPrefOverride))
        {
            return null;
        }
        if (foodSource is Pawn pawn2)
        {
            Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, pawn2);
            job.killIncappedTarget = true;
            return job;
        }
        if (foodSource is Plant && foodSource.def.plant.harvestedThingDef == foodDef)
        {
            return JobMaker.MakeJob(JobDefOf.Harvest, foodSource);
        }
        if (foodSource is Building_NutrientPasteDispenser building_NutrientPasteDispenser && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
        {
            Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
            if (building != null)
            {
                ISlotGroupParent hopperSgp = building as ISlotGroupParent;
                Job job2 = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                if (job2 != null)
                {
                    return job2;
                }
            }

            foodSource = FoodUtility_Dragon.BestFoodSourceOnMap_Dragon(pawn, pawn, desperate, out foodDef, FoodPreferability.MealLavish, allowPlant: false, !pawn.IsTeetotaler(), allowCorpse: false, allowDispenserFull: false, allowDispenserEmpty: false, allowForbidden: false, allowSociallyImproper: false, allowHarvest: false, forceScanWholeMap);
            if (foodSource != null && foodSource is Pawn prey && prey.Faction != null)
            {
                foodSource = null;
            }
            if (foodSource == null)
            {
                return null;
            }
        }
        float nutrition = FoodUtility_Dragon.GetNutrition(pawn, foodSource, foodDef);
        Pawn pawn3 = (foodSource.ParentHolder as Pawn_InventoryTracker)?.pawn;
        if (pawn3 != null && pawn3 != pawn)
        {
            Job job3 = JobMaker.MakeJob(JobDefOf.TakeFromOtherInventory, foodSource, pawn3);
            job3.count = FoodUtility_Dragon.WillIngestStackCountOf(pawn, foodDef, nutrition);
            return job3;
        }
        Job job4 = JobMaker.MakeJob(JobDefOf.Ingest, foodSource);
        job4.count = FoodUtility_Dragon.WillIngestStackCountOf(pawn, foodDef, nutrition);
        return job4;
    }


}
