//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Verse;
//using Verse.AI;
//using Verse.Sound;

//namespace DD
//{
//    public class DraconicLightning : AbstractEffect
//    {
//        private bool DoorPresent => Map.thingGrid.ThingsListAt(Position).OfType<Building_Door>().Any();

//        protected override float InitialEffectSize => MaxEffectSize;
//        protected override float MinEffectSize => 0.2f;
//        protected override float MaxEffectSize => 20.0f;

//        protected float MaxDistance => (MaxEffectSize - MinEffectSize);

//        protected override int EffectTickInterval => 140;
//        protected override int EffectExpiryTicks => GenTicks.SecondsToTicks(10);

//        protected override float EffectGrowthRate => Mathf.LerpUnclamped(EffectSize, 0, TickExpiryProgression) - EffectSize;

//        protected override DamageDef EffectDamageDef => DD_DamageDefOf.DraconicFlame;

//        protected override SoundDef EffectSoundDef => SoundDefOf.FireBurning;

//        public override string Label => parent != null ? "FireOn".Translate(parent.LabelCap, parent).ToStringSafe() : def.label;
//        public override string InspectStringAddon => "Burning".Translate() + " (" + "FireSizeLower".Translate((EffectSize * 100f).ToString("F0")) + ")";

//        protected override RulePackDef DamageRulePack => RulePackDefOf.DamageEvent_Fire;

//        private bool VulnerableToRain()
//        {
//            if (!Spawned)
//            {
//                return false;
//            }
//            RoofDef roofDef = Map.roofGrid.RoofAt(Position);
//            if (roofDef == null)
//            {
//                return true;
//            }
//            if (roofDef.isThickRoof)
//            {
//                return false;
//            }
//            return Position.GetEdifice(Map)?.def.holdsRoof ?? false;
//        }

//        public override void AttachTo(Thing parent)
//        {
//            base.AttachTo(parent);
//            Pawn pawn = parent as Pawn;
//            if (pawn != null)
//            {
//                TaleRecorder.RecordTale(TaleDefOf.WasOnFire, pawn);
//            }
//        }

//        protected override float CalculateEffectDamage(Thing target)
//        {
//            return GenMath.RoundRandom(Mathf.Clamp01(0.1f + (0.9f * EffectSize)) * 5f);
//        }

//        protected override bool CanBeAffectedBy(Thing thing)
//        {
//            return EffectSize >= MinEffectSize && thing.FlammableNow;
//        }

//        protected override void DoEnvironmentEffects()
//        {
//            if (Map.weatherManager.RainRate > MinEffectSize && VulnerableToRain() && Rand.Chance(0.6f))
//            {
//                //parent.TakeDamage(new DamageInfo(DD_DamageDefOf.DraconicLightning, Map.weatherManager.RainRate * ChainedTargets.Count));
//            }

//            FindPawnsInCircuit(Map, Position, MaxDistance, (map, pos) => pos != Position && pos.GetTerrain(map).IsWater, (map, pos) => {
//                MoteMaker.MakeConnectingLine(Position.ToVector3(), pos.ToVector3(), ThingDefOf.Mote_LineEMP, map, EffectSize);
//            });

//            if (Position.GetTerrain(Map).IsWater)
//            {
//                //parent.TakeDamage(new DamageInfo(DD_DamageDefOf.DraconicLightning, EffectSize * ));

//                //EffectUtils.ApplyEffect(thing, def, Rand.Range(MinEffectSize, EffectSize));
//            }
//        }

//        private static void FindPawnsInCircuit(Map map, IntVec3 position, float distance, Func<Map, IntVec3, bool> validator, Action<Map, IntVec3> action)
//        {
//            Queue<IntVec3> positions = new Queue<IntVec3>();

//        }

//        protected override void TryAttachEffect(Thing thing)
//        {
//            //thing.TakeDamage(new DamageInfo(DD_DamageDefOf.DraconicLightning, EffectSize*));
//            EffectUtility.ApplyEffect(thing, def, EffectSize);
//        }

//        protected override void TrySpread(Thing thing)
//        {
//        }

//        protected override bool IsIncompatible(ThingDef def)
//        {
//            return false;
//        }
//    }
//}
