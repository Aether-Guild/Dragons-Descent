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
    public abstract class Ritual : IExposable, IRitual
    {
        private Map map;
        private RitualDef def;

        private int activationCount = 0;
        private TimeKeeper cooldown = new TimeKeeper();

        public int ActivationCount => activationCount;
        public bool CooledDown => cooldown.Expired;
        public int CooldownRemaining => cooldown.Remaining;
        public float CooldownPercent => cooldown.RemainingPercent;

        public string Label => def.label;
        public string LabelCap => def.LabelCap;
        public RitualDef Def => def;

        public virtual void Setup(Map map, RitualDef def)
        {
            this.map = map;
            this.def = def;
            cooldown.Update(def.InitialCooldown);
        }

        public virtual float Cost => Mathf.Round(Def.cost.Evaluate(activationCount));
        public virtual int Cooldown => GenTicks.SecondsToTicks(Def.cooldown.Evaluate(activationCount));

        public abstract void DoActivation();
        public abstract void DoDeactivation();

        protected virtual void PreActivation() { }
        protected virtual void PreDeactivation() { }

        protected virtual void PostActivation() { }
        protected virtual void PostDeactivation() { }

        public virtual bool IsReady => ActivatingFaction != null;

        public Faction ActivatingFaction { get; set; }
        public abstract Faction TargetedFaction { get; }

        public void Activate()
        {
            PreActivation();
            DoActivation();
            DoGoodwillChanges();
            IncrementCount();
            PostActivation();
        }

        public void Deactivate()
        {
            PreDeactivation();
            DoDeactivation();
            cooldown.Update(Cooldown);
            PostDeactivation();
        }

        public void IncrementCount()
        {
            activationCount++;
        }

        private void DoGoodwillChanges()
        {
            if (TargetedFaction != null)
            {
                //Faction was targeted and not self-targeting.
                if (ActivatingFaction != TargetedFaction)
                {
                    TargetedFaction.TryAffectGoodwillWith(ActivatingFaction, Def.CalculateTargetedGoodwillChange(ActivationCount));
                }

                foreach (Faction faction in Find.FactionManager.AllFactionsListForReading.Where(f => !f.defeated && f != TargetedFaction))
                {
                    faction.TryAffectGoodwillWith(ActivatingFaction, Def.CalculateOtherGoodwillChange(ActivationCount));
                }
            }
        }

        private void FinishGoodwillChanges()
        {
            ActivatingFaction = null;
        }

        public virtual bool CanActivate(float level) => CooledDown && level >= Cost && ActivatingFaction != null;

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Deep.Look(ref cooldown, "cd", 0);
            Scribe_Values.Look(ref activationCount, "count", 0);
        }

        public virtual void Reset(bool resetActivationCount = false)
        {
            cooldown.Clear();
            ActivatingFaction = null;
            if (resetActivationCount)
            {
                activationCount = 0;
            }
        }
    }
}
