//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Verse;

//namespace DD
//{

//    public enum RoundType
//    {
//        Wave, Spray, Random
//    }

//    public enum TargetingType
//    {
//        Initial, Following, Leading
//    }

//    public class VerbProperties_Breath : VerbProperties
//    {
//        public RoundType roundType;
//        public TargetingType targetingType;
//    }

//    public class Verb_Breath_Cone : Verb_Breath
//    {
//        protected override int ShotsPerBurst
//        {
//            get
//            {
//                ShotReport report = ShotReport.HitReportFor(Caster, this, CurrentTarget);

//                float angleDelta = report.AimOnTargetChance / 2;

//                Vector3 mag = Caster.TrueCenter() - CurrentTarget.CenterVector3;

//                float angle = Mathf.Atan2(mag.z, mag.x);

//                float minAngle = angle - angleDelta;
//                float maxAngle = angle + angleDelta;

//                for (int shotsFired = 0; shotsFired < verbProps.burstShotCount; shotsFired++)
//                {
//                    for (int c = 0; c <= shotsFired; c++)
//                    {
//                        float targetAngle = Mathf.Lerp(minAngle, maxAngle, (float)c / ((float)shotsFired + 1f)) * Mathf.PI;

//                        IntVec3 target = new IntVec3(
//                            //Mathf.FloorToInt(((float)Caster.Position.x) - (Mathf.Cos(targetAngle) * (float)shotsFired) - (float)(Caster.Position.x - CurrentTarget.Thing.Position.x) / 2f),
//                            Mathf.FloorToInt(((float)Caster.Position.x) - (Mathf.Cos(targetAngle) * (float)shotsFired) - verbProps.EffectiveMinRange(true)),
//                            CurrentTarget.Thing.Position.y,
//                            //Mathf.FloorToInt(((float)Caster.Position.z) - (Mathf.Sin(targetAngle) * (float)shotsFired) - (float)(Caster.Position.z - CurrentTarget.Thing.Position.z) / 2f)
//                            Mathf.FloorToInt(((float)Caster.Position.z) - (Mathf.Sin(targetAngle) * (float)shotsFired) - verbProps.EffectiveMinRange(true))
//                        );

//                        if (target.InBounds(Caster.Map) && target.CanBeSeenOverFast(Caster.Map))
//                        {
//                            targetQueue.Enqueue(target);
//                        }
//                    }
//                }
//            }
//        }

//        public override void WarmupComplete()
//        {
//            base.WarmupComplete();
//        }

//        protected override bool TryFireRound(Thing launcher, ThingDef projectileDef, int round, LocalTargetInfo target, ProjectileHitFlags flags)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class Verb_Breath_Line : Verb_Breath
//    {

//    }

//    public abstract class Verb_Breath : Verb
//    {
//        private VerbProperties_Breath Props => verbProps as VerbProperties_Breath;

//        protected RoundType Round => Props?.roundType ?? RoundType.Wave;
//        protected TargetingType Targeting => Props?.targetingType ?? TargetingType.Initial;

//        private int iteration = 0;

//        public virtual ThingDef Projectile
//        {
//            get
//            {
//                if (EquipmentSource != null)
//                {
//                    CompChangeableProjectile comp = EquipmentSource.GetComp<CompChangeableProjectile>();
//                    if (comp != null && comp.Loaded)
//                    {
//                        return comp.Projectile;
//                    }
//                }
//                return verbProps.defaultProjectile;
//            }
//        }

//        public override bool Available()
//        {
//            if (!base.Available())
//            {
//                return false;
//            }

//            if (CasterIsPawn)
//            {
//                Pawn casterPawn = CasterPawn;
//                if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
//                {
//                    return false;
//                }
//            }
//            return Projectile != null;
//        }

//        protected override bool TryCastShot()
//        {
//            if (currentTarget.HasThing && currentTarget.Thing.Map != Caster.Map)
//            {
//                return false;
//            }

//            ThingDef projectileDef = Projectile;
//            if (projectileDef == null)
//            {
//                return false;
//            }

//            ShootLine resultingLine;
//            if (!TryFindShootLineFromTo(Caster.Position, currentTarget, out resultingLine) && verbProps.stopBurstWithoutLos)
//            {
//                return false;
//            }

//            Thing launcher = Caster;
//            Vector3 drawPos = Caster.DrawPos;

//            ProjectileHitFlags flags = ProjectileHitFlags.IntendedTarget;
//            if (canHitNonTargetPawnsNow)
//            {
//                flags |= ProjectileHitFlags.NonTargetPawns;
//            }

//            if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
//            {
//                flags |= ProjectileHitFlags.NonTargetWorld;
//            }

