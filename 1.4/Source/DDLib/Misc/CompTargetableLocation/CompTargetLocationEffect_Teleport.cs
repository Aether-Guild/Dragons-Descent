using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DD
{
    public class Tweener : IExposable
    {
        private static float Speed = GenTicks.SecondsToTicks(2);

        public Pawn pawn;
        public IntVec3 initial, destination;
        public int ticks = 0;

        public bool Finished => pawn.Position == destination;

        public Tweener(Pawn pawn, IntVec3 destination)
        {
            this.pawn = pawn;
            this.initial = pawn.Position;
            this.destination = destination;
        }

        public void Advance()
        {
            ticks++;

            IntVec3 current = pawn.Position;

            current.x = (int)Mathf.LerpUnclamped(initial.x, destination.x, (float)ticks / ((float)(destination.x - initial.x) / Speed));
            current.y = (int)Mathf.LerpUnclamped(initial.y, destination.y, (float)ticks / ((float)(destination.y - initial.y) / Speed));
            current.z = (int)Mathf.LerpUnclamped(initial.z, destination.z, (float)ticks / ((float)(destination.z - initial.z) / Speed));

            pawn.Position = current;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref ticks, "ticks", 0);
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Values.Look(ref destination, "destination");
        }
    }

    public class CompTargetLocationEffect_Teleport : CompTargetLocationEffect
    {
        public override void DoEffectOn(Pawn user, IntVec3 target)
        {
            user.Position = target;

        }
    }

    public class CompTargetLocationEffect_TeleportTweened : CompTargetLocationEffect
    {
        private List<Tweener> tweeners = new List<Tweener>();

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref tweeners, "tweeners", LookMode.Reference);
        }

        public override void CompTick()
        {
            base.CompTick();

            tweeners.RemoveAll(t => t.Finished);
            foreach (Tweener t in tweeners)
            {
                t.Advance();
            }
        }

        public override void DoEffectOn(Pawn user, IntVec3 target)
        {
            tweeners.Add(new Tweener(user, target));
            //user.Position = target;

        }
    }
}
