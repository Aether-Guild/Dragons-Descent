//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using Verse;
//using Verse.Sound;

//namespace DD
//{
//    public class DraconicWind : AbstractEffect
//    {
//        private bool DoorPresent => Map.thingGrid.ThingsListAt(Position).OfType<Building_Door>().Any();

//        protected override float InitialEffectSize => MaxEffectSize;
//        protected override float MinEffectSize => 0.2f;
//        protected override float MaxEffectSize => 20.0f;

//        protected override int EffectTickInterval => 140;
//        protected override int EffectExpiryTicks => GenTicks.SecondsToTicks(30 * 60);

//        protected override float EffectGrowthRate => Mathf.LerpUnclamped(EffectSize, 0, TickExpiryProgression) - EffectSize;

//        protected override DamageDef EffectDamageDef => DD_DamageDefOf.DraconicFlame;

//        //protected override ThinkTreeDef ThinkTreeOverride => DD_ThinkTreeDefOf.DraconicBurnResponse;

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
//            float generatedHeat = 100f * EffectSize;

//            if (DoorPresent)
//            {
//                generatedHeat *= 0.15f;
//            }

//            GenTemperature.PushHeat(Position, Map, generatedHeat);

//            if (Rand.Value < 0.4f)
//            {
//                float radius = EffectSize * 2f;
//                SnowUtility.AddSnowRadial(Position, Map, radius, -EffectSize * 0.1f);
//            }

//            if (Map.weatherManager.RainRate > MinEffectSize && VulnerableToRain() && Rand.Chance(0.6f))
//            {
//                TakeDamage(new DamageInfo(DamageDefOf.Extinguish, Map.weatherManager.RainRate));
//            }

//            if (Position.GetTerrain(Map).extinguishesFire)
//            {
//                //TakeDamage(new DamageInfo(DamageDefOf.Extinguish, 10));
//                Extinguish(EffectSize);
//            }
//        }

//        protected override void TryAttachEffect(Thing thing)
//        {
//            EffectUtils.ApplyEffect(thing, def, Rand.Range(MinEffectSize, EffectSize));
//        }

//        protected override void TrySpread(Thing thing)
//        {
//        }

//        protected override bool IsIncompatible(ThingDef def)
//        {
//            return (def == DD_EffectDefOf.DraconicFrost);
//        }
//    }
//}
