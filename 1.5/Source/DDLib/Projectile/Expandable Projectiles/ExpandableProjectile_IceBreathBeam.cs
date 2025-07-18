using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;
using VFECore;


namespace DD.Projectiles
{

    public class ExpandableProjectile_DragonBeam_Frost : ExpandableProjectile
    {
        public override void DoDamage(IntVec3 pos)
        {
            base.DoDamage(pos);
            try
            {
                if (pos != this.launcher.Position && this.launcher.Map != null && GenGrid.InBounds(pos, this.launcher.Map))
                {
                    Map.snowGrid.AddDepth(pos, 0.8f);
                    var list = this.launcher.Map.thingGrid.ThingsListAt(pos);
                    for (int num = list.Count - 1; num >= 0; num--)
                    {
                      //  Log.Message("Damaging " + list[num]);
                        if (IsDamagable(list[num]))
                        {
                            this.customImpact = true;
                            base.Impact(list[num]);
                            this.customImpact = false;
                            if (list[num] is Pawn pawn)
                            {
                                if (Rand.Chance(0.199f))
                                //HealthUtility.AdjustSeverity(pawn, HediffDefOf.Burn, 0.1f);
                                HealthUtility.AdjustSeverity(pawn, HediffDef.Named("HypothermicSlowdown"), 0.1f);
                            }
                        }
                      //  if (Rand.Chance(0.199f))
                        { 

                            
                            // FleckMaker.ThrowSmoke(Position.ToVector3Shifted() + Gen.RandomHorizontalVector(0.5f), Map, 2f);
                            Vector3 vector = new Vector3(Position.x, 2f, Position.y);
                            vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                            //FleckMaker.ThrowAirPuffUp(launcher.Position, base.Map);
                            //FleckMaker.ThrowTornadoDustPuff(vector + Vector3Utility.RandomHorizontalOffset(1.5f), base.Map, Rand.Range(90.5f, 100f), new Color(num, num, num));
                            //FleckMaker.WaterSplash(ExactPosition, base.Map, Mathf.Sqrt(DamageAmount) * 10f, 20f);

                        }
                    }
                }
            }
            catch { };
        }

        public override bool IsDamagable(Thing t)
        {
            return t is Pawn && base.IsDamagable(t) || t.def == ThingDefOf.Fire;
        }
    }
}



