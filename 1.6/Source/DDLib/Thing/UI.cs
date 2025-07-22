// using HarmonyLib;
// using RimWorld;
// using Verse;
//
// namespace DD;
//
// public class CompFireBurst_Dragon : ThingComp
// {
//     private CompExplosive compExplosive;
//
//     private Effecter effecter;
//
//     private CompProperties_AbilityFireBurst_Dragon Props => (CompProperties_AbilityFireBurst_Dragon)props;
//
//     private CompExplosive CompExplosive
//     {
//         get
//         {
//             if (compExplosive == null)
//             {
//                 compExplosive = parent.TryGetComp<CompExplosive>();
//             }
//             return compExplosive;
//         }
//     }
//
//     public override void CompTick()
//     {
//         if (CompExplosive.wickStarted)
//         {
//             FireBurstUtility.ThrowFuelTick(parent.Position, Props.radius, parent.Map);
//             if (CompExplosive.wickTicksLeft <= Props.ticksAwayFromDetonate && effecter == null)
//             {
//                 effecter = EffecterDefOf.Fire_Burst.Spawn(parent.Position, parent.Map);
//             }
//         }
//         effecter?.EffectTick(parent, parent);
//     }
// }