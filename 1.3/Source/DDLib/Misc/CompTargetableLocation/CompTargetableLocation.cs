using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DD
{
    public class CompTargetableLocation : CompUseEffect
    {
        private TargetInfo target;

        public CompProperties_Targetable Props => (CompProperties_Targetable)props;

        protected virtual bool PlayerChoosesTarget => true;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_TargetInfo.Look(ref target, "target");
        }

        public override bool SelectedUseOption(Pawn p)
        {
            if (PlayerChoosesTarget)
            {
                Find.Targeter.BeginTargeting(GetTargetingParameters(), t =>
                {
                    target = new TargetInfo(t.Cell, p.Map);
                    parent.GetComp<CompUsable>().TryStartUseJob(p, target.Cell);
                }, p);
                return true;
            }
            target = null;
            return false;
        }

        public override void DoEffect(Pawn usedBy)
        {
            if ((!PlayerChoosesTarget || target != null) && (target == null || GetTargetingParameters().CanTarget(target)))
            {
                base.DoEffect(usedBy);
                foreach (CompTargetLocationEffect comp in parent.GetComps<CompTargetLocationEffect>())
                {
                    comp.DoEffectOn(usedBy, target.Cell);
                }
                if (Props.moteOnTarget != null)
                {
                    MoteMaker.MakeAttachedOverlay(usedBy, Props.moteOnTarget, Vector3.zero);
                }
                if (Props.moteConnecting != null)
                {
                    MoteMaker.MakeConnectingLine(usedBy.DrawPos, target.CenterVector3, Props.moteConnecting, usedBy.Map);
                }
                if (Props.fleckConnecting != null)
                {
                    FleckMaker.ConnectingLine(usedBy.DrawPos, target.CenterVector3, Props.fleckConnecting, usedBy.Map);
                }
                target = null;
            }
        }

        public bool BaseTargetValidator(Thing t)
        {
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                if (Props.psychicSensitiveTargetsOnly && pawn.GetStatValue(StatDefOf.PsychicSensitivity) <= 0f)
                {
                    return false;
                }
                if (Props.ignoreQuestLodgerPawns && pawn.IsQuestLodger())
                {
                    return false;
                }
                if (Props.ignorePlayerFactionPawns && pawn.Faction == Faction.OfPlayer)
                {
                    return false;
                }
            }
            if (Props.fleshCorpsesOnly)
            {
                Corpse corpse = t as Corpse;
                if (corpse != null && !corpse.InnerPawn.RaceProps.IsFlesh)
                {
                    return false;
                }
            }
            if (Props.nonDessicatedCorpsesOnly)
            {
                Corpse corpse2 = t as Corpse;
                if (corpse2 != null && corpse2.GetRotStage() == RotStage.Dessicated)
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetSelf = false,
                canTargetHumans = false,
                canTargetPawns = false,
                canTargetAnimals = false,
                canTargetLocations = true,
                validator = (x => BaseTargetValidator(x.Thing))
            };
        }

    }
}
