using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace DD;

public class CompAbilityEffect_FireBurst_Dragon : CompAbilityEffect
{
    private new CompProperties_AbilityFireBurst_Dragon Props => (CompProperties_AbilityFireBurst_Dragon)props;

    private Pawn Pawn => parent.pawn;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        GenExplosion.DoExplosion(Pawn.Position, Pawn.MapHeld, Props.radius, DamageDefOf.Flame, Pawn, -1, -1f, null, null, null, null, ThingDefOf.Filth_Fuel, 1f, 1, null, null, 255, applyDamageToExplosionCellsNeighbors: false, null, 0f, 1, 1f, damageFalloff: false, null, null, null, doVisualEffects: false, 0.6f);
        base.Apply(target, dest);
    }

    public override IEnumerable<PreCastAction> GetPreCastActions()
    {
        yield return new PreCastAction
        {
            action = delegate
            {
                parent.AddEffecterToMaintain(DD_EffecterDefOf.Fire_Burst_Dragon.Spawn(parent.pawn.Position, parent.pawn.Map), parent.pawn.Position, 17, parent.pawn.Map);
            },
            ticksAwayFromCast = 17
        }; 
    }
    // Vaporize_Heatwave Fire_Burst Fire_Burst_Dragon
    public override bool AICanTargetNow(LocalTargetInfo target)
    {
        if (Pawn.Faction == Faction.OfPlayer)
        {
            return false;
        }
        if (target.HasThing && target.Thing is Pawn pawn)
        {
            return pawn.TargetCurrentlyAimingAt == Pawn;
        }
        return false;
    }

    public override void CompTickInterval(int delta)
    {
        if (parent.Casting)
        {
            FireBurstUtility.ThrowFuelTick(Pawn.Position, Props.radius, Pawn.Map);
        }
    }
}