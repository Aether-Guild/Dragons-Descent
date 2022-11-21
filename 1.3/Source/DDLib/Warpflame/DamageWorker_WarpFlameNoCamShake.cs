/* using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace DD
{
    public class DamageWorker_WarpFlameNoCamShake : DamageWorker_AddInjury
    {
        // Token: 0x06004AA6 RID: 19110 RVA: 0x0022CA48 File Offset: 0x0022AE48
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            Pawn pawn = victim as Pawn;
            if (pawn != null && pawn.Faction == Faction.OfPlayer)
            {
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
            Map map = victim.Map;
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (!damageResult.deflected && !dinfo.InstantPermanentInjury)
            {
                Rand.PushState();
                WarpfireUtility.TryAttachWarpfire(victim, Rand.Range(0.15f, 0.25f));
                Rand.PopState();
            }
            if (victim.Destroyed && map != null && pawn == null)
            {
                foreach (IntVec3 c in victim.OccupiedRect())
                {
                    FilthMaker.TryMakeFilth(c, map, ThingDefOf.Filth_Ash, 1);
                }
                Plant plant = victim as Plant;
                if (plant != null && victim.def.plant.IsTree && plant.LifeStage != PlantLifeStage.Sowing && victim.def != ThingDefOf.BurnedTree)
                {
                    DeadPlant deadPlant = (DeadPlant)GenSpawn.Spawn(ThingDefOf.BurnedTree, victim.Position, map, WipeMode.Vanish);
                    deadPlant.Growth = plant.Growth;
                }
            }
            return damageResult;
        }
        public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
        {
            base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
            Rand.PushState();
            if ((this.def == DD_WarpfireDamageDefOf.OG_Chaos_Deamon_Warpfire || this.def == DD_WarpfireDamageDefOf.OG_WarpStormStrike) && Rand.Chance(WarpfireUtility.ChanceToStartWarpfireIn(c, explosion.Map)))
            {
                WarpfireUtility.TryStartWarpfireIn(c, explosion.Map, Rand.Range(0.2f, 0.6f));
            }
            Rand.PopState();
        }

        public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (this.def.explosionHeatEnergyPerCell > 1.401298E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map, this.def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
            }
            FleckMaker.Static(explosion.DrawPos, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
        }
    }
}
 */