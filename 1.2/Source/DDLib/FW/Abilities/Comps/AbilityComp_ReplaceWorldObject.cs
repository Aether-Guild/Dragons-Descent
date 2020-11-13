using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class AbilityComp_ReplaceWorldObject : CompAbilityEffect
    {
        public AbilityCompProperties_ReplaceWorldObject CProps => (AbilityCompProperties_ReplaceWorldObject)props;

        public override void Apply(GlobalTargetInfo target)
        {
            MapParent oldParent = Find.WorldObjects.MapParentAt(target.Tile);
            MapParent newParent = (MapParent)WorldObjectMaker.MakeWorldObject(CProps.replacementWorldObjectDef);

            if (oldParent.HasMap)
            {
                //Kill everything
                if(CProps.killDamageDef != null)
                {
                    foreach (Thing thing in oldParent.Map.spawnedThings.OfType<Pawn>())
                    {
                        thing.Kill(new DamageInfo(CProps.killDamageDef, float.MaxValue, instigator: parent.pawn));
                    }
                }
                Current.Game.DeinitAndRemoveMap(oldParent.Map);
            }

            if (oldParent.Faction != null)
            {
                //Carry over the faction
                newParent.SetFaction(oldParent.Faction);

                FactionRelationKind playerRelationKind = newParent.Faction.PlayerRelationKind;
                if (!newParent.Faction.HostileTo(Faction.OfPlayer))
                {
                    newParent.Faction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Hostile);
                }

                if (CProps.attackedGoodwillChange != 0)
                {
                    newParent.Faction.TryAffectGoodwillWith(parent.pawn.Faction, CProps.attackedGoodwillChange, lookTarget: newParent);
                }

                if (CProps.otherGoodwillChange != 0)
                {
                    foreach (Faction faction in Find.FactionManager.AllFactionsListForReading.Where(f => !f.defeated && f != oldParent.Faction))
                    {
                        faction.TryAffectGoodwillWith(parent.pawn.Faction, CProps.otherGoodwillChange);
                    }
                }
            }

            newParent.Tile = oldParent.Tile;

            oldParent.Destroy();

            Find.WorldObjects.Add(newParent);
        }

        public override string ConfirmationDialogText(GlobalTargetInfo target)
        {
            Settlement s = Find.WorldObjects.SettlementAt(target.Tile);

            string txt = "ConfirmAttackFriendlyFaction".Translate(s.LabelCap, s.Faction.Name) + "\n";
            if(CProps.attackedGoodwillChange != 0)
            {
                txt += "Ability_NotifyAttackFaction".Translate(CProps.attackedGoodwillChange.Named("GOODWILL")) + "\n";
            }
            if(CProps.otherGoodwillChange != 0)
            {
                txt += "Ability_NotifyOtherFactions".Translate(CProps.otherGoodwillChange.Named("GOODWILL")) + "\n";
            }
            txt += "\n";
            if(CProps.killDamageDef != null && s.HasMap)
            {
                txt += "Ability_NotifyMapGenocide".Translate();
            }

            return txt;
        }

        public override bool CanApplyOn(GlobalTargetInfo target)
        {
            return target.HasWorldObject && target.WorldObject.def == CProps.searchWorldObjectDef && target.WorldObject.Faction != parent.pawn.Faction;
        }

        public override bool Valid(GlobalTargetInfo target, bool throwMessages = false)
        {
            return target.HasWorldObject && target.WorldObject.def == CProps.searchWorldObjectDef && target.WorldObject.Faction != parent.pawn.Faction;
        }
    }
}
