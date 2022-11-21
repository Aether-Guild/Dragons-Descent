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
    public enum RitualTarget
    {
        NoTarget, LocalTarget, GlobalTarget, ForeignTarget
    }

    public class RitualDef : Def
    {
        public Type ritualClass;

        public SimpleCurve cost;
        public SimpleCurve cooldown;

        public FloatRange initialCooldown;

        public IntRange targetedGoodwillChange = IntRange.zero, otherGoodwillChange = IntRange.zero;
        public int maxGoodwillActivations = 10;

        public RitualTargetingParameters targetingParams = new RitualTargetingParameters();

        public ThingDef moteConnecting, moteOnTarget, moteOnSource, moteOnTick;
        public FleckDef fleckConnecting, fleckOnTarget, fleckOnSource, fleckOnTick;
        public float moteConnectingWidth = 1f, moteOnTargetScale = 1f, moteOnSourceScale = 1f, moteOnTickScale = 1f;

        public SoundDef sound;
        public bool shakeCamera = false;

        public string iconPath, targetingOverlayPath;

        public string confirmationMessage;

        [Unsaved(false)]
        private Texture2D gizmoIcon;
        [Unsaved(false)]
        private Texture2D overlayIcon;

        public int InitialCooldown => GenTicks.SecondsToTicks(initialCooldown.RandomInRange);
        public bool WillChangeGoodwill => targetedGoodwillChange.Average != 0 || otherGoodwillChange.Average != 0;
        public int CalculateTargetedGoodwillChange(int activations) => Mathf.RoundToInt(Mathf.Lerp(targetedGoodwillChange.min, targetedGoodwillChange.max, Mathf.Clamp01((float)activations / (float)maxGoodwillActivations)));
        public int CalculateOtherGoodwillChange(int activations) => Mathf.RoundToInt(Mathf.Lerp(otherGoodwillChange.min, otherGoodwillChange.max, Mathf.Clamp01((float)activations / (float)maxGoodwillActivations)));

        public bool HasConfirmationMessage => WillChangeGoodwill || !confirmationMessage.NullOrEmpty();
        public string GetConfirmationMessage(Ritual ritual)
        {
            string message = "";

            if (!confirmationMessage.NullOrEmpty())
            {
                message += confirmationMessage + "\n";
            }

            if (WillChangeGoodwill)
            {
                if (targetedGoodwillChange.Average != 0)
                {
                    message += "RitualTargetedGoodwillChangeMessage".Translate(ritual.TargetedFaction.Name.Named("FACTION"), CalculateTargetedGoodwillChange(ritual.ActivationCount).Named("GOODWILL")) + "\n";
                }
                if (otherGoodwillChange.Average != 0)
                {
                    message += "RitualOtherGoodwillChangeMessage".Translate(CalculateOtherGoodwillChange(ritual.ActivationCount).Named("GOODWILL"));
                }
            }

            return message;
        }

        public Texture2D GizmoIcon
        {
            get
            {
                if (gizmoIcon == null)
                {
                    gizmoIcon = !iconPath.NullOrEmpty() ? ContentFinder<Texture2D>.Get(iconPath) : BaseContent.BadTex;
                }
                return gizmoIcon;
            }
        }
        public Texture2D TargetingOverlay
        {
            get
            {
                if (overlayIcon == null && !targetingOverlayPath.NullOrEmpty())
                {
                    overlayIcon = ContentFinder<Texture2D>.Get(targetingOverlayPath);
                }
                return overlayIcon;
            }
        }
    }
}
