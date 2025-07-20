using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DD
{
    [StaticConstructorOnStartup]
    public class Command_RitualEffect : Command
    {
        private static readonly Texture2D FullCooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0.5f, 0.5f));
        private static readonly Texture2D FullTickingBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 1f, 0.5f));
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        private Thing source;
        private Ritual ritual;
        private RitualTracker tracker;
        private RitualActivator ritualRequest;

        protected virtual RitualActivator CreateSetup => new RitualActivator(tracker, ritual);

        private readonly string baseLabel, baseDescription;

        public Command_RitualEffect(Thing source, RitualTracker tracker, RitualDef def)
        {
            this.source = source;
            this.tracker = tracker;
            this.ritual = tracker[def];

            this.icon = def.GizmoIcon;

            this.baseLabel = def.label;

            this.baseDescription = def.description + "\n\n" + "RitualLabelType".Translate((ritual is ITickingRitual ? "RitualTypeTicking" : "RitualTypeInstant").Translate().Named("TYPE")) + " ";
            switch (def.targetingParams.ritualTarget)
            {
                case RitualTarget.NoTarget:
                    this.baseDescription += "RitualTargetMap".Translate();
                    break;
                case RitualTarget.LocalTarget:
                    this.baseDescription += "RitualTargetLocal".Translate();
                    break;
                case RitualTarget.GlobalTarget:
                    this.baseDescription += "RitualTargetWorld".Translate();
                    break;
                case RitualTarget.ForeignTarget:
                    this.baseDescription += "RitualTargetForeign".Translate();
                    break;
            }
        }

        protected override GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
        {
            RitualDef def = ritual.Def;

            bool showBar = false;
            Rect barRect = butRect.RightPart(0.25f);
            Texture2D barTexture = null;
            float percent = 0f;
            string barTip = null;

            bool disabled = false;
            string reason = "", extraDescription = "";

            ITickingRitual tickingRitual = ritual as ITickingRitual;

            if (tickingRitual != null)
            {
                if (tickingRitual.Active)
                {
                    disabled = true;
                    reason += "RitualActive".Translate() + "\n";

                    percent = tickingRitual.DurationPercent;
                    barTexture = FullTickingBarTex;
                    barTip = "RitualDurationExpiry".Translate(tickingRitual.DurationRemaining.ToStringTicksToPeriod().Named("TIME"));
                    showBar = true;
                }
                extraDescription += "RitualLabelDuration".Translate(tickingRitual.Duration.ToStringTicksToPeriod().Named("TIME")) + "\n";
            }

            extraDescription += "RitualLabelCost".Translate(ritual.Cost.Named("COST")) + "\n";
            extraDescription += "RitualLabelCooldown".Translate(ritual.Cooldown.ToStringTicksToPeriod().Named("TIME"));

            if (tracker.Current < ritual.Cost)
            {
                disabled = true;
                reason += "RitualInsufficientFavor".Translate((ritual.Cost - tracker.Current).Named("NEEDED")) + "\n";
            }

            if (!ritual.CooledDown)
            {
                disabled = true;
                reason += "RitualCoolingDown".Translate() + "\n";

                percent = ritual.CooldownPercent;
                barTexture = FullCooldownBarTex;
                barTip = "RitualCooldownExpiry".Translate(ritual.CooldownRemaining.ToStringTicksToPeriod().Named("TIME"));
                showBar = true;
            }

            reason = reason.TrimEndNewlines();

            this.defaultLabel = "RitualLabel".Translate(baseLabel.Named("LABEL"), ritual.Cost.Named("COST"));
            this.defaultDesc = baseDescription + "\n" + extraDescription;

            this.disabled = disabled;
            this.disabledReason = reason;

            if (showBar)
            {
                Widgets.DrawWindowBackground(barRect);
                if (Mouse.IsOver(barRect) && DoTooltip && !barTip.NullOrEmpty())
                {
                    TipSignal tip = barTip;
                    TooltipHandler.TipRegion(barRect, tip);
                }

                butRect = butRect.LeftPartPixels(butRect.width - barRect.width);
                barRect = barRect.ContractedBy(3f);

                GUI.DrawTexture(barRect, EmptyBarTex);
                GUI.DrawTexture(barRect.BottomPart(percent), barTexture);

                parms.shrunk = true;
            }

            return base.GizmoOnGUIInt(butRect, parms);
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();

            ritualRequest = CreateSetup;
            ritualRequest.Initialize(source);

            switch (ritual.Def.targetingParams.ritualTarget)
            {
                case RitualTarget.NoTarget:
                    ActivateOnNoTarget();
                    break;
                case RitualTarget.LocalTarget:
                    Find.Targeter.BeginTargeting(ritual.Def.targetingParams, ActivateOnLocalTarget, mouseAttachment: ritual.Def.TargetingOverlay);
                    break;
                case RitualTarget.GlobalTarget:
                    CameraJumper.TryJump(CameraJumper.GetWorldTarget(source));
                    Find.WorldTargeter.BeginTargeting(ActivateOnGlobalTarget, true, extraLabelGetter: GetGlobalLabel, closeWorldTabWhenFinished: true, canSelectTarget: target => ritual.Def.targetingParams.CanTargetGlobal(source, target));
                    break;
                case RitualTarget.ForeignTarget:
                    CameraJumper.TryJump(CameraJumper.GetWorldTarget(source));
                    Find.WorldTargeter.BeginTargeting(ActivateOnForeignTarget, true, extraLabelGetter: GetGlobalLabel, canSelectTarget: target => ritual.Def.targetingParams.CanTargetGlobal(source, target));
                    break;
            }
        }

        private string GetGlobalLabel(GlobalTargetInfo target)
        {
            string text = ritual.Def.LabelCap;

            if (target.IsValid)
            {
                if (ritual.Def.targetingParams.InRange(source, target))
                {
                    if (target.Map != null)
                    {
                        text += "\n" + "RitualGlobalLabelTargetCount".Translate(target.Map.mapPawns.AllPawnsSpawned.Count(pawn => ritual.Def.targetingParams.CanTarget(pawn)).Named("COUNT"));
                    }
                }
                else
                {
                    if (target.Tile >= 0 && ritual.Def.targetingParams.tileRange.HasValue)
                    {
                        string range = ritual.Def.targetingParams.tileRange.Value.ToString();
                        if (ritual.Def.targetingParams.tileRange.Value.Span == 0)
                        {
                            range = "0~" + ritual.Def.targetingParams.tileRange.Value.TrueMax;
                        }

                        text += "\n~" + "RitualGlobalLabelTargetRange".Translate(Mathf.Round(Find.WorldGrid.ApproxDistanceInTiles(source.Tile, target.Tile)).Named("DISTANCE"), range.Named("RANGE"));
                    }
                }
            }

            return text;
        }

        protected virtual void ActivateOnNoTarget()
        {
            TryActivate(ritual);
        }

        protected virtual void ActivateOnLocalTarget(LocalTargetInfo target)
        {
            if (!target.IsValid)
            {
                return;
            }

            ritualRequest.Target = target.ToTargetInfo(Find.CurrentMap);

            TryActivate(ritual);
        }

        protected virtual bool ActivateOnGlobalTarget(GlobalTargetInfo target)
        {
            if (!target.IsValid)
            {
                return false;
            }

            if (!ritual.Def.targetingParams.CanTargetGlobal(source, target))
            {
                return false;
            }

            if (ritual is ISingleTargetRitual sr)
            {
                if (target.HasThing && !target.ThingDestroyed)
                {
                    ritualRequest.Target = target.Thing;
                }
            }

            if (ritual is IWorldTargetRitual wr)
            {
                if (target.IsWorldTarget || target.HasWorldObject)
                {
                    ritualRequest.TargetObject = target.WorldObject;
                }
            }

            if (ritual is IMultipleTargetRitual mr)
            {
                if (target.Map != null)
                {
                    ritualRequest.TargetMap = target.Map;
                }
                else if (target.IsWorldTarget || target.HasWorldObject)
                {
                    if (target.WorldObject is MapParent mp)
                    {
                        ritualRequest.TargetMap = mp.Map;
                    }
                }
            }

            return TryActivate(ritual);
        }

        protected virtual bool ActivateOnForeignTarget(GlobalTargetInfo target)
        {
            if (!target.IsValid)
            {
                return false;
            }

            if (!ritual.Def.targetingParams.InRange(source, target))
            {
                return false;
            }

            if (target.WorldObject is MapParent mp && mp.HasMap)
            {
                CameraJumper.TryJump(mp.Map.Center, mp.Map);
                Find.Targeter.BeginTargeting(ritual.Def.targetingParams, ActivateOnLocalTarget, mouseAttachment: ritual.Def.TargetingOverlay);

                return true;
            }

            return false;
        }

        protected virtual bool TryActivate(Ritual ritual)
        {
            ritualRequest.Prepare();

            if (!ritual.IsReady)
            {
                Messages.Message("RitualNoTargetMessage".Translate(), MessageTypeDefOf.CautionInput);
                return false;
            }

            return ritualRequest.Activate();
        }

        //public override bool GroupsWith(Gizmo other)
        //{
        //    if (other is Command_RitualEffect cmd)
        //    {
        //        if (groupKey == cmd.groupKey && groupKey != 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    else
        //    {
        //        return base.GroupsWith(other);
        //    }
        //}

        public override bool InheritInteractionsFrom(Gizmo other)
        {
            return false;
        }
    }
}
