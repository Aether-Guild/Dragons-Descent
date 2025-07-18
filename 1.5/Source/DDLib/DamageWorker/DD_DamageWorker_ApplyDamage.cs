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

    public class DD_DamageWorker_ApplyDamage : DamageWorker_AddInjury
    {
        public EffectDef Def => def.GetModExtension<EffectDefExtension>().def;

        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            Pawn pawn = victim as Pawn;
            if (pawn != null && pawn.Faction == Faction.OfPlayer)
            {
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
            Map map = victim.Map;

            //Damage 
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
                if (dinfo.Amount <= 0f)
                {
                    return damageResult;
                }
            if (victim.Destroyed && map != null && pawn == null)
            {
                foreach (IntVec3 c in victim.OccupiedRect())
                {
                    FilthMaker.TryMakeFilth(c, map, ThingDefOf.Filth_Ash, 1, FilthSourceFlags.None);
                }
                Plant plant = victim as Plant;
                if (plant != null && plant.LifeStage != PlantLifeStage.Sowing && victim.def.plant.burnedThingDef != null)
                {
                    ((DeadPlant)GenSpawn.Spawn(victim.def.plant.burnedThingDef, victim.Position, map, WipeMode.Vanish)).Growth = plant.Growth;
                }
                EffectInfo einfo = new EffectInfo(Def, EffectUtils.DamageToSize(Def, dinfo.Amount));
                if (EffectUtils.CanDamage(einfo, victim))
                {
                    damageResult = base.Apply(dinfo, victim);
                    einfo = new EffectInfo(Def, EffectUtils.DamageToSize(Def, damageResult.totalDamageDealt));
                }
                EffectUtils.TryAffect(einfo, victim);

            }
            return damageResult;
        }

        //public override void ExplosionAffectCell(Explosion explosion, IntVec3 cell, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
        //{
        //    List<Thing> things = cell.GetThingList(explosion.Map);
        //    EffectInfo einfo = new EffectInfo(Def, explosion.GetDamageAmountAt(cell));
        //    if (this.def == DD_DamageDefOf.DraconicExplosion)
        //    {
        //        EffectUtils.TryAffect(einfo, new TargetInfo(cell, explosion.Map));
        //        base.ExplosionAffectCell(explosion, cell, damagedThings, ignoredThings, canThrowMotes);

        //    }
        //}
        public override void ExplosionAffectCell(Explosion explosion, IntVec3 cell, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
        {
            List<Thing> things = cell.GetThingList(explosion.Map);
            EffectInfo einfo = new EffectInfo(Def, explosion.GetDamageAmountAt(cell));

            if (!things.NullOrEmpty())
            {
                //Don't damage things that are immune to effect.
                ignoredThings = things.Where(thing => !EffectUtils.CanDamage(einfo, thing)).ToList();
            }

            EffectUtils.TryAffect(einfo, new TargetInfo(cell, explosion.Map));

            base.ExplosionAffectCell(explosion, cell, damagedThings, ignoredThings, canThrowMotes);
        }


    }
}
