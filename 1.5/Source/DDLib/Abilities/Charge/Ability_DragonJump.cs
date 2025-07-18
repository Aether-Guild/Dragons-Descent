using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace DD
{
    public class Ability_DragonJump : Ability
    {
        public override void Cast(LocalTargetInfo target)
        {
            //this.pawn.stances.SetStance(new DD_Stance_Stand(250, target, verb));
            base.Cast(target);
            LongEventHandler.QueueLongEvent(() =>
            {
                IntVec3 destination = target.Cell + ((this.pawn.Position - target.Cell).ToVector3().normalized * 2).ToIntVec3();
                Map map = this.pawn.Map;
                AbilityPawnFlyer flyer = (AbilityPawnFlyer)PawnFlyer.MakeFlyer(DD_ThingDefOf.DD_DragonJumpFlyer, this.pawn, destination, null, null);
                flyer.ability = this;
                flyer.DestinationCell = destination;
                GenSpawn.Spawn(flyer, target.Cell, map);

                target.Thing.TakeDamage(new DamageInfo(DamageDefOf.Blunt, this.GetPowerForPawn(), float.MaxValue, instigator: this.pawn));
                //LongEventHandler.QueueLongEvent(() =>
                //{
                //    GenExplosion.DoExplosion(target.Cell, this.pawn.MapHeld, 3f, DD_DamageDefOf.DraconicFlame, null, -1, -1f, null, null, null, null, null, 0f, 1);
                //}, "DragonJumpAbility", false, null);
            }, "DragonJumpAbility", false, null);
        }
    }






}







