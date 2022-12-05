using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    public class WingedFlyerVariant
    {
        public string variantPath;
        public AlternateGraphic variantData;
    }

    public class WingedFlyerExtension : DefModExtension
    {
        public float flightSpeed;
        public GraphicData flyingGraphicData;
        public List<WingedFlyerVariant> variants;

        public SimpleCurve travelFlightCurve;
        public float minimumWeight;
    }
}
