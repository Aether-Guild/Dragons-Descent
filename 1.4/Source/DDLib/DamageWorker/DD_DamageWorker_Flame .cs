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
    public class DD_DamageWorker_Flame : DamageWorker_AddInjury
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            Pawn pawn = victim as Pawn;
            if (pawn != null && pawn.Faction == Faction.OfPlayer)
            {
                Find.TickManager.slower.SignalForceNormalSpeedShort();
            }
            Map map = victim.Map;
            DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
            if (!damageResult.deflected && !dinfo.InstantPermanentInjury && Rand.Chance(DD_FireUtility.ChanceToAttachFireFromEvent(victim)))
            {
                victim.TryAttachFire(Rand.Range(0.15f, 0.25f));
            }
            if (victim.Destroyed && map != null && pawn == null)
            {
                foreach (IntVec3 c in victim.OccupiedRect())
                {
                    FilthMaker.TryMakeFilth(c, map, ThingDefOf.Filth_Ash, 1, FilthSourceFlags.None, true);
                }
                Plant plant;
                if ((plant = (victim as Plant)) != null && plant.LifeStage != PlantLifeStage.Sowing)
                {
                    plant.TrySpawnStump(PlantDestructionMode.Flame);
                }
            }
            return damageResult;
        }

        public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
        {
            base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
            if (this.def == DD_DamageDefOf.DraconicFlame && Rand.Chance(FireUtility.ChanceToStartFireIn(c, explosion.Map)))
            {
                DD_FireUtility.TryStartFireIn(c, explosion.Map, Rand.Range(0.2f, 0.6f));
            }

        }

    }
}
