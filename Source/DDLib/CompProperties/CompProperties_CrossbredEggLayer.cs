using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DD
{

    public class DominantProperties
    {
        public Pawn donor;
        public CompProperties_EggLayer props;

        public Pawn Donor => donor;
        public CompProperties_EggLayer Props => props;

        public DominantProperties(Pawn donor, CompProperties_EggLayer props)
        {
            this.donor = donor;
            this.props = props;
        }
    }

    public class CompProperties_CrossbredEggLayer : CompProperties_EggLayer
    {
        public float failChance = 0.5f; //Chances of out right not fertilizing.
        public float paternalChance = 0.5f; //Chances of the egg def being the father's.
        public float maternalChance = 0.5f; //Chances of the egg def being the mother's.

        public CompProperties_CrossbredEggLayer()
        {
            this.compClass = typeof(CompCrossbredEggLayer);
        }

        public DominantProperties SelectDominantProperties(List<Pawn> donors, Pawn recipient)
        {
            //Log.Message(recipient.ToStringSafe() + ": Rolling for dominant properties...");
            if (!Rand.Chance(failChance))
            {
				if (donors.Count > 0)
                { //Has father candidates.
                    if (Rand.Chance(paternalChance + maternalChance))
                    {
                        //Common chances
                        if (Rand.Chance(maternalChance))
                        {
                            //Mother's egg def dominates.
                            //Log.Message(recipient.ToStringSafe() + ": Maternal");
                            return GenerateMotherEggProperties(recipient);
                        }
                        else
                        {
                            //Father's egg def dominates.
                            //Log.Message(recipient.ToStringSafe() + ": Paternal");
                            return GenerateFatherEggProperties(donors, recipient);
                        }
                    }
                    else
                    {
                        //Randomly decide. Dunno? Should we spawn a special egg 1% of the time?
                        //Log.Message(recipient.ToStringSafe() + ": Rare");
                        return GenerateRareEggProperties(donors, recipient);
                    }
                }
                else
                {
                    //Fertilized, but we don't know who the father is.
                    //Log.Message(recipient.ToStringSafe() + ": Asexual");
                    return GenerateMotherEggProperties(recipient);
                }
            }
            else
            {
                //Failed to fertilize. Will produce an unfertilized egg.
                //Log.Message(recipient.ToStringSafe() + ": Failure");
                return GenerateFailedEggProperties(donors, recipient);
            }
        }

        private DominantProperties GenerateFatherEggProperties(List<Pawn> donors, Pawn recipient)
        {
            //Get random parent which has a CompEggLayer.
            Pawn pawn = donors.Where(p => p.TryGetComp<CompEggLayer>() != null).RandomElementWithFallback(null) ?? recipient;
            CompProperties_EggLayer props = pawn.TryGetComp<CompEggLayer>().Props;

            DominantProperties dprops = new DominantProperties(pawn, MemberwiseClone() as CompProperties_EggLayer);

            dprops.Props.eggFertilizedDef = props.eggFertilizedDef;
            dprops.Props.eggUnfertilizedDef = props.eggUnfertilizedDef;

            return dprops;
        }

        private DominantProperties GenerateMotherEggProperties(Pawn recipient)
        {
            //We already have access to 'this' parent.
            Pawn pawn = recipient;
            CompProperties_EggLayer props = this;

            DominantProperties dprops = new DominantProperties(pawn, MemberwiseClone() as CompProperties_EggLayer);

            dprops.Props.eggFertilizedDef = props.eggFertilizedDef;
            dprops.Props.eggUnfertilizedDef = props.eggUnfertilizedDef;

            return dprops;
        }

        private DominantProperties GenerateRareEggProperties(List<Pawn> donors, Pawn recipient)
        {
            //Collect all the parents, then remove any duplicate properties, and finally randomly select one
            Pawn pawn = donors.Concat(recipient).Where(p => p.TryGetComp<CompEggLayer>() != null).GroupBy(p => p.def).Select(defs => defs.RandomElement()).RandomElementWithFallback(null) ?? recipient; //Try to filter, if we get null, just select the mother.
            CompProperties_EggLayer props = pawn.TryGetComp<CompEggLayer>().Props;

            DominantProperties dprops = new DominantProperties(pawn, MemberwiseClone() as CompProperties_EggLayer);

            dprops.Props.eggFertilizedDef = props.eggFertilizedDef;
            dprops.Props.eggUnfertilizedDef = props.eggUnfertilizedDef;

            //tl;dr Randomly select one of the egg defs of all the possible parents. (all the father candidates and the mother)
            //Is equal opportunity for each egg def to be selected. (No bias)
            return dprops;
        }

        private DominantProperties GenerateFailedEggProperties(List<Pawn> donors, Pawn recipient)
        {
            DominantProperties dprops = GenerateRareEggProperties(donors, recipient); //Will select a random parent

            dprops.donor = null; //Will remove the parent from the egg.
            dprops.Props.eggFertilizationCountMax = 0; //Will de-fertilize the egg the next Fertilize call happens.

            return dprops;
        }
    }
}
