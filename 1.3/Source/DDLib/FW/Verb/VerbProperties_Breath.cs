using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class VerbProperties_Breath : VerbProperties
    {
        public SimpleCurve effectiveRange;

        public SimpleCurve angle;

        public DamageDef damageDef;
        public SimpleCurve damageAmount;
    }
}
