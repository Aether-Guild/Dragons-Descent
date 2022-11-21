using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class DamageWorker_ExtinguishEffect : DamageWorker
    {
        private const float DamageAmountToSizeRatio = 0.01f;

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            AbstractEffect effect = victim as AbstractEffect;

            if (!effect.DestroyedOrNull() && effect.Extinguishable)
            {
                base.Apply(dinfo, victim);
                effect.Extinguish(dinfo.Amount * DamageAmountToSizeRatio);
            }

            return new DamageResult();
        }
    }
}
