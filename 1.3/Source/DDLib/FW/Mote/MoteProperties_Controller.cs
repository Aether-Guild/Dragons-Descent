using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class MoteProperties_Controller : MoteProperties
    {
        public FloatRange speed;
        public FloatRange destinationVariance, rotationVariance, scaleVariance;
        public SimpleCurve alphaCurve;
    }
}
