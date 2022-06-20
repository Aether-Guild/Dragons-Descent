using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public abstract class DamageWorker_AbstractEffect : DamageWorker_AddInjury
    {

        protected virtual FloatRange EffectSize => new FloatRange(1.5f, 2.5f);
        protected virtual FloatRange ExplosionEffectSize => new FloatRange(2f, 6f);

        protected abstract ThingDef EffectDef { get; }

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            if (victim is Pawn && victim.Faction != null && victim.Faction.IsPlayer)
            {
                //Signal slowdown.
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }

            return !victim.DestroyedOrNull() ? ApplyEffect(dinfo, victim) : new DamageResult();
        }

        public override void ExplosionAffectCell(Explosion explosion, IntVec3 cell, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
        {
            base.ExplosionAffectCell(explosion, cell, damagedThings, ignoredThings, canThrowMotes);

            if (!Immune(cell, explosion.Map) && Rand.Chance(ChanceToAttachEffect(cell, explosion.Map)))
            {
                TryAttachEffect(cell, explosion.Map, ExplosionEffectSize.RandomInRange);
            }
        }

        protected DamageResult ApplyEffect(DamageInfo dinfo, Thing victim)
        {
            Map map = victim.Map;
            DamageResult result = base.Apply(dinfo, victim);

            if (!Immune(victim) && !result.deflected && !dinfo.InstantPermanentInjury && Rand.Chance(ChanceToAttachEffect(victim)))
            {
                //Apply effect on the thing.
                TryAttachEffect(victim, EffectSize.RandomInRange);
            }

            if (victim.Destroyed && map != null)
            {
                OnVictimDestroyed(victim, map);
            }

            return result;
        }

        protected virtual void OnVictimDestroyed(Thing victim, Map map)
        {
            foreach (IntVec3 item in victim.OccupiedRect())
            {
                FilthMaker.TryMakeFilth(item, map, ThingDefOf.Filth_Ash);
            }

            Plant plant = victim as Plant;
            if (plant != null && victim.def.plant.IsTree && plant.LifeStage != 0 && victim.def != ThingDefOf.BurnedTree)
            {
                DeadPlant deadplant = (DeadPlant)GenSpawn.Spawn(ThingDefOf.BurnedTree, victim.Position, map);
                deadplant.Growth = plant.Growth;
            }
        }

        protected virtual bool HasEffect(IntVec3 cell, Map map) => cell.GetThingList(map).OfType<AbstractEffect>().Any();

        protected virtual void TryAttachEffect(Thing victim, float effectSize) => EffectUtility.ApplyEffect(victim, EffectDef, effectSize);
        protected virtual void TryAttachEffect(IntVec3 cell, Map map, float effectSize) => EffectUtility.ApplyEffect(cell, map, EffectDef, effectSize);

        protected virtual bool Immune(Thing victim)
        {
            //bool immune = !victim.DestroyedOrNull() && (victim.FireBulwark || victim.Position.GetThingList(victim.Map).Where(t => t.FireBulwark).Any());

            //return victim.GetStatValue(StatDefOf.Flammability) > 0.01f && !immune;
            return !victim.FlammableNow;
        }
        protected virtual bool Immune(IntVec3 cell, Map map)
        {
            //TerrainDef def = cell.GetTerrain(map);
            //bool immune = cell.GetThingList(map).Where(t => t.FireBulwark).Any();

            //return def.GetStatValueAbstract(StatDefOf.Flammability) > 0.01f && !immune && !def.extinguishesFire;
            return cell.GetTerrain(map).Flammable();

        }

        protected virtual float ChanceToAttachEffectTo(Thing thing) => thing.GetStatValue(StatDefOf.Flammability);
        protected virtual float ChanceToAttachEffectTo(IntVec3 cell, Map map) => cell.GetTerrain(map).GetStatValueAbstract(StatDefOf.Flammability);

        protected virtual float ChanceToAttachEffect(Thing thing) => thing.Spawned && HasEffect(thing.Position, thing.Map) ? 0f : 1f;
        protected virtual float ChanceToAttachEffect(IntVec3 cell, Map map)
        {
            if (HasEffect(cell, map))
            {
                //Already active.
                return 0f;
            }

            //Terrain chances
            float chance = ChanceToAttachEffectTo(cell, map);

            IEnumerable<Thing> things = cell.GetThingList(map);

            if (things.Where(t => t.def.category != ThingCategory.Pawn).Any())
            {
                //Chances of the things in the cell.
                float maxThingChance = things.Max(t => ChanceToAttachEffectTo(t));

                //Get the maximum chance.
                chance = Mathf.Max(chance, maxThingChance);
            }

            if (chance > 0f)
            {
                Building edifice = cell.GetEdifice(map);
                if (edifice != null && edifice.def.passability == Traversability.Impassable && edifice.OccupiedRect().ContractedBy(1).Contains(cell))
                {
                    return 0f;
                }

                if (things.Where(t => t.def.category == ThingCategory.Filth && !t.def.filth.allowsFire).Any())
                {
                    return 0f;
                }
            }
            return chance;
        }
    }
}
