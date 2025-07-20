using RimWorld;
using Verse;

namespace DD
{
    public class CompTargetEffect_PawnEquipper : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead)
            {
                CompProperties_Thing vProps = props as CompProperties_Thing;
                if (vProps != null)
                {
                    ThingWithComps thing = ThingMaker.MakeThing(vProps.thingDef) as ThingWithComps;
                    if(!pawn.equipment.Contains(thing))
                    {
                        pawn.equipment.MakeRoomFor(thing);
                        pawn.equipment.AddEquipment(thing);
                    }
                }
            }
        }
    }
}
