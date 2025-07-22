using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace DD;

public class CompProperties_AbilityFireSpew_Dragon : CompProperties_AbilityEffect
{
    public float range;

    public float lineWidthEnd;

    public ThingDef filthDef;

    public int damAmount = -1;

    public EffecterDef effecterDef;

    public bool canHitFilledCells;

    public CompProperties_AbilityFireSpew_Dragon()
    {
        compClass = typeof(CompAbilityEffect_FireSpew_Dragon);
    }
}