using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    //public class Verb_Breath : Verb
    //{
    //    public bool HasProps => verbProps is VerbProperties_Breath;
    //    public VerbProperties_Breath Props => verbProps as VerbProperties_Breath;

    //    private List<TargetInfo> targetList = new List<TargetInfo>();

    //    public TargetInfo Target => CurrentTarget.IsValid ? CurrentTarget.ToTargetInfo(Caster.Map) : CurrentDestination.IsValid ? CurrentDestination.ToTargetInfo(Caster.Map) : TargetInfo.Invalid;

    //    protected override int ShotsPerBurst => verbProps.burstShotCount;
    //    protected int ShotsFired => ShotsPerBurst - burstShotsLeft + 1;

    //    public float GetAngle(TargetInfo target)
    //    {
    //        if (!HasProps || Props.angle == null)
    //        {
    //            return Mathf.PI / 2;
    //        }

    //        float angle = Props.angle.Evaluate(0);
    //        if (target.IsValid)
    //        {
    //            angle = Props.angle.Evaluate(Vector3.Distance(Caster.Position.ToVector3Shifted(), target.Cell.ToVector3Shifted()));
    //        }

    //        return ((angle + 360) % 360) * Mathf.Deg2Rad;
    //    }

    //    public float GetDamageAmount(IntVec3 cell)
    //    {
    //        if (!HasProps || Props.damageAmount == null)
    //        {
    //            return verbProps.meleeDamageBaseAmount;
    //        }

    //        float damage = Props.damageAmount.Evaluate(0);
    //        if (cell.IsValid)
    //        {
    //            damage = Props.damageAmount.Evaluate(Vector3.Distance(Caster.Position.ToVector3Shifted(), cell.ToVector3Shifted()));
    //        }

    //        return damage;
    //    }

    //    protected float GetCurrentShotRange(TargetInfo target)
    //    {
    //        float range = verbProps.range;

    //        if (HasProps && Props.effectiveRange != null)
    //        {
    //            range = Props.effectiveRange.Evaluate(target.Cell.DistanceTo(Caster.Position));
    //        }

    //        return Mathf.Lerp(0f, range, (float)ShotsFired / (float)ShotsPerBurst);
    //    }

    //    protected override bool TryCastShot()
    //    {
    //        if (targetList.NullOrEmpty())
    //        {
    //            return false;
    //        }

    //        TargetInfo target = Target;
    //        if (!target.IsValid)
    //        {
    //            return false;
    //        }

    //        if (target.HasThing && target.Thing.Map != Caster.Map)
    //        {
    //            return false;
    //        }

    //        if (target.HasThing && !CasterPawn.CanSee(target.Thing) && verbProps.stopBurstWithoutLos)
    //        {
    //            return false;
    //        }

    //        float iterationRange = GetCurrentShotRange(target);
    //        List<IntVec3> leanCells = new List<IntVec3>();
    //        ShootLeanUtility.LeanShootingSourcesFromTo(Caster.Position, target.Cell, target.Map, leanCells);

    //        IEnumerable<IntVec3> destinationCells = targetList.Where(t => InBurstRange(t, iterationRange) && HasLOS(t, leanCells)).Select(t => t.Cell);

    //        if (verbProps.impactMote != null)
    //        {
    //            Mote_ParticleController mote = (Mote_ParticleController)ThingMaker.MakeThing(verbProps.impactMote);
    //            mote.exactPosition = Caster.Position.ToVector3();
    //            mote.Setup(Caster.Position, destinationCells, verbProps.ticksBetweenBurstShots.TicksToSeconds());
    //            GenSpawn.Spawn(mote, Caster.Position, Caster.Map);
    //        }

    //        foreach (IntVec3 cell in destinationCells)
    //        {
    //            ApplyDamage(cell);
    //        }

    //        if (ShotsFired == ShotsPerBurst)
    //        {
    //            targetList.Clear();
    //        }

    //        return true;
    //    }

    //    public override void WarmupComplete()
    //    {
    //        RecalculateTargetCells();
    //        base.WarmupComplete();
    //    }

    //    public override void ExposeData()
    //    {
    //        base.ExposeData();
    //        Scribe_Collections.Look(ref targetList, "targetList", LookMode.TargetInfo);
    //        if (targetList == null)
    //        {
    //            targetList = new List<TargetInfo>();
    //        }
    //    }

    //    public override void Reset()
    //    {
    //        base.Reset();
    //        targetList.Clear();
    //    }

    //    protected bool InBurstRange(TargetInfo target, float iterationRange)
    //    {
    //        return Mathf.Round(Vector3.Distance(Caster.Position.ToVector3Shifted(), target.Cell.ToVector3Shifted())) <= iterationRange;
    //    }

    //    protected bool HasLOS(TargetInfo target, List<IntVec3> leanCells = null)
    //    {
    //        if (GenSight.LineOfSight(Caster.Position, target.Cell, Caster.Map, true))
    //        {
    //            return true;
    //        }

    //        if (!leanCells.NullOrEmpty())
    //        {
    //            foreach (IntVec3 c in leanCells)
    //            {
    //                if (GenSight.LineOfSight(c, target.Cell, target.Map, true))
    //                {
    //                    return true;
    //                }
    //            }
    //        }

    //        return false;
    //    }

    //    protected void ApplyDamage(IntVec3 cell)
    //    {
    //        List<Thing> things = cell.GetThingList(Caster.Map).ToList();
    //        if (!things.NullOrEmpty())
    //        {
    //            float damageAmount = GetDamageAmount(cell);
    //            Thing intendedTarget = null;
    //            if (Target.HasThing && !Target.ThingDestroyed)
    //            {
    //                intendedTarget = Target.Thing;
    //            }
    //            things.RemoveAll(thing => thing == Caster); //Avoid self attack.

    //            foreach (Thing thing in things)
    //            {
    //                thing.TakeDamage(new DamageInfo(Props.damageDef, damageAmount, instigator: Caster, intendedTarget: intendedTarget));
    //            }
    //        }
    //    }

    //    private void RecalculateTargetCells()
    //    {
    //        targetList.Clear();

    //        TargetInfo target = Target;

    //        float targetAngle = Mathf.Atan2(target.Cell.z - Caster.Position.ToVector3Shifted().z, target.Cell.x - Caster.Position.ToVector3Shifted().x);
    //        if (targetAngle < 0)
    //        {
    //            targetAngle = (2f * Mathf.PI) + targetAngle;
    //        }

    //        float angleSize = GetAngle(target) / 2f;

    //        Vector3 casterVector = Caster.Position.ToVector3Shifted();

    //        //Actual boundaries
    //        FloatRange angleRange = new FloatRange(targetAngle - angleSize, targetAngle + angleSize);
    //        Caster.Map.floodFiller.FloodFill(Caster.Position, cell => cell.IsValid && cell.InBounds(Caster.Map) && Mathf.Round(Vector3.Distance(Caster.Position.ToVector3Shifted(), cell.ToVector3Shifted())) <= verbProps.range, cell => SelectCell(cell, angleRange, Caster.Position.ToVector3Shifted()));
    //    }

    //    private void SelectCell(IntVec3 cell, FloatRange targetAngle, Vector3 casterVector)
    //    {
    //        Vector3 cellVector = cell.ToVector3Shifted();

    //        if (Vector3.Distance(casterVector, cellVector) < Props.EffectiveMinRange(true))
    //        {
    //            //Skip min range.
    //            return;
    //        }

    //        //Relevant points
    //        float
    //            fx = Mathf.Floor(cellVector.x - casterVector.x),
    //            cx = Mathf.Ceil(cellVector.x - casterVector.x),
    //            fz = Mathf.Floor(cellVector.z - casterVector.z),
    //            cz = Mathf.Ceil(cellVector.z - casterVector.z);

    //        //Cell angles
    //        float[] angles = {
    //            Mathf.Atan2(cz, cx),
    //            Mathf.Atan2(cz, fx),
    //            Mathf.Atan2(fz, cx),
    //            Mathf.Atan2(fz, fx)
    //        };

    //        //Adjusting
    //        for (int i = 0; i < angles.Length; i++)
    //        {
    //            if (angles[i] < 0)
    //            {
    //                angles[i] = (2f * Mathf.PI) + angles[i];
    //            }
    //        }

    //        if (angles.Min() <= targetAngle.TrueMin && targetAngle.TrueMax <= angles.Max())
    //        {
    //            //Accepted
    //            targetList.Add(new TargetInfo(cell, Caster.Map));
    //            return;
    //        }

    //        for (int i = 0; i < angles.Length; i++)
    //        {
    //            if (targetAngle.Includes(angles[i]))
    //            {
    //                //Accepted
    //                targetList.Add(new TargetInfo(cell, Caster.Map));
    //                return;
    //            }
    //        }
    //    }
    //}
}