//            if (currentTarget.Thing != null)
//            {
//                return TryFireRound(launcher, projectileDef, iteration++, CurrentTarget, flags);
//            }

//            return false;

//                //if (currentTarget.Thing != null)
//                //{
//                //    float angleDelta = report.AimOnTargetChance / 2;

//                //    Vector3 mag = Caster.TrueCenter() - CurrentTarget.CenterVector3;

//                //    float angle = Mathf.Atan2(mag.z, mag.x);

//                //    float minAngle = angle - angleDelta;
//                //    float maxAngle = angle + angleDelta;

//                //    int shotsFired = verbProps.burstShotCount - burstShotsLeft;

//                //    for (int c = 0; c <= shotsFired; c++)
//                //    {
//                //        Projectile projectile = GenSpawn.Spawn(projectileDef, resultingLine.Source, caster.Map) as Projectile;

//                //        float targetAngle = Mathf.Lerp(minAngle, maxAngle, (float)c / ((float)shotsFired + 1f));

//                //        IntVec3 target = new IntVec3(
//                //            Mathf.FloorToInt(((float)Caster.Position.x) - (Mathf.Cos(targetAngle) * (float)shotsFired) - (float)(Caster.Position.x - CurrentTarget.Thing.Position.x) / 2f),
//                //            CurrentTarget.Thing.Position.y,
//                //            Mathf.FloorToInt(((float)Caster.Position.z) - (Mathf.Sin(targetAngle) * (float)shotsFired) - (float)(Caster.Position.z - CurrentTarget.Thing.Position.z) / 2f)
//                //        );

//                //        projectile.Launch(launcher, drawPos, target, CurrentTarget, flags);
//                //    }
//                //}
//        }

//        protected abstract bool TryFireRound(Thing launcher, ThingDef projectileDef, int round, LocalTargetInfo target, ProjectileHitFlags flags);

//        public override void WarmupComplete()
//        {
//            iteration = 0;

//            ShotReport report = ShotReport.HitReportFor(caster, this, currentTarget);

//            float angleDelta = report.AimOnTargetChance / 2;

//            Vector3 mag = Caster.TrueCenter() - CurrentTarget.CenterVector3;

//            float angle = Mathf.Atan2(mag.z, mag.x);

//            float minAngle = angle - angleDelta;
//            float maxAngle = angle + angleDelta;

//            for(int shotsFired = 0; shotsFired < verbProps.burstShotCount; shotsFired++)
//            {
//                for (int c = 0; c <= shotsFired; c++)
//                {
//                    float targetAngle = Mathf.Lerp(minAngle, maxAngle, (float)c / ((float)shotsFired + 1f)) * Mathf.PI;

//                    IntVec3 target = new IntVec3(
//                        //Mathf.FloorToInt(((float)Caster.Position.x) - (Mathf.Cos(targetAngle) * (float)shotsFired) - (float)(Caster.Position.x - CurrentTarget.Thing.Position.x) / 2f),
//                        Mathf.FloorToInt(((float)Caster.Position.x) - (Mathf.Cos(targetAngle) * (float)shotsFired) - verbProps.EffectiveMinRange(true)),
//                        CurrentTarget.Thing.Position.y,
//                        //Mathf.FloorToInt(((float)Caster.Position.z) - (Mathf.Sin(targetAngle) * (float)shotsFired) - (float)(Caster.Position.z - CurrentTarget.Thing.Position.z) / 2f)
//                        Mathf.FloorToInt(((float)Caster.Position.z) - (Mathf.Sin(targetAngle) * (float)shotsFired) - verbProps.EffectiveMinRange(true))
//                    );

//                    if(target.InBounds(Caster.Map) && target.CanBeSeenOverFast(Caster.Map))
//                    {
//                        targetQueue.Enqueue(target);
//                    }
//                }
//            }

//            base.WarmupComplete();
//            //Find.BattleLog.Add(new BattleLogEntry_RangedFire(caster, currentTarget.HasThing ? currentTarget.Thing : null, (base.EquipmentSource != null) ? base.EquipmentSource.def : null, Projectile, ShotsPerBurst > 1));
//        }

//        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
//        {
//            needLOSToCenter = true;
//            return Projectile?.projectile.explosionRadius ?? 0f;
//        }

//        public override void ExposeData()
//        {
//            base.ExposeData();
//            Scribe_Values.Look(ref iteration, "iteration", 0);
//        }

//        public override void DrawHighlight(LocalTargetInfo target)
//        {
//            base.DrawHighlight(target);
//        }

//        public override void Reset()
//        {
//            base.Reset();
//            iteration = 0;
//        }
//    }
//}
