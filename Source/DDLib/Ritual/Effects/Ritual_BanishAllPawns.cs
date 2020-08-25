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
    public class Ritual_BanishAllPawns : Ritual_AoE
    {
        public override void ApplyRitual(Pawn target)
        {
            if (target.Faction == null)
            {
                //target.ClearMind();
                target.mindState.exitMapAfterTick = 0;
                //target.jobs.StopAll();
            }
        }

        protected override void PreActivation()
        {
            IEnumerable<Pawn> BanishedPawns = base.AllTargetedPawns.Where(target => target.Faction == null);

            if (BanishedPawns.Any())
            {
                Messages.Message("RitualBanishAllMessage".Translate(BanishedPawns.Count().Named("COUNT")), new LookTargets(BanishedPawns), MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
