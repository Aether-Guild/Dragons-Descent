using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;



namespace DD;



public class CompProperties_AbilityFireBurst_Dragon : CompProperties_AbilityEffect
{
    public float radius = 6f;

    public CompProperties_AbilityFireBurst_Dragon()
    {
        compClass = typeof(CompAbilityEffect_FireBurst_Dragon);
    }
}