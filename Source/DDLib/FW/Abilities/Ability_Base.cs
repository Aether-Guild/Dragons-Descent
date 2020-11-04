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
    public class Ability_Base : Ability
    {
        public Ability_Base(Pawn pawn) : base(pawn)
        {
        }

        public Ability_Base(Pawn pawn, AbilityDef def) : base(pawn, def)
        {
        }

        public virtual bool CanAutoCast => AbilityControl?.AutoUse ?? false;

        protected IEnumerable<AbilityComp_Base> BaseAbilityComps => CompsOfType<AbilityComp_Base>() ?? new AbilityComp_Base[0];
        protected AbilityComp_AbilityControl AbilityControl => CompOfType<AbilityComp_AbilityControl>();

        public override bool CanCast
        {
            get
            {
                if (!base.CanCast)
                {
                    return false;
                }

                foreach (AbilityComp_Base comp in BaseAbilityComps)
                {
                    if(!comp.CanCast)
                    {
                        return false;
                    }
                }
                
                return true;
            }
        }
        
        public virtual bool CanShowGizmos => pawn.FactionOrExtraMiniOrHomeFaction != null && pawn.FactionOrExtraMiniOrHomeFaction.IsPlayer && !pawn.InMentalState;


        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if(CooldownTicksRemaining > 0)
            {
                return false;
            }

            foreach (AbilityComp_Base comp in BaseAbilityComps)
            {
                if(!comp.CanActivateOn(target, dest))
                {
                    return false;
                }
            }

            return base.Activate(target, dest);
        }

        public override void AbilityTick()
        {
            base.AbilityTick();

            foreach (AbilityComp_Base comp in BaseAbilityComps)
            {
                comp.PostTick();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            foreach(AbilityComp_Base comp in BaseAbilityComps)
            {
                comp.PostExposeData();
            }
        }

        public virtual void Reset()
        {
            StartCooldown(0);

            foreach (AbilityComp_Base comp in BaseAbilityComps)
            {
                comp.PostReset();
            }
        }

        public override IEnumerable<Command> GetGizmos()
        {
            foreach (Command cmd in base.GetGizmos())
            {
                yield return cmd;
            }

            foreach (AbilityComp_Base comp in BaseAbilityComps)
            {
                if (comp.Gizmo != null)
                {
                    yield return comp.Gizmo;
                }
            }

            //yield return new Command_Action()
            //{
            //    defaultLabel = "Stop",
            //    defaultDesc = "Cancel the current job.",
            //    action = () =>
            //    {
            //        pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            //    }
            //};
        }
    }
}
