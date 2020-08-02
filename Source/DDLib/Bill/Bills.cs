using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class RitualFactorModExtension : DefModExtension
    {
        public FloatRange factor;
    }

    public abstract class BillGeneratorOverrideExtension : DefModExtension
    {
        public abstract Bill NewBill(RecipeDef recipe);
    }

    public class RitualBillOverrideExtension : BillGeneratorOverrideExtension
    {
        public override Bill NewBill(RecipeDef recipe) => new Bill_Ritual(recipe);
    }

    public class LoadIncubatorBillOverrideExtension : BillGeneratorOverrideExtension
    {
        public override Bill NewBill(RecipeDef recipe) => new Bill_LoadIncubator(recipe);
    }

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

    public class Bill_LoadIncubator : Bill_Production
    {
        public Bill_LoadIncubator() { }

        public Bill_LoadIncubator(RecipeDef recipe) : base(recipe) { }

        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            base.Notify_IterationCompleted(billDoer, ingredients);
            if (!ingredients.NullOrEmpty())
            {
                if (billStack.billGiver is Building_WorkTable thing)
                {
                    thing.TryGetComp<CompEggIncubator>()?.IncubateEgg(ingredients.First());
                }
            }
        }
    }
}
