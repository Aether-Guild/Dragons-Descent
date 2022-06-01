using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DD
{
    public class AbilityComp_Flight : AbilityComp_Base
    {
        public AbilityCompProperties_Flight VProps => props as AbilityCompProperties_Flight;
        public override bool CanCast => !parent.pawn.IsFormingCaravan();

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return WingedFlyer.IsCellValid(parent.pawn.Position, parent.pawn.Map, true) && WingedFlyer.IsCellValid(target.Cell, parent.pawn.Map, true) && parent.pawn.Position.DistanceTo(target.Cell) <= VProps.range;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            WingedFlyer.MakeFlyer(parent.pawn, target.ToTargetInfo(parent.pawn.Map));
        }
    }
}
