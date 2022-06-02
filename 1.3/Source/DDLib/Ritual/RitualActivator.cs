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
    public class RitualActivator
    {
        private RitualTracker tracker;
        private Ritual ritual;
        private Thing source;

        public RitualDef Def => ritual.Def;
        public Faction Faction => source.Faction;

        public TargetInfo Target { get; set; }
        public Map TargetMap { get; set; }
        public WorldObject TargetObject { get; set; }

        public RitualActivator(RitualTracker tracker, Ritual ritual)
        {
            this.tracker = tracker;
            this.ritual = ritual;
        }

        public virtual void Initialize(Thing source)
        {
            Target = TargetInfo.Invalid;
            TargetMap = source.Map;
            TargetObject = source.Map.Parent;
            this.source = source;
        }

        public virtual void Prepare()
        {
            if (Faction == null)
            {
                Messages.Message("RitualNoFactionMessage".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }

            ritual.ActivatingFaction = Faction;

            if (ritual is ISingleTargetRitual sr)
            {
                sr.Target = Target;
            }

            if (ritual is IMultipleTargetRitual mr)
            {
                mr.TargetMap = TargetMap;
            }

            if (ritual is IWorldTargetRitual wr)
            {
                wr.WorldTarget = TargetObject;
            }
        }

        public virtual bool Activate()
        {
            if (ritual.IsReady)
            {
                if (Def.HasConfirmationMessage)
                {
                    Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation(Def.GetConfirmationMessage(ritual), DoActivate, title: "RitualConfirmationTitle".Translate());
                    Find.WindowStack.Add(window);
                }
                else
                {
                    DoActivate();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void DoActivate()
        {
            if (ritual.Def.moteOnSource != null)
            {
                MoteMaker.MakeAttachedOverlay(source, ritual.Def.moteOnSource, Vector3.zero, ritual.Def.moteOnSourceScale);
            }
            if (ritual.Def.fleckOnSource != null)
            {
                FleckMaker.AttachedOverlay(source, ritual.Def.fleckOnSource, Vector3.zero, ritual.Def.moteOnSourceScale);
            }

            if (ritual is ISingleTargetRitual str)
            {
                if (str.Target.IsValid)
                {
                    if (ritual.Def.moteConnecting != null)
                    {
                        MoteMaker.MakeConnectingLine(source.DrawPos, str.Target.CenterVector3, ritual.Def.moteConnecting, str.Target.Map, ritual.Def.moteConnectingWidth);
                    }
                    if (ritual.Def.fleckConnecting != null)
                    {
                        FleckMaker.ConnectingLine(source.DrawPos, str.Target.CenterVector3, ritual.Def.fleckConnecting, str.Target.Map, ritual.Def.moteConnectingWidth);
                    }
                    if (ritual.Def.moteOnTarget != null)
                    {
                        if (str.Target.HasThing)
                        {
                            MoteMaker.MakeAttachedOverlay(str.Target.Thing, ritual.Def.moteOnTarget, Vector3.zero, ritual.Def.moteOnTargetScale);
                        }
                    }
                    else if (ritual.Def.fleckOnTarget != null)
                    {
                        if (str.Target.HasThing)
                        {
                            FleckMaker.AttachedOverlay(str.Target.Thing, ritual.Def.fleckOnTarget, Vector3.zero, ritual.Def.moteOnTargetScale);
                        }
                    }
                    else
                    {
                        FleckMaker.ThrowMetaPuff(str.Target.CenterVector3, str.Target.Map);
                    }
                }
            }

            if (ritual is IMultipleTargetRitual mtr)
            {
                foreach (TargetInfo t in mtr.AllTargets)
                {
                    if (ritual.Def.moteConnecting != null)
                    {
                        MoteMaker.MakeConnectingLine(source.DrawPos, t.CenterVector3, ritual.Def.moteConnecting, t.Map, ritual.Def.moteConnectingWidth);
                    }

                    if (ritual.Def.moteOnTarget != null)
                    {
                        MoteMaker.MakeAttachedOverlay(source, ritual.Def.moteOnTarget, Vector3.zero, ritual.Def.moteOnTargetScale);
                    }
                    else if (ritual.Def.fleckOnTarget != null)
                    {
                        FleckMaker.AttachedOverlay(source, ritual.Def.fleckOnTarget, Vector3.zero, ritual.Def.moteOnTargetScale);
                    }
                    else
                    {
                        FleckMaker.ThrowMetaPuff(t.CenterVector3, t.Map);
                    }
                }
            }

            if (ritual.Def.shakeCamera)
            {
                Find.CameraDriver.shaker.DoShake(1f);
            }

            if (ritual.Def.sound != null)
            {
                ritual.Def.sound.PlayOneShot(new TargetInfo(source.Position, source.Map));
            }

            tracker.Activate(Def);
        }
    }
}
