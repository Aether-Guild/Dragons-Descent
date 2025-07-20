using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFECore.Abilities;

namespace DD
{
    public class AbilityDragonStraightPawnFlyer : AbilityPawnFlyer
    {


        /* public override void Tick()
         {

             float progress = (float)this.ticksFlying / (float)this.ticksFlightTime;
             this.position = Vector3.Lerp(this.startVec, this.target, progress);


             IList value = Traverse.Create(this.FlyingPawn.Drawer.renderer).Field("effecters").Field("pairs").GetValue() as IList;
             foreach (object o in value)
                 Traverse.Create(o).Field("effecter").GetValue<Effecter>().EffectTick(new TargetInfo(this.position.ToIntVec3(), this.Map), TargetInfo.Invalid);

             base.Tick();
         }
        */


    }
}