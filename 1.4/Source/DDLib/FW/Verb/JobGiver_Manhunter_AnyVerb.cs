using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    public class JobGiver_Manhunter_AnyVerb : JobGiver_Manhunter
    {
        public float castRangeFactor = 0.95f;
        public float castCoverRange = .7f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            Verb verb = pawn.CurrentEffectiveVerb;
            if (verb != null && !verb.IsMeleeAttack && verb.Available())
            {
                //MethodInfo searchMethod = AccessTools.Method(typeof(JobGiver_Manhunter), "FindPawnTarget", new Type[] { typeof(Pawn) });
                //Pawn target = searchMethod.Invoke(this, new object[] { pawn }) as Pawn;

                Pawn target = AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedThreat | TargetScanFlags.NeedLOSToAll | TargetScanFlags.NeedAutoTargetable, (Thing x) => x is Pawn && (int)x.def.race.intelligence >= 1, canBashDoors: true, canBashFences: true) as Pawn;

                CastPositionRequest request = new CastPositionRequest()
                {
                    caster = pawn,
                    target = target,
                    verb = verb,
                    wantCoverFromTarget = verb.verbProps.range > castCoverRange,
                    maxRangeFromCaster = verb.verbProps.range * castRangeFactor,
                    maxRangeFromTarget = verb.verbProps.range * castRangeFactor
                };

                if (target != null)
                {
                    if (verb.CanHitTargetFrom(pawn.Position, target))
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.AttackStatic, target);
                        job.maxNumStaticAttacks = 3;
                        job.expiryInterval = Rand.Range(200, 900);
                        job.endIfCantShootInMelee = true;
                        job.ignoreForbidden = true;
                        job.killIncappedTarget = false;
                        job.canUseRangedWeapon = true;
                        job.verbToUse = pawn.TryGetAttackVerb(target, true) ?? verb;
                        return job;
                    }
                    else if (CastPositionFinder.TryFindCastPosition(request, out IntVec3 shootPos))
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.Goto, shootPos);
                        job.locomotionUrgency = LocomotionUrgency.Sprint;
                        return job;
                    }
                }
            }
            return base.TryGiveJob(pawn);
        }
    }
}
