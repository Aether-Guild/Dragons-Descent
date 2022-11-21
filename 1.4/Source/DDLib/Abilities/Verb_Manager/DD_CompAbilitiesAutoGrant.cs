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
//using MVCF.Utilities;



//namespace DD.Abilities.Verb_Manager
//{
//    public class CompAbilitiesAutoGrant : CompAbilities
//    {
//        public new CompProperties_AbilitiesAutoGrant Props => (CompProperties_AbilitiesAutoGrant)props;

//        public override void PostSpawnSetup(bool respawningAfterLoad)
//        {
//            base.PostSpawnSetup(respawningAfterLoad);

//            if (Props.abilities == null) return;

//            foreach (var ability in Props.abilities)
//            {
//                ability.verbProperties.verbClass = typeof(Verb_CastAbility_Animal);
//                ability.verbProperties.hasStandardCommand = true;
//                GiveAbility(ability);
//            }

//            var manager = Pawn.Manager();
//            if (manager != null)
//            {
//                foreach (var ability in LearnedAbilities)
//                {
//                    if (ability.GetVerb != null)
//                    {
//                        var verb = new ManagedVerb_Animal();
//                        verb.Initialize(ability.GetVerb, null, null);
//                        manager.AddVerb(ability.GetVerb, VerbSource.RaceDef);
//                    }
//                }
//            }
//        }
//    }
//}
