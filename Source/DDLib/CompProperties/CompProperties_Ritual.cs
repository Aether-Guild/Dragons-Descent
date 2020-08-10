using RimWorld;
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
    public class RitualReference
    {
        public RitualDef def;

        public string iconPath;
        private Gizmo gizmo;

        public ThingDef moteConnecting, moteOnTarget, moteOnSource;
        public int moteConnectingWidth = 1, moteOnTargetScale = 1, moteOnSourceScale = 1;

        public SoundDef sound;
        public bool shakeCamera = false;

        public Command CreateCommand(Ritual ritual)
        {
            Command cmd = null;

            if (ritual is Ritual_Targeting)
            {
                cmd = new Command_Target();
            }
            else if (ritual is Ritual_AoE)
            {
                cmd = new Command_Action();
            }

            if (cmd != null)
            {
                if (!iconPath.NullOrEmpty())
                {
                    cmd.icon = ContentFinder<Texture2D>.Get(iconPath);
                }
            }

            return cmd;
        }

        public Command SetupAction(Thing parent, Command command, Ritual ritual, RitualTracker rituals)
        {
            if (command is Command_Target)
            {
                Command_Target cmd = command as Command_Target;
                cmd.targetingParams = new TargetingParameters()
                {
                    canTargetPawns = true,
                    canTargetBuildings = false,
                    validator = x => x.HasThing && x.Thing is Pawn pawn && pawn.def.HasModExtension<RitualTargetExtension>()
                };

                cmd.action = target =>
                {
                    Pawn pawn = target as Pawn;
                    (ritual as Ritual_Targeting).Target = pawn;

                    if (moteConnecting != null)
                    {
                        MakeLine(parent, pawn, moteConnecting, moteConnectingWidth);
                    }

                    if (moteOnTarget != null)
                    {
                        MakeEffect(pawn, moteOnTarget, moteOnTargetScale);
                    }
                    else
                    {
                        MakeDefaultEffect(pawn);
                    }

                    if (moteOnSource != null)
                    {
                        MakeEffect(parent, moteOnSource, moteOnSourceScale);
                    }

                    if (shakeCamera)
                    {
                        Find.CameraDriver.shaker.DoShake(1f);
                    }

                    if (sound != null)
                    {
                        sound.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                    }

                    rituals.Activate(def);
                };
            }

            if (command is Command_Action)
            {
                Command_Action cmd = command as Command_Action;

                cmd.action = () =>
                {
                    Ritual_AoE aoeRitual = ritual as Ritual_AoE;

                    foreach (Pawn pawn in aoeRitual.AllTargetedPawns)
                    {
                        if (moteConnecting != null)
                        {
                            MakeLine(parent, pawn, moteConnecting, moteConnectingWidth);
                        }

                        if (moteOnTarget != null)
                        {
                            MakeEffect(pawn, moteOnTarget, moteOnTargetScale);
                        }
                        else
                        {
                            MakeDefaultEffect(pawn);
                        }
                    }

                    if (moteOnSource != null)
                    {
                        MakeEffect(parent, moteOnSource, moteOnSourceScale);
                    }

                    if (shakeCamera)
                    {
                        Find.CameraDriver.shaker.DoShake(1f);
                    }

                    if (sound != null)
                    {
                        sound.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                    }

                    rituals.Activate(def);
                };
            }

            return command;
        }

        private static void MakeLine(Thing from, Thing to, ThingDef mote, int width)
        {
            MoteMaker.MakeConnectingLine(from.DrawPos, to.DrawPos, mote, from.Map, width);
        }

        private static void MakeDefaultEffect(Thing target)
        {
            MoteMaker.ThrowMetaPuff(target.DrawPos, target.Map);
        }

        private static void MakeEffect(Thing target, ThingDef mote, int scale)
        {
            MoteMaker.MakeAttachedOverlay(target, mote, target.DrawPos, scale);
        }

        public Gizmo GetGizmo(Thing parent)
        {
            Map map = parent.Map;
            RitualTracker rituals = map.GetComponent<MapComponent_Tracker>().Rituals;
            Ritual ritual = rituals[def];

            Command cmd = gizmo as Command;

            if (gizmo == null)
            {
                cmd = SetupAction(parent, CreateCommand(ritual), ritual, rituals);
                gizmo = cmd;
            }

            return UpdateState(cmd, ritual, rituals);
        }

        private Gizmo UpdateState(Command cmd, Ritual ritual, RitualTracker rituals)
        {
            string extraDescription = "";

            string reason = "";

            if (ritual.Active)
            {
                reason = "- Currently Active -\n" + "Expires in " + ritual.DurationRemaining.ToStringTicksToPeriodVague() + "\n";
            }
            else
            {
                if (rituals.CanActivate(def))
                {
                    extraDescription = "\n\n";

                    if ((ritual is Ritual_Targeting || ritual is Ritual_AoE) && !(ritual is ITickingRitual))
                    {
                        extraDescription += "Type: Instananeous Effect";
                    }
                    else if (ritual is ITickingRitual)
                    {
                        extraDescription += "Type: Effect over Time";
                    }

                    extraDescription += " ";

                    if (ritual is Ritual_Targeting)
                    {
                        extraDescription += "(Single Target)";
                    }
                    else if (ritual is Ritual_AoE)
                    {
                        extraDescription += "(Map-wide)";
                    }

                    extraDescription += "\n";

                    if (ritual is ITickingRitual)
                    {
                        extraDescription += "Duration: " + ritual.Duration.ToStringTicksToPeriod() + "\n";
                    }

                    extraDescription += "Cost: " + ritual.Cost + " favor.\n";
                    extraDescription += "Cooldown: " + ritual.Cooldown.ToStringTicksToPeriod();
                }
                else
                {
                    if (!ritual.CooledDown)
                    {
                        reason += "Cool down in " + ritual.CooldownRemaining.ToStringTicksToPeriodVague() + "\n";
                    }
                    if (rituals.Current < ritual.Cost)
                    {
                        reason += "Insufficient Favor: An additional " + (ritual.Cost - rituals.Current) + " favor required.\n";
                    }
                }
            }

            reason = reason.TrimEndNewlines();

            cmd.defaultLabel = def.label + " (" + ritual.Cost + ")";
            cmd.defaultDesc = def.description + extraDescription;

            cmd.disabled = !reason.NullOrEmpty();
            cmd.disabledReason = reason;

            return cmd;
        }
    }

    [StaticConstructorOnStartup]
    public class Gizmo_RitualInfo : Gizmo
    {
        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

        public RitualTracker rituals;

        public Gizmo_RitualInfo(Map map)
        {
            rituals = map.GetComponent<MapComponent_Tracker>().Rituals;
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override bool GroupsWith(Gizmo other)
        {
            return other is Gizmo_RitualInfo;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect insetRect = rect.ContractedBy(6f);

            Widgets.DrawWindowBackground(rect);

            if (rituals != null)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(insetRect, "Favor");

                Rect barRect = insetRect;
                barRect.yMin = insetRect.y + insetRect.height / 2f;

                float fillPercent = rituals.Current / rituals.Max;
                Widgets.FillableBar(barRect, fillPercent, FullBarTex, EmptyBarTex, doBorder: false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(barRect, (rituals.Current).ToString("F0") + " / " + (rituals.Max).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(insetRect, "Rituals\nUnsupported");
                Text.Anchor = TextAnchor.UpperLeft;
            }

            return new GizmoResult(GizmoState.Clear);
        }
    }

    public class CompProperties_Ritual : CompProperties
    {
        public List<RitualReference> rituals;

        public CompProperties_Ritual()
        {
            compClass = typeof(CompRitualAltar);
        }
    }
}
