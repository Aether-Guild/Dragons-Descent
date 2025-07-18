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
    public class Ritual_BanishPawn : Ritual_Targeting
    {
        public override void ApplyRitual(TargetInfo target)
        {
            if (target.HasThing && !target.ThingDestroyed && target.Thing is Pawn pawn)
            {
                if (pawn.Faction == null)
                {
                    pawn.mindState.exitMapAfterTick = 0;

                    Messages.Message("RitualBanishMessage".Translate(pawn.LabelCap.Named("TARGET")), target, MessageTypeDefOf.NeutralEvent);
                }
            }
        }
    }
}
