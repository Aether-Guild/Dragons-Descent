using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Verse;
using Verse.Sound;
using VFECore;


namespace DD
{
    public class VEF_DD_FireBreathProjectileTEST2 : ExpandableProjectile
    {
        public override void DoDamage(IntVec3 pos)
        {
            base.DoDamage(pos);
            try
            {
                if (pos != this.launcher.Position && this.launcher.Map != null && GenGrid.InBounds(pos, this.launcher.Map))
                    if (Rand.Chance(0.005f))
                    {
                        FilthMaker.TryMakeFilth(pos, launcher.Map, ThingDefOf.Filth_Dirt, 1, FilthSourceFlags.None);
                        FleckMaker.ThrowSmoke(GenThing.TrueCenter(this) + new Vector3(0f, 0f, 0.1f), Map, 1f);
                        FleckMaker.ThrowMicroSparks(Position.ToVector3(), launcher.Map);

                    }


                //FleckMaker.ThrowFireGlow(PositionHeld.ToVector3(), Map, .3f);
               
                //FleckMaker.ThrowLightningGlow(Position.ToVector3(), launcher.Map, 1f);
                {
                    var list = this.launcher.Map.thingGrid.ThingsListAt(pos);
                    for (int num = list.Count - 1; num >= 0; num--)
                    {
                        if (IsDamagable(list[num]))
                        {
                            if (!list.Where(x => x.def == ThingDefOf.Fire).Any())
                                if (Rand.Chance(0.005f))
                                {
                                CompAttachBase compAttachBase = list[num].TryGetComp<CompAttachBase>();
                                Fire obj = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire);
                                obj.fireSize = .1f;
                                GenSpawn.Spawn(obj, list[num].Position, list[num].Map, Rot4.North);
                                if (compAttachBase != null)
                                {
                                    obj.AttachTo(list[num]);
                                    Pawn pawn = list[num] as Pawn;
                                    if (pawn != null)
                                    {
                                        pawn.jobs.StopAll();
                                        pawn.records.Increment(RecordDefOf.TimesOnFire);
                                    }
                                }
                            }
                            this.customImpact = true;
                            base.Impact(list[num]);
                            this.customImpact = false;
                        }
                    }
                }
            }
            catch { };
        }

        public override bool IsDamagable(Thing t)
        {
            return base.IsDamagable(t) && t.def != ThingDefOf.Fire;
        }
    }
    //public class DamageWorker_ExtraDamageMechanoids : DamageWorker_Cut
    //{


    //    protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
    //    {
    //        base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
    //        if (pawn.RaceProps.FleshType == FleshTypeDefOf.Mechanoid)
    //        {
    //            pawn.TakeDamage(new DamageInfo(DamageDefOf.EMP, 30, 0f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown));

    //        }

    //    }
    //}
}


