using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DD
{
    //public class Alert_RitualEffect_MaintainMultiple : Alert_RitualEffect
    //{
    //    public Alert_RitualEffect_MaintainMultiple()
    //    {
    //        label = "AlertRitualSustainMultiple_Label".Translate();
    //        explanation = "AlertRitualSustainMultiple_Desc".Translate();
    //    }

    //    public override RitualDef Def => DD_RitualDefOf.Ritual_MaintainAllDragonNeeds;

    //    public override AlertReport GetAlertReport(Ritual ritual)
    //    {
    //        if(ritual is Ritual_MaintainAllNeeds rman && rman.TargetMap == Map)
    //        {
    //            IEnumerable<Thing> things = rman.AllTargets.Where(target => target.IsValid && target.HasThing && !target.ThingDestroyed).Select(target => target.Thing);
    //            if(things.Any())
    //            {
    //                return AlertReport.CulpritsAre(things.ToList());
    //            }
    //        }
    //        return AlertReport.Inactive;
    //    }
    //}
}
