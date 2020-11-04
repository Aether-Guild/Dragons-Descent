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
        public override void ApplyRitual(Pawn target)
        {
            if (target.Faction == null)
            {
                //target.ClearMind();
                target.mindState.exitMapAfterTick = 0;
                //target.jobs.StopAll();

                Messages.Message("RitualBanishMessage".Translate(target.LabelCap.Named("TARGET")), target, MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
