using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using RimWorld.Planet;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;



namespace DD;

public class CompProperties_AbilityBurner_DragonFire : CompProperties_AbilityEffect
{
    public ThingDef moteDef;

    public int numStreams;

    public float coneSizeDegrees;

    public float range;

    public float rangeNoise;

    public float barrelOffsetDistance;

    public int lifespanNoise;

    public float sizeReductionDistanceThreshold;

    public EffecterDef effecterDef;

    public CompProperties_AbilityBurner_DragonFire()
    {
        compClass = typeof(CompAbilityEffect_Burner_DragonFire);
    }
}
public class CompProperties_AbilityBurner_DragonNecro : CompProperties_AbilityEffect
{
    public ThingDef moteDef;

    public int numStreams;

    public float coneSizeDegrees;

    public float range;

    public float rangeNoise;

    public float barrelOffsetDistance;

    public int lifespanNoise;

    public float sizeReductionDistanceThreshold;

    public EffecterDef effecterDef;

    public CompProperties_AbilityBurner_DragonNecro()
    {
        compClass = typeof(CompAbilityEffect_Burner_DragonNecro);
    }
}
