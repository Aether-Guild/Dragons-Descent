using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
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
