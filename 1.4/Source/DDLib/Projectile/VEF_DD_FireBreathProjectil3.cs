using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Verse;
using Verse.Sound;
using VFECore;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;


namespace DD
{
    //public class Ability_Charge : Ability
    //{
    //    public override void Cast(LocalTargetInfo target)
    //    {
    //        base.Cast(target);

    //        LongEventHandler.QueueLongEvent(() =>
    //        {
    //            IntVec3 destination = target.Cell + ((this.pawn.Position - target.Cell).ToVector3().normalized * 2).ToIntVec3();
    //            Map map = this.pawn.Map;

    //            AbilityPawnFlyer flyer = (AbilityPawnFlyer)PawnFlyer.MakeFlyer(DD_InternalDefOf.DD_StraightFlyer, this.pawn, destination);
    //            flyer.ability = this;
    //            flyer.target = destination.ToVector3();
    //            GenSpawn.Spawn(flyer, target.Cell, map);
    //            target.Thing.TakeDamage(new DamageInfo(DamageDefOf.Cut, this.GetPowerForPawn(), float.MaxValue, instigator: this.pawn));

    //        }, "chargeAbility", false, null);
    //    }
    //}
}