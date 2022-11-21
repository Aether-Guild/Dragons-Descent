using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class Bill_Ritual : Bill_Production
    {
        public Bill_Ritual() { }

        public Bill_Ritual(RecipeDef recipe) : base(recipe) { }

        public override void Notify_DoBillStarted(Pawn billDoer)
        {
            base.Notify_DoBillStarted(billDoer);

            if (billStack.billGiver is Building_WorkTable thing)
            {
                thing.BroadcastCompSignal(CompFlickable.FlickedOnSignal);
            }
        }

        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);
            Map.GetComponent<MapComponent_Tracker>().Rituals.Current += recipe.GetModExtension<RitualFactorModExtension>()?.factor.RandomInRange ?? 1f;

            if (billStack.billGiver is Building_WorkTable thing)
            {
                thing.BroadcastCompSignal(CompFlickable.FlickedOffSignal);
            }
        }
    }
}
