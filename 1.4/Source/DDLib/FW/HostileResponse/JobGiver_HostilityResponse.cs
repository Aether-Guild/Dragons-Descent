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
    //public class JobGiver_HostilityResponse : ThinkNode_JobGiver
    //{
    //    protected override Job TryGiveJob(Pawn pawn)
    //    {
    //        if (pawn.DestroyedOrNull())
    //        {
    //            return null;
    //        }

    //        CompHostileResponse comp = pawn.GetComp<CompHostileResponse>();

    //        if (comp == null)
    //        {
    //            //Needs CompHostileResponse.
    //            return null;
    //        }

    //        if (comp.Type == HostilityResponseType.Passive)
    //        {
    //            return null;
    //        }

    //        if (!pawn.Awake() || pawn.IsBurning() || pawn.Drafted || pawn.Downed || pawn.Dead)
    //        {
    //            //Disabled for inactive, burning, drafted, downed or dead.
    //            return null;
    //        }

    //        if (pawn.WorkTagIsDisabled(WorkTags.Violent))
    //        {
    //            //Can't attack.
    //            return null;
    //        }

    //        if (pawn.jobs.startingNewJob)
    //        {
    //            return null;
    //        }

    //        if (pawn.IsFighting() || pawn.stances.FullBodyBusy)
    //        {
    //            //Is fighting, or is busy.
    //            return null;
    //        }

    //        if(!PawnUtility.EnemiesAreNearby(pawn))
    //        {
    //            //Needs enemies nearby.
    //            return null;
    //        }
    //        if (pawn.jobs.jobQueue.Any(j => j.job.def == JobDefOf.AttackMelee || j.job.def == JobDefOf.AttackStatic))
    //        {
    //            return null;
    //        }
    //        if (PawnUtility.PlayerForcedJobNowOrSoon(pawn) || !pawn.jobs.IsCurrentJobPlayerInterruptible())
    //        {
    //            //Job is uninterruptable or uninterruptable.
    //            return null;
    //        }

    //        IAttackTarget target = comp.PreferredTarget;

    //        if(target == null || target.Thing == null)
    //        {
    //            //No targets.
    //            return null;
    //        }

    //        Thing thing = target.Thing;

    //        Verb verb = pawn.TryGetAttackVerb(thing);
    //        if (verb == null)
    //        {
    //            //Can't pick a verb??? We should've been able to...
    //            return null;
    //        }

    //        Job job;

    //        if (verb.IsMeleeAttack)
    //        {
    //            job = JobMaker.MakeJob(JobDefOf.AttackMelee, thing);
    //            job.maxNumMeleeAttacks = 2;
    //        }
    //        else
    //        {
    //            job = JobMaker.MakeJob(JobDefOf.AttackStatic, thing);
    //            job.maxNumStaticAttacks = 3;
    //            job.endIfCantShootInMelee = verb.verbProps.EffectiveMinRange(thing, pawn) > 1.0f;
    //            pawn.jobs.StartJob(job, JobCondition.InterruptForced);
    //        }
    //        job.verbToUse = verb;
    //        job.expireRequiresEnemiesNearby = true;
    //        //job.killIncappedTarget = comp.Type == HostilityResponseType.Aggressive;
    //        job.playerForced = true;
    //        job.expiryInterval = GenTicks.TickLongInterval;

    //        return job;
    //    }
    //}
}
