using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace DD
{
    public class Effect : AttachableThing, ISizeReporter
    {
        public EffectDef effectDef;
        public float size = 0;

        private int nextDamageTick, nextUpdateTick, nextSpreadTick;

        private Sustainer sustainer;

        public EffectDef EffectDef => effectDef;
        public float Size { get => size + deltaSize; set => deltaSize += value - Size; }
        private float deltaSize = 0;

        public JobDef ResponseJob
        {
            get
            {
                if (!effectDef.responses.NullOrEmpty())
                {
                    return effectDef.responses.Reverse<EffectResponseJob>().Where(r => r.minSize <= size).Select(r => r.job).FirstOrFallback();
                }
                else
                {
                    return null;
                }
            }
        }

        //Random
        public int TickPerUpdate => EffectDef.ticksPerUpdate.RandomInRange;
        public int TickPerDamage => EffectDef.ticksPerDamage.RandomInRange;
        public int TickPerSpread => EffectDef.ticksPerSpread.RandomInRange;
        public float UpdateSize => EffectDef.sizeChangePerUpdate.RandomInRange;
        public int SpreadCount => EffectDef.spreadCount.RandomInRange;

        public override string LabelMouseover => base.LabelMouseover + ": " + Size;

        public override string InspectStringAddon => effectDef.LabelCap + ": " + Size;

        private float ___CurrentSize => (EffectDef.effectSize.ClampToRange(Size) / EffectDef.effectSize.TrueMax) * Fire.MaxFireSize;
        public float CurrentSize() => float.IsNaN(___CurrentSize) ? 0 : ___CurrentSize;

        protected virtual DamageInfo CreateDamageInfo => new DamageInfo(EffectDef.damageDef, EffectUtils.SizeToDamage(EffectDef, Size), instigator: this);

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            RecalcPathsOnAndAroundMe(map);

            nextUpdateTick = TickPerUpdate;
            nextSpreadTick = TickPerSpread;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (sustainer != null)
            {
                if (sustainer.externalParams.sizeAggregator == null)
                {
                    sustainer.externalParams.sizeAggregator = new SoundSizeAggregator();
                }
                sustainer.externalParams.sizeAggregator.RemoveReporter(this);
            }

            if (parent is Pawn pawn)
            {
                JobDef response = ResponseJob;
                if (response != null && pawn.CurJobDef == response)
                {
                    //Response to effect.
                    pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
            }

            Map map = Map;
            base.DeSpawn(mode);
            RecalcPathsOnAndAroundMe(map);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref effectDef, "effectDef");
            Scribe_Values.Look(ref size, "size");
            Scribe_Values.Look(ref deltaSize, "deltaSize");
            Scribe_Values.Look(ref nextDamageTick, "nextDamageTick");
            Scribe_Values.Look(ref nextSpreadTick, "nextSpreadTick");
            Scribe_Values.Look(ref nextUpdateTick, "nextUpdateTick");
        }

        public override void Tick()
        {
            if (sustainer == null || sustainer.Ended)
            {
                if (EffectDef.soundDef != null && !Position.Fogged(Map))
                {
                    sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, EffectDef.soundDef, SoundInfo.InMap(new TargetInfo(Position, Map), MaintenanceType.PerTick));
                }
            }
            else
            {
                sustainer.Maintain();
            }

            if (parent is Pawn pawn)
            {
                if (pawn.Dead)
                {
                    EffectUtils.ApplyEffect(new TargetInfo(Position, Map), EffectDef, Size);
                    Destroy();
                    return;
                }

                if (!pawn.Downed)
                {
                    JobDef response = ResponseJob;
                    if (response != null && pawn.CurJobDef != response)
                    {
                        //Response to effect.
                        pawn.jobs.StartJob(JobMaker.MakeJob(response), JobCondition.InterruptForced);
                    }
                }
            }

            //Effect-effect interactions
            if (nextUpdateTick <= 0)
            {
                AddSize(UpdateSize);

                if (parent != null)
                {
                    //Do Effect Interactions.
                    HandleEffectInteractions();
                }

                ApplySizeUpdates();

                //Check if effect should be removed.
                if (ShouldRemove(Size))
                {
                    Destroy();
                    return;
                }

                nextUpdateTick = TickPerUpdate;
            }
            else
            {
                nextUpdateTick--;
            }

            //Give damage to stuff.
            if (nextDamageTick <= 0)
            {
                DoDamage();

                nextDamageTick = TickPerDamage;
            }
            else
            {
                nextDamageTick--;
            }

            //Try to spread (if possible)
            if (nextSpreadTick <= 0)
            {
                TrySpread();

                nextSpreadTick = TickPerSpread;
            }
            else
            {
                nextSpreadTick--;
            }
        }

        private void RecalcPathsOnAndAroundMe(Map map)
        {
            foreach (IntVec3 cell in GenAdj.AdjacentCellsAndInside.Select(cell => cell + Position).Where(cell => cell.InBounds(map)))
            {
                map.pathing.RecalculatePerceivedPathCostAt(cell);
            }
        }

        protected virtual void DoDamage()
        {
            //Damage to all things on cell
            EffectInfo einfo = new EffectInfo(this);
            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(Position, Map, EffectDef.damageRadius.RandomInRange, true).Where(thing => EffectUtils.CanDamage(einfo, thing)))
            {
                thing.TakeDamage(CreateDamageInfo);
            }
        }

        protected virtual void TrySpread()
        {
            //Try to spawn on the position or adjacent cells
            int spreadCount = SpreadCount;
            EffectInfo einfo = new EffectInfo(this);

            foreach (TargetInfo targetCell in GenRadial.RadialCellsAround(Position, EffectDef.spreadRadius.RandomInRange, true).Where(cell => cell.InBounds(Map)).Select(cell => new TargetInfo(cell, Map)).InRandomOrder())
            {
                if (spreadCount < 0)
                {
                    break;
                }

                if (EffectUtils.TrySpreadTo(einfo, targetCell))
                {
                    spreadCount--;
                }
            }

        }

        protected virtual void HandleEffectInteractions()
        {
            if (!EffectDef.boostedBy.NullOrEmpty())
            {
                foreach (Effect other in EffectDef.boostedBy.Select(def => parent.GetAttachment(def.thingDef)).OfType<Effect>().Where(other => other != this && Size >= other.Size))
                {
                    //Consumed other event and became bigger.
                    AddSize(EffectDef.spreadSize.Evaluate(Size));
                }
            }

            if (!EffectDef.reducedBy.NullOrEmpty())
            {
                foreach (Effect other in EffectDef.reducedBy.Select(def => parent.GetAttachment(def.thingDef)).OfType<Effect>().Where(other => other != this && Size >= other.Size))
                {
                    //Consumed other effect and became smaller.
                    AddSize(-EffectDef.spreadSize.Evaluate(Size));
                }
            }
        }

        public bool ShouldRemove(float size)
        {
            if (float.IsNaN(size))
            {
                Log.Error("Invalid effect size at " + parent.ToStringSafe());
                return true;
            }

            if (EffectDef.underMinimum == OutOfRangeAction.Remove && size < EffectDef.effectSize.TrueMin)
            {
                return true;
            }

            if (EffectDef.overMaximum == OutOfRangeAction.Remove && size > EffectDef.effectSize.TrueMax)
            {
                return true;
            }

            return false;
        }

        public float AddSize(float size)
        {
            float excess = 0;
            Size += size;

            if (Size < effectDef.effectSize.TrueMin)
            {
                excess = effectDef.effectSize.TrueMin - Size;
                switch (EffectDef.underMinimum)
                {
                    case OutOfRangeAction.Clamp:
                        Size = effectDef.effectSize.ClampToRange(Size);
                        break;
                    case OutOfRangeAction.Remove:
                        //Size = 0;
                        break;
                }
            }

            if (Size > effectDef.effectSize.TrueMax)
            {
                excess = Size - effectDef.effectSize.TrueMax;
                switch (EffectDef.overMaximum)
                {
                    case OutOfRangeAction.Clamp:
                        Size = effectDef.effectSize.ClampToRange(Size);
                        break;
                    case OutOfRangeAction.Remove:
                        //Size = 0;
                        break;
                }
            }

            return excess;
        }

        public void ApplySizeUpdates()
        {
            size += deltaSize;
            deltaSize = 0;
        }
    }
}
