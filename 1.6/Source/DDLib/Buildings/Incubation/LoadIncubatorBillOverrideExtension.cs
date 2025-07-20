using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class LoadIncubatorBillOverrideExtension : BillGeneratorOverrideExtension
    {
        public override Bill NewBill(RecipeDef recipe) => new Bill_LoadIncubator(recipe);
    }
}
