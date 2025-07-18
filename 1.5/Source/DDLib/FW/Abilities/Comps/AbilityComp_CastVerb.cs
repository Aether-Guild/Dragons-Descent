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
    //public class AbilityComp_CastVerb : AbilityComp_Base, IVerbOwner
    //{
    //    private VerbTracker tracker;
    //
    //    private LocalTargetInfo target = LocalTargetInfo.Invalid, dest = LocalTargetInfo.Invalid;
    //
    //    public AbilityCompProperties_CastVerb VProps => props as AbilityCompProperties_CastVerb;
    //
    //    public VerbTracker VerbTracker
    //    {
    //        get
    //        {
    //            if (tracker == null)
    //            {
    //                tracker = new VerbTracker(this);
    //            }
    //            return tracker;
    //        }
    //    }
    //
    //    public bool Casting => tracker.AllVerbs.Any(verb => verb.WarmingUp);
    //    public bool Bursting => tracker.AllVerbs.Any(verb => verb.Bursting);
    //
    //    public List<VerbProperties> VerbProperties => VProps.verbProperties;
    //
    //    public List<Tool> Tools => VProps.tools;
    //
    //    public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;
    //
    //    public Thing ConstantCaster => parent.pawn;
    //
    //    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    //    {
    //        base.Apply(target, dest);
    //
    //        if (VerbTracker.AllVerbs.Any(v => v.Available() && v.IsUsableOn(target.Thing)))
    //        {
    //            this.target = target;
    //            this.dest = dest;
    //        }
    //        else
    //        {
    //            Log.Error(UniqueVerbOwnerID()+ ": Activated with no verbs available");
    //        }
    //    }
    //
    //    public string UniqueVerbOwnerID() => "AbilityComp_" + parent.UniqueVerbOwnerID() + "_Verbs";
    //
    //    public bool VerbsStillUsableBy(Pawn p) => !VerbTracker.AllVerbs.NullOrEmpty() && VerbTracker.AllVerbs.Any(verb => verb.Available());
    //
    //    public override bool CanCast => VerbTracker.AllVerbs.All(v => v.Available());
    //
    //    public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
    //    {
    //        return base.CanApplyOn(target, dest) && !VerbTracker.AllVerbs.NullOrEmpty() && VerbTracker.AllVerbs.Any(v => v.Available() && v.IsUsableOn(target.Thing));
    //    }
    //
    //    public override void PostTick()
    //    {
    //        VerbTracker.VerbsTick();
    //        if (target.IsValid || dest.IsValid)
    //        {
    //            Verb verb = VerbTracker.AllVerbs.Where(v => v.Available() && v.IsUsableOn(target.Thing)).OrderByDescending(v => v.verbProps.commonality).FirstOrFallback();
    //
    //            if (verb != null)
    //            {
    //                Job job = JobMaker.MakeJob(JobDefOf.UseVerbOnThingStatic, target);
    //                job.verbToUse = verb;
    //                job.expiryInterval = GenTicks.TickLongInterval;
    //                parent.pawn.jobs.StartJob(job, JobCondition.InterruptForced);
    //            }
    //            else
    //            {
    //                //Reset cooldown if secondary cast fails.
    //                parent.StartCooldown(0);
    //            }
    //
    //            target = LocalTargetInfo.Invalid;
    //            dest = LocalTargetInfo.Invalid;
    //        }
    //    }
    //
    //    public override void PostExposeData()
    //    {
    //        Scribe_Deep.Look(ref tracker, "castVerbs", ctorArgs: this);
    //        Scribe_TargetInfo.Look(ref target, "currentTarget", LocalTargetInfo.Invalid);
    //        Scribe_TargetInfo.Look(ref dest, "currentDestination", LocalTargetInfo.Invalid);
    //    }
    //}
}
