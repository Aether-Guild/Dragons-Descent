using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DD
{
    public class CompCrossbredEggLayer : CompEggLayer
    {
        public List<Pawn> donors = new List<Pawn>();

        private CompProperties_CrossbredEggLayer CBProps { get; set; }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            CBProps = (CompProperties_CrossbredEggLayer)props; //Make sure it errors out rather than having it be null.
        }

        public void FertilizeWithTracking(Pawn male)
        {
            donors.Add(male); //Keep track of the fertilization history.
            base.Fertilize(male);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref donors, "donors", LookMode.Reference);
        }

        public override Thing ProduceEgg()
        {
            Thing egg = null;

            if (this.FullyFertilized && this.CanLayNow)
            {
                DominantProperties dominant = CBProps.SelectDominantProperties(donors, parent as Pawn);
                try
                {
                    this.props = dominant.Props; //Swap the props.
                    
                    base.Fertilize(dominant.Donor); //Set the parent to be the selected donor.
                    egg = base.ProduceEgg(); //Produce the egg.

                    if(egg != null && dominant.notify)
                    {
                        Messages.Message("DragonLayingEggMessage".Translate(parent.Named("PARENT"), egg.LabelNoCount.Named("EGG")), new LookTargets(egg), MessageTypeDefOf.PositiveEvent);
                    }
                    
                    donors.Clear(); //Clear the previous round of donors.
                }
                finally
                {
                    this.props = CBProps; //Back to normal props.
                }
            }

            return egg;
        }
    }
}
