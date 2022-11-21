using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using System.Linq;

namespace DD
{
    public class Verb_MeleeAttackDamage_Resetting : Verb_MeleeAttackDamage
    {
        protected override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                if(CasterIsPawn)
                { //Making sure the user is a pawn. (What else could it be?)
                    foreach (Verb_Shoot_Limited verb in VerbUtils.GetVerbs(CasterPawn).OfType<Verb_Shoot_Limited>())
                    {
                       // Log.Message(CasterPawn.ToStringSafe()+"'s "+verb.ToStringSafe()+" was reset");
                        verb.Refresh(); //Reset all limited verbs
                    }
                }
                return true;
            }
            return false;
        }
    }
}
