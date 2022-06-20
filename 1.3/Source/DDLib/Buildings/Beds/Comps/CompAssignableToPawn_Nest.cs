using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class CompAssignableToPawn_Nest : CompAssignableToPawn_Bed
    {
        protected CompProperties_AssignableToPawn_Body BProps => props as CompProperties_AssignableToPawn_Body;

        public override AcceptanceReport CanAssignTo(Pawn pawn)
        {
            if (pawn.Faction == null || !pawn.Faction.IsPlayer)
            {
                return AcceptanceReport.WasRejected;
            }

            if (pawn.RaceProps.body != BProps.bodyDef)
            {
                return AcceptanceReport.WasRejected;
            }

            return base.CanAssignTo(pawn);
        }

        public override IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!parent.Spawned)
                {
                    return Enumerable.Empty<Pawn>();
                }

                return parent.Map.mapPawns.AllPawnsSpawned.Where(pawn => CanAssignTo(pawn).Accepted);
            }
        }


        protected override bool ShouldShowAssignmentGizmo()
        {
            return parent.Faction == Faction.OfPlayer ? true : base.ShouldShowAssignmentGizmo();
        }
    }
}
