using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using VFECore;
using VFECore.Abilities;
using RimWorld.Planet;
using Ability = VFECore.Abilities.Ability;


namespace DD

{
    public class Ability_ShootProjectile_Multiple_Dragon : Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            foreach (GlobalTargetInfo target in targets)
            {
                ShootProjectile(target);
            }
        }
        protected virtual Projectile ShootProjectile(GlobalTargetInfo target)
        {
            var extension = this.def.GetModExtension<AbilityExtension_Projectile_Multiple_Dragon>();
            var origin = pawn.DrawPosHeld ?? pawn.PositionHeld.ToVector3Shifted();
            var source = this.pawn.PositionHeld;

            Projectile projectile1 = GenSpawn.Spawn(extension.projectile, source, this.pawn.MapHeld) as Projectile;
            Projectile projectile2 = GenSpawn.Spawn(extension.projectile, source, this.pawn.MapHeld) as Projectile;
            Projectile projectile3 = GenSpawn.Spawn(extension.projectile, source, this.pawn.MapHeld) as Projectile;

            if (projectile1 is AbilityProjectile abilityProjectile1)
            {
                abilityProjectile1.ability = this;
            }
            if (projectile2 is AbilityProjectile abilityProjectile2)
            {
                abilityProjectile2.ability = this;
            }
            if (projectile3 is AbilityProjectile abilityProjectile3)
            {
                abilityProjectile3.ability = this;
            }

            if (extension.forcedMissRadius > 0.5f)
            {
                float forcedMissRadius = extension.forcedMissRadius;
                float adjustedForcedMiss = VerbUtility.CalculateAdjustedForcedMiss(forcedMissRadius, target.Cell - Caster.Position);
                if (adjustedForcedMiss > 0.5f)
                {

                    int max = GenRadial.NumCellsInRadius(forcedMissRadius);
                    IntVec3 forcedMissTarget = target.Cell + GenRadial.RadialPattern[Rand.Range(0, max)];
                    if (forcedMissTarget != target.Cell)
                    {
                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        if (Rand.Chance(0.5f)) { projectileHitFlags = ProjectileHitFlags.All; }

                        projectile1?.Launch(pawn, origin, forcedMissTarget, target.HasThing ? target.Thing : target.Cell, projectileHitFlags);
                        return projectile2;
                    }
                }
            }

            var accuracy = this.CalculateModifiedStatForPawn(1f, extension.accuracyStatFactors, extension.accuracyStatOffsets);
            if (Rand.Chance(accuracy))
            {
                if (target.HasThing)
                {
                    projectile1?.Launch(this.pawn, origin, target.Thing, target.Thing, extension.hitFlags);
                    projectile2?.Launch(this.pawn, origin, target.Thing, target.Thing, extension.hitFlags);
                    projectile3?.Launch(this.pawn, origin, target.Thing, target.Thing, extension.hitFlags);
                }
                else
                {
                    projectile1?.Launch(this.pawn, origin, target.Cell, target.Cell, extension.hitFlags);
                    projectile2?.Launch(this.pawn, origin, target.Cell, target.Cell, extension.hitFlags);
                    projectile3?.Launch(this.pawn, origin, target.Cell, target.Cell, extension.hitFlags);
                }
            }
            else
            {
                ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                var cell = ChangeDestToMissWild(target.Cell, source, accuracy);
                projectile1?.Launch(this.pawn, origin, cell, cell, projectileHitFlags);
                projectile2?.Launch(this.pawn, origin, cell, cell, projectileHitFlags);
                projectile3?.Launch(this.pawn, origin, cell, cell, projectileHitFlags);
            }
            return projectile1; // You can return either projectile1, projectile2, or projectile3
        }
        public IntVec3 ChangeDestToMissWild(IntVec3 dest, IntVec3 source, float aimOnChance)
        {
            float num = ShootTuning.MissDistanceFromAimOnChanceCurves.Evaluate(aimOnChance, Rand.Value);
            if (num < 0f)
            {
                Log.ErrorOnce("Attempted to wild-miss less than zero tiles away", 94302089);
            }
            IntVec3 intVec;
            do
            {
                Vector2 unitVector = Rand.UnitVector2;
                Vector3 vector = new Vector3(unitVector.x * num, 10f, unitVector.y * num);
                intVec = (dest.ToVector3Shifted() + vector).ToIntVec3();
            }
            while (Vector3.Dot((dest - source).ToVector3(), (intVec - source).ToVector3()) < 0f);
            return intVec;
        }


        public override void CheckCastEffects(GlobalTargetInfo[] targetInfos, out bool cast, out bool target, out bool hediffApply)
        {
            base.CheckCastEffects(targetInfos, out cast, out _, out _);
            target = false;
            hediffApply = false;
        }
    }

    public class AbilityExtension_Projectile_Multiple_Dragon : DefModExtension
    {
        public ThingDef projectile;
        public SoundDef soundOnImpact;
        public float forcedMissRadius;
        public ProjectileHitFlags hitFlags = ProjectileHitFlags.IntendedTarget;
        public List<StatModifier> accuracyStatFactors = new List<StatModifier>();
        public List<StatModifier> accuracyStatOffsets = new List<StatModifier>();
    }

   
    }

