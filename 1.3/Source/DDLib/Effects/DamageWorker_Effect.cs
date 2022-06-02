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
    public class DamageWorker_ApplyEffect : DamageWorker_AddInjury
    {
        public EffectDef Def => def.GetModExtension<EffectDefExtension>().def;

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            if (Def.causesSlowdown)
            {
                if (victim is Pawn && victim.Faction != null && victim.Faction.IsPlayer)
                {
                    //Signal slowdown.
                    Find.TickManager.slower.SignalForceNormalSpeedShort();
                }
            }

            DamageResult result = new DamageResult();
            if (!(victim.DestroyedOrNull() || victim is Mote))
            {
                EffectInfo einfo = new EffectInfo(Def, EffectUtils.DamageToSize(Def, dinfo.Amount));
                if (EffectUtils.CanDamage(einfo, victim))
                {
                    result = base.Apply(dinfo, victim);
                    einfo = new EffectInfo(Def, EffectUtils.DamageToSize(Def, result.totalDamageDealt));
                }
                EffectUtils.TryAffect(einfo, victim);
            }

            return result;
        }

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
