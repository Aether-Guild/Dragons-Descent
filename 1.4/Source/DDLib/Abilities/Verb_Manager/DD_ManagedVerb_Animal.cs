//using System;
//using System.Collections.Generic;
//using System.Linq;
//using HarmonyLib;
//using RimWorld;
//using UnityEngine;
//using Verse;
//using RimWorld.Planet;
//using VFECore.Abilities;
//using Ability = VFECore.Abilities.Ability;
//using MVCF;
//using MVCF.Comps;
//using MVCF.Utilities;
//using Verb_CastAbility = VFECore.Abilities.Verb_CastAbility;
//using MVCF.VerbComps;

//namespace DD.Abilities.Verb_Manager
//{
//    public class ManagedVerb_Animal : ManagedVerb
//    {
//        private Ability ability;

//        public override void Initialize(Verb verb, AdditionalVerbProps props, IEnumerable<VerbCompProperties> additionalComps)
//        {
//            base.Initialize(verb, props, additionalComps);

//            if (verb is Verb_CastAbility castAbility)
//                ability = castAbility.ability;
//        }

//        public override IEnumerable<Gizmo> GetGizmos(Thing ownerThing)
//        {
//            yield break;
//        }

//        public override bool Available()
//        {
//            if (ability == null)
//                return true;

//            return ability.IsEnabledForPawn(out _) && ability.AutoCast;
//        }
//    }
//}