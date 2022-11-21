using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DD
{
    public enum OutOfRangeAction
    {
        Nothing, Remove, Clamp
    }

    public enum EffectTargets
    {
        None = 0x00,
        Pawns = 0x01,
        Corpse = 0x02,
        Plants = 0x04,
        Buildings = 0x08,
        Terrain = 0x10,

        Things = 0x0F,
        NotPawn = 0x1E,
    }

    public class EffectResponseJob
    {
        public float minSize = 0f;
        public JobDef job;
    }

    public class EffectDef : Def
    {
        public ThingDef thingDef;

        public DamageDef damageDef;
        public BodyPartHeight damageHeight = BodyPartHeight.Undefined;
        public BodyPartDepth damageDepth = BodyPartDepth.Undefined;

        public SoundDef soundDef;

        public EffectCondition affectCondition, damageCondition, spreadCondition;

        public FloatRange sizeChangePerUpdate;
        public IntRange ticksPerUpdate, ticksPerDamage, ticksPerSpread;
        public IntRange spreadCount = IntRange.zero;

        public IntRange damageRadius = IntRange.one, spreadRadius = IntRange.one;

        public bool causesSlowdown = true, spawnPostMortem = true, consumesFire = false;

        public SimpleCurve damagePerSize, spreadSize;

        public FloatRange effectSize;
        public OutOfRangeAction underMinimum = OutOfRangeAction.Remove, overMaximum = OutOfRangeAction.Clamp;

        public List<EffectDef> boostedBy, reducedBy;

        public List<EffectResponseJob> responses;
    }

    public class JobDriver_RandomFlee : JobDriver_Flee
    {
        public const int FleeRange = 15;

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            if (!job.GetTarget(DestInd).IsValid)
            {
                job.SetTarget(DestInd, CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, FleeRange));
            }
        }
    }
}
