using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
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

        protected IEnumerable<AbilityComp_Base> BaseAbilityComps => CompsOfType<AbilityComp_Base>() ?? new AbilityComp_Base[0];
        public AbilityComp_AbilityControl AbilityControl => CompOfType<AbilityComp_AbilityControl>();
        public IEnumerable<Verb> Verbs
        {
            get
            {
                yield return this.verb;
            }
        }

        public virtual bool Ready
        {
            get
            {
                if(!CanCast)
                {
                    //Cooling down.
                    return false;
                }

                if(!CanCastSub)
                {
                    //Comps denying cast
                    return false;
                }

                return true;
            }
        }

        public virtual bool CanAutoCast => AbilityControl?.AutoUse ?? false;

        public virtual bool CanCastSub
        {
            get
            {
                if (comps != null)
                {
                    foreach (AbilityComp_Base comp in BaseAbilityComps)
                    {
                        if (!comp.CanCast)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public virtual bool CastingOrSubCasting
        {
            get
            {
                if(Casting)
                {
                    return true;
                }

                if(this.verb != null)
                {
                    return this.verb.WarmingUp;
                }

                return false;
            }
        }

        public virtual bool CanShowGizmos => pawn.Faction != null && pawn.Faction.IsPlayer && !pawn.InMentalState;
        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (CooldownTicksRemaining > 0)
            {
                return false;
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

        public void InitializeComps()
        {
            if (def != null && !def.comps.NullOrEmpty())
            {
                comps = new List<AbilityComp>();
                for (int i = 0; i < def.comps.Count; i++)
                {
                    AbilityComp abilityComp = null;
                    try
                    {
                        abilityComp = (AbilityComp)Activator.CreateInstance(def.comps[i].compClass);
                        abilityComp.parent = this;
                        comps.Add(abilityComp);
                        abilityComp.Initialize(def.comps[i]);
                    }
                    catch (Exception arg)
                    {
                        Log.Error("Could not instantiate or initialize an AbilityComp: " + arg);
                        comps.Remove(abilityComp);
                    }
                }
            }
        }

        public override void ExposeData()
        {
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
            {
                //Calling it during PostLoadInit will recreate the comps and get rid of our saved comp data.
                base.ExposeData();
            }

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                //Need to have the comps, so create them for now.
                InitializeComps();
            }

            foreach (AbilityComp_Base comp in BaseAbilityComps)
            {
                //Save comp data.
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
        }

        public override string ToString()
        {
            return base.ToString() + " - " + this.def;
        }
    }
}
