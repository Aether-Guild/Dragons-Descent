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
    public class FireOverlay_Entry
    {
        public Direction8Way rotation;
        public List<Vector3> offsets;
    }

    public class CompProperties_FireOverlay_RotAware : CompProperties_FireOverlay
    {
        public List<Direction8Way> rotation = new List<Direction8Way>() {
            Direction8Way.North,
            Direction8Way.East,
            Direction8Way.West,
            Direction8Way.South
        };

        public CompProperties_FireOverlay_RotAware()
        {
            compClass = typeof(CompFireOverlay_RotAware);
        }
    }

    public class CompFireOverlay_RotAware : CompFireOverlay
    {
        private static Dictionary<Direction8Way, Rot4> rots = new Dictionary<Direction8Way, Rot4>()
        {
            [Direction8Way.North] = Rot4.North,
            [Direction8Way.East] = Rot4.East,
            [Direction8Way.West] = Rot4.West,
            [Direction8Way.South] = Rot4.South,
        };
        public static Rot4 LookupRot4(Direction8Way dir) => rots.Where(re => dir == re.Key).Select(e => e.Value).FirstOrFallback();
        public static Direction8Way LookupDirection(Rot4 rot) => rots.Where(re => rot == re.Value).Select(e => e.Key).FirstOrFallback();

        public CompProperties_FireOverlay_RotAware RAProps => props as CompProperties_FireOverlay_RotAware;

        public override void PostDraw()
        {
            if (RAProps.rotation.Select(dir => LookupRot4(dir)).Where(rot => rot != null && rot == parent.Rotation).Any())
            {
                base.PostDraw();
            }
        }
    }

    public class CompProperties_FireOverlay_Multiple : CompProperties_FireOverlay
    {
        public List<FireOverlay_Entry> rotationOffsets = new List<FireOverlay_Entry>();

        public CompProperties_FireOverlay_Multiple()
        {
            compClass = typeof(CompFireOverlay_Multiple);
        }
    }

    public class CompFireOverlay_Multiple : ThingComp
    {
        private bool active = false;

        public bool Active { get => active; set => active = value; }
        public CompProperties_FireOverlay_Multiple Props => props as CompProperties_FireOverlay_Multiple;

        public CompFireOverlay_Multiple()
        {
        }

        public override void ReceiveCompSignal(string signal)
        {
            if(signal == CompFlickable.FlickedOnSignal)
            {
                active = true;
            }

            if(signal == CompFlickable.FlickedOffSignal)
            {
                active = false;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            IEnumerable<FireOverlay_Entry> rots = Props.rotationOffsets.Where(e => CompFireOverlay_RotAware.LookupRot4(e.rotation) == parent.Rotation);

            if (rots.Any())
            {
                Draw(rots.First().offsets);
            }
        }

        private void Draw(List<Vector3> offsets)
        {
            if (active)
            {
                Vector3 drawPos = parent.DrawPos;
                drawPos.y += Altitudes.AltInc;
                foreach (Vector3 offset in offsets)
                {
                    CompFireOverlay.FireGraphic.Draw(drawPos + offset, parent.Rotation, parent);
                }
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref active, "active", false);
        }
    }
}
