using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    //public class CompHostileResponse : ThingComp
    //{
    //    public CompProperties_HostileResponse Props => (CompProperties_HostileResponse)props;
    //    public IAttackTargetSearcher Self => (IAttackTargetSearcher)parent;
    //    public Pawn SelfPawn => (Pawn)parent;

    //    private Dictionary<string, HostilityStatisticRecord> stats = new Dictionary<string, HostilityStatisticRecord>();

    //    private HostilityResponseType type;
    //    public HostilityResponseType Type
    //    {
    //        get
    //        {
    //            if (parent is Pawn pawn)
    //            {
    //                if (pawn.InAggroMentalState || pawn.Faction.HostileTo(Faction.OfPlayer))
    //                {
    //                    return HostilityResponseType.Aggressive;
    //                }

    //                if (pawn.InMentalState)
    //                {
    //                    return Props.initialHostility;
    //                }
    //            }
    //            return type;
    //        }
    //        set
    //        {
    //            type = value;
    //        }
    //    }

    //    public bool CanInteractGizmo => parent.Faction != null && parent.Faction.IsPlayer && parent is Pawn pawn && pawn.InMentalState;

    //    private Dictionary<HostilityResponseType, Command_Action> gizmos = new Dictionary<HostilityResponseType, Command_Action>();
    //    public Command_Action Gizmo
    //    {
    //        get
    //        {
    //            Command_Action cmd = null;

    //            List<HostileResponseOption> options = Props.options;
    //            int index = options.FindIndex(o => o.type == Type);
    //            HostileResponseOption option = options[index];

    //            if (!gizmos.ContainsKey(Type))
    //            {
    //                //Not created yet, so create it.
    //                cmd = new Command_Action()
    //                {
    //                    defaultLabel = option.label,
    //                    defaultDesc = option.description,
    //                    icon = option.Texture,
    //                    action = () =>
    //                    {
    //                        Type = options[(index + 1) % options.Count].type;
    //                    }
    //                };
    //                gizmos.Add(Type, cmd);
    //            }
    //            else
    //            {
    //                //Load it from the cache.
    //                cmd = gizmos[Type];
    //            }

    //            //Check whether it needs to be updated.
    //            if (CanInteractGizmo == !cmd.disabled)
    //            {
    //                //If you should be able to interact with the gizmo, then it shouldn't be disabled and vice versa.
    //                cmd.disabled = false;

    //                if (!CanInteractGizmo)
    //                {
    //                    cmd.Disable(option.disableMessage);
    //                }
    //            }

    //            return cmd;
    //        }
    //    }

    //    //Target selection
    //    public IEnumerable<IAttackTarget> Targets => stats.Values.Select(r => r.Target).Where(IsValidTarget);
    //    public IEnumerable<IAttackTarget> TargetsPreferredOrder => stats.Where(entry => IsValidTarget(entry.Value.Target)).Select(entry => entry.Value).OrderByDescending(record => record.CalculatePoints(Type) / Self.Thing.Position.DistanceTo(record.Target.Thing.Position)).Select(record => record.Target);
    //    public IAttackTarget PreferredTarget
    //    {
    //        get
    //        {
    //            IAttackTarget target = null;
    //            CastPositionRequest request = new CastPositionRequest()
    //            {
    //                caster = SelfPawn
    //            };

    //            Pair<IAttackTarget, float> entry = TargetsPreferredOrder.Select(t => CalculateScore(t, request)).RandomElementByWeightWithFallback(p => p.Second);

    //            if (entry != null && entry.First != null)
    //            {
    //                //Fightback.
    //                target = entry.First;
    //            }
    //            else
    //            {
    //                //No targets in history.
    //                IAttackTarget t = AttackTargetFinder.BestAttackTarget(
    //                    SelfPawn,
    //                    TargetScanFlags.NeedThreat | TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedLOSToAll | TargetScanFlags.LOSBlockableByGas,
    //                    thing =>
    //                    {
    //                        switch (Type)
    //                        {
    //                            case HostilityResponseType.Aggressive:
    //                                //Always valid
    //                                return true;
    //                            case HostilityResponseType.Defensive:
    //                                //Valid if aiming at you
    //                                IAttackTarget targ = thing as IAttackTarget;
    //                                return targ.TargetCurrentlyAimingAt == SelfPawn;
    //                        }

    //                        return false;
    //                    }
    //                );
    //                if (t != null)
    //                {
    //                    target = t;
    //                }
    //            }


    //            return target;
    //        }
    //    }

    //    private bool IsValidTarget(IAttackTarget target)
    //    {
    //        if (target == null)
    //        {
    //            return false;
    //        }

    //        if (target.Thing.DestroyedOrNull())
    //        {
    //            return false;
    //        }

    //        if (target.Thing.Map != parent.Map)
    //        {
    //            //Ignore off-map
    //            return false;
    //        }
    //        if (!SelfPawn.CanSee(target.Thing))
    //        {
    //            return false;
    //        }
    //        if (target.Thing is Pawn pawn)
    //        {
    //            //Aggressive goes for the kill.
    //            if (pawn.Downed && Type != HostilityResponseType.Aggressive)
    //            {
    //                return false;
    //            }

    //            if (pawn.Dead)
    //            {
    //                return false;
    //            }

    //        }

    //        if (target.ThreatDisabled(Self))
    //        {
    //            return false;
    //        }

    //        return true;
    //    }

    //    /// <summary>
    //    /// <para>Calculates score for this pawn to attack the specified target and returns the score of the verb that returned the largest score value.</para>
    //    /// <para>When scoring, this pawn will try getting a cast position for each verb it has available and cast positions that are further away from its current position will be scored lower.</para>
    //    /// <para>Note: If the cast position is equal to your current location then the score is positive infinity and if no score could be calculated then it returns negative infinity.</para>
    //    /// </summary>
    //    /// <param name="target">The target being evaluated against.</param>
    //    /// <param name="request">Optional: A request object to use/reuse.</param>
    //    /// <returns>Pair Object with the target and their top score.</returns>
    //    private Pair<IAttackTarget, float> CalculateScore(IAttackTarget target, CastPositionRequest request = new CastPositionRequest())
    //    {
    //        float maxScore = float.NegativeInfinity;

    //        if (Self.Thing is Pawn pawn)
    //        {
    //            IEnumerable<Verb> verbs = VerbUtils.GetPossibleVerbs(pawn);

    //            request.target = target.Thing;

    //            foreach (Verb v in verbs)
    //            {
    //                request.verb = v;
    //                if (CastPositionFinder.TryFindCastPosition(request, out IntVec3 p))
    //                {
    //                    float distance = Self.Thing.Position.DistanceTo(p);
    //                    float score = 0;

    //                    if (distance != 0)
    //                    {
    //                        score = GetTargetPoints(target) / distance; //Score = points / distance between current and firing position
    //                        score *= v.CalculateCommonality(pawn, target.Thing); //Score is scaled by commonality. (Zero commonality would result in zero score for this verb)

    //                        if (float.IsPositiveInfinity(score))
    //                        {
    //                            score = float.MaxValue;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        score = float.MaxValue;
    //                    }

    //                    if (maxScore < score)
    //                    {
    //                        maxScore = score;
    //                    }
    //                }
    //            }
    //        }

    //        return new Pair<IAttackTarget, float>(target, Mathf.Clamp(maxScore, 0, float.MaxValue));
    //    }

    //    /// <summary>
    //    /// <para>Calculates the points to be assigned to each target.</para>
    //    /// <para>Note: Returns 0 when target is not logged, and max value when the target is standing on us.</para>
    //    /// </summary>
    //    /// <param name="target">Target to be evaluated.</param>
    //    /// <returns>float value depicting 'how threatening' the target is, based on damage done to us and distance between us.</returns>
    //    private float GetTargetPoints(IAttackTarget target)
    //    {
    //        if (!stats.Keys.Contains(target.Thing.ThingID))
    //        {
    //            //No info about target...
    //            return 0f;
    //        }

    //        HostilityStatisticRecord record = stats[target.Thing.ThingID];

    //        float distance = Self.Thing.Position.DistanceTo(record.Target.Thing.Position);
    //        if (distance != 0)
    //        {
    //            //Target is on a different cell than us.
    //            return record.CalculatePoints(Type) / distance;
    //        }
    //        else
    //        {
    //            //Target is standing on us.
    //            return float.MaxValue;
    //        }
    //    }

    //    public override void Initialize(CompProperties props)
    //    {
    //        base.Initialize(props);
    //        type = Props.initialHostility;
    //    }

    //    public override void PostExposeData()
    //    {
    //        Scribe_Values.Look(ref type, "responseType", HostilityResponseType.Passive);
    //        Scribe_Collections.Look(ref stats, "responseStats", LookMode.Value, LookMode.Deep);

    //        if (stats == null)
    //        {
    //            stats = new Dictionary<string, HostilityStatisticRecord>();
    //        }
    //    }

    //    public override void PostDestroy(DestroyMode mode, Map previousMap)
    //    {
    //        //Remove all targets
    //        stats.Clear();
    //    }

    //    public override void PostDeSpawn(Map map)
    //    {
    //        //Remove all targets
    //        stats.Clear();
    //    }

    //    public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    //    {
    //        //Add target
    //        if (dinfo.Amount <= 0)
    //        {
    //            return;
    //        }

    //        if (dinfo.Instigator != null && dinfo.Instigator is IAttackTarget target)
    //        {
    //            if (target == null)
    //            {
    //                return;
    //            }

    //            if (dinfo.IntendedTarget == Self)
    //            {
    //                //Intending to attack this pawn.

    //                if (Type != HostilityResponseType.Passive)
    //                {
    //                    if (target.Thing.Faction != null && target.Thing.Faction == SelfPawn.Faction)
    //                    {
    //                        if(target.Thing is Pawn pawn && pawn.InMentalState)
    //                        {
    //                            //Ignore friendly damage if currently in mental state.
    //                            return;
    //                        }

    //                        //Pawn is not wild, and is part of the same faction.
    //                        if (Props.friendlyFireMentalState != null)
    //                        {
    //                            SelfPawn.mindState.mentalStateHandler.TryStartMentalState(Props.friendlyFireMentalState, "HostileResponseFriendlyFireMessage".Translate(SelfPawn.Named("PAWN"), target.Named("ATTACKER")), forceWake: true);
    //                        }
    //                    }

    //                    ProcessDamage(target, dinfo);
    //                    return;
    //                }
    //            }

    //            if (target.Thing.HostileTo(SelfPawn))
    //            {
    //                //Hostile to pawn.
    //                ProcessDamage(target, dinfo);
    //                return;
    //            }
    //        }
    //    }

    //    private void ProcessDamage(IAttackTarget target, DamageInfo dinfo)
    //    {
    //        if (!stats.ContainsKey(target.Thing.ThingID))
    //        {
    //            stats.Add(target.Thing.ThingID, new HostilityStatisticRecord(target));
    //        }

    //        stats[target.Thing.ThingID].ProcessAttack(dinfo.Amount, dinfo.IntendedTarget == null || parent == dinfo.IntendedTarget);
    //    }

    //    public override void Notify_KilledPawn(Pawn pawn)
    //    {
    //        //Remove target
    //        stats.Remove(pawn.ThingID);
    //    }

    //    public override string CompInspectStringExtra()
    //    {
    //        if (Props.debug)
    //        {
    //            //Show message when targets are available
    //            int count = Targets.Count();
    //            if (count > 0)
    //            {
    //                switch (Type)
    //                {
    //                    case HostilityResponseType.Aggressive:
    //                        return "Hostility: Aggressive [" + count + "]";
    //                    case HostilityResponseType.Defensive:
    //                        return "Hostility: Defensive [" + count + "]";
    //                    case HostilityResponseType.Passive:
    //                        return "Hostility: Passive [" + count + "]";
    //                    default:
    //                        return "Hostility: Unknown [" + count + "]";
    //                }
    //            }
    //        }
    //        return base.CompInspectStringExtra();
    //    }

    //    public override void CompTickRare()
    //    {
    //        //Check targets and remove as needed
    //        if (stats.RemoveAll(entry => entry.Value.Target == null || entry.Value.Target.Thing.DestroyedOrNull() || SelfPawn.Map != entry.Value.Target.Thing.Map) > 0)
    //        {
    //            if (SelfPawn.CurJobDef == JobDefOf.AttackMelee || SelfPawn.CurJobDef == JobDefOf.AttackStatic)
    //            {
    //                //Is already fighting, so update the attack target.
    //                IAttackTarget target = PreferredTarget;

    //                if (target != null)
    //                {
    //                    //Update the current job's target.
    //                    SelfPawn.CurJob.targetA = target.Thing;
    //                }
    //                else
    //                {
    //                    //No more targets.
    //                    SelfPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
    //                }
    //            }
    //        }
    //    }

    //    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    //    {
    //        //Show state toggle gizmo
    //        if (Gizmo != null && Props.controllableGizmo)
    //        {
    //            yield return Gizmo;
    //        }
    //    }
    //}
}
