using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace DD;

public class CompProperties_Ability_Dragon_Fire : CompProperties_AbilityEffect
{
    public float range;

    public float lineWidthEnd;

    public ThingDef filthDef;

    public int damAmount = -1;

    public EffecterDef effecterDef;

    public bool canHitFilledCells;

    public CompProperties_Ability_Dragon_Fire()
    {
        compClass = typeof(CompAbilityEffect_Dragon_Fire);
    }
}