using System;
using Verse;

namespace DD
{
    public class HediffCompProperties_ModifyAge : HediffCompProperties
    {
        public AgeUtils.AgeUpdateMethod updateMethod;

        public float amountPerTick;

        public HediffCompProperties_ModifyAge()
        {
            compClass = typeof(HediffComp_ModifyAge);
        }

        public virtual float Amount
        {
            get
            {
                return amountPerTick;
            }
        }

        public virtual AgeUtils.AgeUpdateMethod Method
        {
            get
            {
                return updateMethod;
            }
        }
    }
}
