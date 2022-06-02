using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    //public abstract class Alert_RitualEffect : Alert_Custom
    //{
    //    public abstract RitualDef Def { get; }
    //    public abstract AlertReport GetAlertReport(ITickingRitual ritual);

    //    public Map Map => Find.CurrentMap;
    //    public RitualTracker Tracker => Map?.GetComponent<MapComponent_Tracker>()?.Rituals ?? null;

    //    public Alert_RitualEffect()
    //    {
    //        defaultPriority = AlertPriority.Medium;
    //    }

    //    public override TaggedString GetExplanation()
    //    {
    //        string exp = base.GetExplanation();

    //        RitualTracker rituals = Tracker;
    //        if (rituals != null)
    //        {
    //            if (rituals[Def] != null && rituals[Def].Active)
    //            {
    //                exp = exp.Formatted(rituals[Def].DurationRemaining.ToStringTicksToPeriodVague().Named("DURATION"));
    //            }
    //        }

    //        return exp;
    //    }

    //    public override AlertReport GetReport()
    //    {
    //        RitualTracker rituals = Tracker;
    //        if (Map != null && rituals != null && rituals[Def] != null && rituals[Def].Active && rituals[Def] is ITickingRitual)
    //        {
    //            if (!report.active)
    //            {
    //                report = GetAlertReport(rituals[Def] as ITickingRitual);
    //            }
    //        }
    //        else
    //        {
    //            if (report.active)
    //            {
    //                report = AlertReport.Inactive;
    //            }
    //        }
    //        return base.GetReport();
    //    }
    //}
}
