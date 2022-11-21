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
    public abstract class AbstractEffect : AttachableThing, ISizeReporter
    {
        public float effectSize = 0;

        public float EffectSize { get => effectSize; set => effectSize = Mathf.Clamp(value, 0, MaxEffectSize); }
        public int TicksSinceSpawn => ticks;

        private int ticks;
        private int ticksSinceSpread;

        private Sustainer sustainer;

        protected abstract float InitialEffectSize { get; }
        protected abstract float MinEffectSize { get; }
        protected abstract float MaxEffectSize { get; }

        protected abstract int EffectTickInterval { get; }

        protected virtual bool EffectExpiresAfterTicks => EffectExpiryTicks > 0;
        protected abstract int EffectExpiryTicks { get; }

        protected abstract float EffectGrowthRate { get; }

        protected abstract DamageDef EffectDamageDef { get; }
        protected virtual SoundDef EffectSoundDef => null;

        protected virtual float TickExpiryProgression => (float)ticks / (float)EffectExpiryTicks;

        protected virtual IEnumerable<Thing> AffectedThings => GenAdj.AdjacentCellsAndInside.Select(delta => Position + delta).Where(cell => cell.InBounds(Map)).Select(cell => cell.GetThingList(Map).ToList() as IEnumerable<Thing>).Aggregate((l1,l2) => l1.Concat(l2)).Where(t => !(t is AbstractEffect));
        protected virtual IEnumerable<Thing> SpreadTargets => GenAdj.AdjacentCells.Select(c => Position + c).Where(c => c.InBounds(Map)).Select(c => Map.thingGrid.ThingAt<Thing>(c)).Where(t => t != null && t != parent && t != this && CanBeAffectedBy(t)).ToList();

        protected virtual Thing AffectedThing => parent ?? AffectedThings.RandomElementWithFallback();
        protected virtual Thing SpreadTarget => SpreadTargets.RandomElementWithFallback();

        protected virtual bool SpreadEnabled => false;
        protected virtual bool CanSpread => false;
        protected virtual int SpreadInterval => -1;

        protected abstract RulePackDef DamageRulePack { get; }

        protected abstract float CalculateEffectDamage(Thing target);

        protected abstract bool CanBeAffectedBy(Thing thing);
        protected abstract void TryAttachEffect(Thing thing);

        protected abstract void DoEnvironmentEffects();

        protected virtual void TrySpread(Thing thing)
        {
            EffectUtility.ApplyEffect(thing.Position, thing.Map, def, Rand.Range(MinEffectSize, EffectSize));
        }

        public virtual bool Extinguishable => true;

        public virtual void Extinguish(float extinguishDamage)
        {
            EffectSize -= extinguishDamage;
            if (EffectSize < MinEffectSize)
            {
                Destroy();
            }
        }

        public virtual bool CanInteractWith(ThingDef def)
        {
            return IsCompatible(def) || IsIncompatible(def);
        }

        public virtual AbstractEffect MergeWith(ThingDef def, float size)
        {
            if (IsCompatible(def))
            {
                EffectSize += size;
            }

            if (IsIncompatible(def))
            {
                EffectSize -= size;
            }

            if (EffectSize >= MinEffectSize)
            {
                return this;
            }
            else
            {
                AbstractEffect effect = (AbstractEffect)ThingMaker.MakeThing(def);
                effect.EffectSize = EffectSize - MinEffectSize;
                GenSpawn.Spawn(effect, Position, Map);
                
                if(parent != null)
                {
                    effect.AttachTo(parent);
                }

                Destroy();

                return effect;
            }
        }

        protected virtual bool IsCompatible(ThingDef def) => def == this.def;
        protected virtual bool IsIncompatible(ThingDef def) => false;

        public virtual void Refresh()
        {
            ticks = 0;
        }

        public float CurrentSize()
        {
            return effectSize;
        }

        protected AbstractEffect()
        {
            EffectSize = InitialEffectSize;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticks, "ticks", 0);
            Scribe_Values.Look(ref ticksSinceSpread, "ticksSinceSpread", 0);
            Scribe_Values.Look(ref effectSize, "effectSize", InitialEffectSize);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            RecalcPathsOnAndAroundMe(map);
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

            Map map = Map;
            base.DeSpawn(mode);
            RecalcPathsOnAndAroundMe(map);
        }

        protected void RecalcPathsOnAndAroundMe(Map map)
        {
            foreach (IntVec3 cell in GenAdj.AdjacentCellsAndInside.Select(c => Position + c).Where(c => c.InBounds(map)))
            {
                //map.pathGrid.RecalculatePerceivedPathCostAt(cell);
            }
        }

        public override void Tick()
        {
            //ticks++;

            if (EffectSoundDef != null)
            {
                if (sustainer != null)
                {
                    sustainer.Maintain();
                }
                else if (!Position.Fogged(Map))
                {
                    SoundInfo info = SoundInfo.InMap(new TargetInfo(Position, Map), MaintenanceType.PerTick);
                    sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, EffectSoundDef, info);
                }
            }

            //Apply the effect.
            if (this.IsHashIntervalTick(EffectTickInterval))
            {
                DoEffectTick();
            }

            //Spread if possible.
            if (SpreadEnabled)
            {
                ticksSinceSpread++;

                if (CanSpread && this.IsHashIntervalTick(SpreadInterval))
                {
                    foreach(Thing thing in SpreadTargets)
                    {
                        TrySpread(thing);
                    }

                    ticksSinceSpread = 0;
                }
            }

            //Update size.
            if (EffectSize >= MinEffectSize)
            {
                EffectSize += EffectGrowthRate;
            }

            //Should destroy.
            if ((EffectExpiresAfterTicks && ticks >= EffectExpiryTicks) || EffectSize < MinEffectSize)
            {
                if(!Destroyed)
                {
                    Destroy();
                }
            }
        }

        protected virtual void DoEffectTick()
        {
            if(EffectDamageDef != null)
            {
                foreach(Thing thing in AffectedThings)
                {
                    if (thing != null && CanBeAffectedBy(thing))
                    {
                        //DoEffectDamage(thing);

                        if(parent == null)
                        {
                            TryAttachEffect(thing);
                        }
                    }
                }
            }

            //if (ThinkTreeOverride != null && parent is Pawn)
            //{
            //    Pawn pawn = parent as Pawn;

            //    if(!pawn.DestroyedOrNull() || !pawn.Downed)
            //    {
            //        ThinkResult result = ThinkTreeOverride.thinkRoot.TryIssueJobPackage(pawn, default(JobIssueParams));
            //        if (result.IsValid && (pawn.CurJob == null || result.Job.def != pawn.CurJobDef))
            //        {
            //            Log.Message("Override: " + pawn);
            //            result.Job.playerForced = true;
            //            pawn.jobs.StartJob(result.Job, JobCondition.InterruptForced);
            //        }
            //    }
            //}

            if (Spawned)
            {
                DoEnvironmentEffects();
            }
        }

        protected virtual void DoEffectDamage(Thing targ)
        {
            DamageInfo dinfo = new DamageInfo(EffectDamageDef, CalculateEffectDamage(targ), instigator: this);
            Pawn pawn = targ as Pawn;

            if (pawn != null)
            {
                BattleLogEntry_DamageTaken entry = new BattleLogEntry_DamageTaken(pawn, DamageRulePack);
                Find.BattleLog.Add(entry);
                dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                DamageWorker.DamageResult result = targ.TakeDamage(dinfo);
                result.AssociateWithLog(entry);

                if (pawn.apparel != null && pawn.apparel.WornApparel.TryRandomElement(out Apparel apparel))
                {
                    apparel.TakeDamage(dinfo);
                }
            }
            else
            {
                targ.TakeDamage(dinfo);
            }
        }
    }
}
