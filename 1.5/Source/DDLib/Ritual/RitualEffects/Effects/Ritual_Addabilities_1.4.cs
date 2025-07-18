using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using VFECore.Abilities;

namespace DD
{
    public class Ritual_AddAbilitesToAllDragons : Ritual_MapTargeting
    {
        private IEnumerable<Pawn> AffectedPawns => AllTargets.Where(target => target.HasThing && !target.ThingDestroyed).Select(target => target.Thing).OfType<Pawn>();

        public override void ApplyRitual(TargetInfo target)
        {
            if (target.HasThing && !target.ThingDestroyed && target.Thing is Pawn pawn)
            {

                // Add Dragon_Breath ability to affected pawns
                CompAbilities abilitiesComp = pawn.TryGetComp<CompAbilities>();
                if (abilitiesComp != null)
                {
                    VFECore.Abilities.AbilityDef DD_DragonBreath_Fire = DefDatabase<VFECore.Abilities.AbilityDef>.GetNamed("DD_DragonBreath_Fire");
                    if (DD_DragonBreath_Fire == null)
                    {
                        Log.ErrorOnce("Could not find the Dragon_Breath ability.", 18463572);
                    }
                    else
                    {
                        abilitiesComp.GiveAbility(DD_DragonBreath_Fire);
                    }
                    VFECore.Abilities.AbilityDef DD_DragonJump = DefDatabase<VFECore.Abilities.AbilityDef>.GetNamed("DD_DraconicFlight");
                    if (DD_DragonJump == null)
                    {
                        Log.ErrorOnce("Could not find the DragonJump ability.", 18463572);
                    }
                    else
                    {
                        abilitiesComp.GiveAbility(DD_DragonJump);
                    }
                    VFECore.Abilities.AbilityDef DD_DragonSpit_Fire = DefDatabase<VFECore.Abilities.AbilityDef>.GetNamed("DD_DragonSpit_Fire");
                    if (DD_DragonSpit_Fire == null)
                    {
                        Log.ErrorOnce("Could not find the Dragon Spit ability.", 18463572);
                    }
                    else
                    {
                        abilitiesComp.GiveAbility(DD_DragonSpit_Fire);
                    }
                    VFECore.Abilities.AbilityDef GR_DraconicFlight = DefDatabase<VFECore.Abilities.AbilityDef>.GetNamed("GR_DraconicFlight");
                    if (GR_DraconicFlight == null)
                    {
                        Log.ErrorOnce("Could not find the GRDragon Flight ability.", 18463572);
                    }
                    else
                    {
                        abilitiesComp.GiveAbility(GR_DraconicFlight);
                    }
                }
            }
        }
    }
}


