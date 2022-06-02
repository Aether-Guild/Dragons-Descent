using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    //public class MoteProperties_Sequence : MoteProperties
    //{
    //    public float graphicAngle;
    //}

    //public class Mote_Sequenced : Mote
    //{
    //    private TargetInfo source, target;

    //    public override void Tick()
    //    {
    //        base.Tick();
    //    }

    //    protected override void TimeInterval(float deltaTime)
    //    {
    //        base.TimeInterval(deltaTime);
    //    }
    //}

    //public class Mote_Resizer : Mote_Sequenced
    //{
    //    private Vector2 size;
    //    private float angle;

    //    public bool Expired => duration <= 0;

    //    public float Speed => def.mote is MoteProperties_Controller prop ? prop.speed.RandomInRange : Rand.Range(0.5f, 0.75f);

    //    public FloatRange DestinationVariance => def.mote is MoteProperties_Controller prop ? prop.destinationVariance : FloatRange.Zero;
    //    public FloatRange RotationVariance => def.mote is MoteProperties_Controller prop ? prop.rotationVariance : FloatRange.Zero;
    //    public FloatRange ScaleVariance => def.mote is MoteProperties_Controller prop ? prop.scaleVariance : FloatRange.One;
    //    public SimpleCurve AlphaCurve => def.mote is MoteProperties_Controller prop ? prop.alphaCurve : null;

    //    public void Setup(IntVec3 start, IEnumerable<IntVec3> cells, float duration)
    //    {
    //        this.duration = duration;
    //        motes = new List<SubMote_Particle>();
    //        foreach (IntVec3 cell in cells)
    //        {
    //            motes.Add(new SubMote_Particle(start.ToVector3Shifted(), cell.ToVector3Shifted(), this));
    //        }
    //    }

    //    protected override void TimeInterval(float deltaTime)
    //    {
    //        base.TimeInterval(deltaTime);
    //        if (Destroyed)
    //        {
    //            return;
    //        }

    //        if (duration > 0)
    //        {
    //            duration -= deltaTime;
    //        }

    //        foreach (SubMote_Particle mote in motes)
    //        {
    //            if (mote.Finished || def.mote.collide && mote.Position.Filled(Map))
    //            {
    //                if (!Expired)
    //                {
    //                    mote.Restart();
    //                }
    //            }
    //            else
    //            {
    //                mote.Update(deltaTime);
    //            }
    //        }
    //    }

    //    public override void ExposeData()
    //    {
    //        base.ExposeData();
    //        Scribe_Values.Look(ref duration, "duration");
    //        Scribe_Collections.Look(ref motes, "motes", LookMode.Deep);
    //    }
    //}
}
