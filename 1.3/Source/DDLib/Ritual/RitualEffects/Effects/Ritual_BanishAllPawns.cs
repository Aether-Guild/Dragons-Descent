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
    public class Ritual_BanishAllPawns : Ritual_MapTargeting
    {
        private IEnumerable<Pawn> AffectedPawns => AllTargets.Where(target => target.HasThing && !target.ThingDestroyed).Select(target => target.Thing).OfType<Pawn>();

        public override void ApplyRitual(TargetInfo target)
        {
            if (target.HasThing && !target.ThingDestroyed && target.Thing is Pawn pawn)
            {
                if (pawn.Faction == null)
                {
                    pawn.mindState.exitMapAfterTick = 0;
                }
            }
        }

        protected override void PreActivation()
        {
            IEnumerable<Pawn> BanishedPawns = AffectedPawns.Where(target => target.Faction == null);

            if (BanishedPawns.Any())
            {
                Messages.Message("RitualBanishAllMessage".Translate(BanishedPawns.Count().Named("COUNT")), new LookTargets(BanishedPawns), MessageTypeDefOf.NeutralEvent);
            }
        }
    }
}
