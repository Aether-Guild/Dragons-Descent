using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using RimWorld.Planet;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace DD;

public class CompAbilityEffect_Burner_DragonNecro : CompAbilityEffect
{

    public new CompProperties_AbilityBurner_DragonNecro Props => (CompProperties_AbilityBurner_DragonNecro)props;

    public override IEnumerable<PreCastAction> GetPreCastActions()
    {
    if (ModsConfig.AnomalyActive)
    { 

        
        yield return new PreCastAction
        {
            action = delegate (LocalTargetInfo a, LocalTargetInfo _)
            {
                Vector3 drawPos = parent.pawn.DrawPos;
                IntVec3 intVec = drawPos.Yto0().ToIntVec3();
                Map map = parent.pawn.Map;
                IncineratorSpray incineratorSpray = GenSpawn.Spawn(DD_ThingDefOf.DD_IncineratorSpray_Dragon, intVec, map) as IncineratorSpray;
                int numStreams = Props.numStreams;
                Vector3 normalized = (a.CenterVector3 - drawPos).normalized;
                for (int i = 0; i < numStreams; i++)
                {
                    float angle = Rand.Range(0f - Props.coneSizeDegrees, Props.coneSizeDegrees);
                    Vector3 vector = normalized.RotatedBy(angle);
                    Vector3 vect = drawPos + vector * (Props.range + Rand.Value * Props.rangeNoise);
                    IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(intVec, vect.ToIntVec3(), (IntVec3 c) => c.CanBeSeenOverFast(map), skipFirstCell: true);
                    if (!intVec2.IsValid)
                    {
                        intVec2 = vect.ToIntVec3();
                    }
                    float num = Vector3.Distance(intVec2.ToVector3(), drawPos);
                    float num2 = Mathf.Clamp01(num / Props.sizeReductionDistanceThreshold);
                    if (!(Vector3.Dot((intVec2.ToVector3() - drawPos).normalized, vector) <= 0.5f))
                    {
                       
                            MoteDualAttached mote = MoteMaker.MakeInteractionOverlay(DD_ThingDefOf.DD_Mote_IncineratorBurst_Necro, new TargetInfo(intVec, map), new TargetInfo(intVec2, map));
                        incineratorSpray.Add(new IncineratorProjectileMotion
                        {
                            mote = mote,
                            targetDest = a.Cell,
                            worldSource = drawPos + vector * Props.barrelOffsetDistance,
                            worldTarget = intVec2.ToVector3(),
                            moveVector = vector,
                            startScale = Rand.Range(1f, 2.2f) * num2,
                            endScale = (1f + Rand.Range(0.7f, 1.8f)) * num2,
                            lifespanTicks = Mathf.FloorToInt(num * 5f) + Rand.Range(-Props.lifespanNoise, Props.lifespanNoise)
                        });
                        map.effecterMaintainer.AddEffecterToMaintain(Props.effecterDef.Spawn(intVec2, map), intVec2, 100);
                    }
                }
            },
            ticksAwayFromCast = 5
        };
    }
    }

}

