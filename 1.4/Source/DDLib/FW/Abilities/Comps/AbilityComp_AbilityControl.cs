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
    public class AbilityComp_AbilityControl : AbilityComp_Base
    {
        private bool status = true;
        private Command gizmo;
        private Texture2D iconOn, iconOff;

        public AbilityCompProperties_AbilityControl VProps => props as AbilityCompProperties_AbilityControl;

        public bool AutoUse => VProps.autoUse && Status;
        public bool Controllable => VProps.abilityControllable;

        public override bool CanCast => true;


        public bool Status
        {
            get
            {
                Pawn pawn = parent.pawn;

                if (pawn.Faction == null || !pawn.Faction.IsPlayer)
                {
                    return true;
                }

                if (pawn.InMentalState)
                {
                    return true;
                }
                return status;
            }
            set
            {
                if (status != value)
                {
                    status = value;

                    if (!value)
                    {
                        //Only when disabled.
                        if (parent.Casting || (parent is Ability_Base ability && ability.CastingOrSubCasting))
                        {
                            parent.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        }
                        parent.pawn.jobs.ClearQueuedJobs();
                    }
                }
            }
        }

        public override Command Gizmo
        {
            get
            {
                if (!VProps.abilityControllable)
                {
                    //Hide gizmo
                    return null;
                }

                if (gizmo == null)
                {
                    //Load images
                    if (VProps.gizmoOnIconPath != null)
                    {
                        iconOn = ContentFinder<Texture2D>.Get(VProps.gizmoOnIconPath);
                    }
                    if (VProps.gizmoOffIconPath != null)
                    {
                        iconOff = ContentFinder<Texture2D>.Get(VProps.gizmoOffIconPath);
                    }

                    if (iconOn == null)
                    {
                        //If one is unset, use the same image for both.
                        iconOn = iconOff;
                    }
                    if (iconOff == null)
                    {
                        //If one is unset, use the same image for both.
                        iconOff = iconOn;
                    }

                    gizmo = new Command_Toggle()
                    {
                        defaultDesc = VProps.gizmoDesc,
                        isActive = () => Status,
                        activateIfAmbiguous = true,
                        toggleAction = () => Status = !Status
                    };
                }

                gizmo.defaultLabel = Status ? VProps.gizmoOnText : VProps.gizmoOffText;
                gizmo.icon = Status ? iconOn : iconOff;

                gizmo.disabled = GizmoDisabled(out String reason);
                if (gizmo.disabled)
                {
                    gizmo.disabledReason = reason;
                }

                return gizmo;
            }
        }

        //public override void Initialize(AbilityCompProperties props)
        //{
        //    base.Initialize(props);
        //    if (!parent.VerbTracker.AllVerbs.NullOrEmpty())
        //    {
        //        foreach (IAttackVerb verb in parent.VerbTracker.AllVerbs.OfType<IAttackVerb>())
        //        {
        //            verb.Ability = parent;
        //        }
        //    }
        //}

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref status, "status", defaultValue: true);
        }
    }
}
