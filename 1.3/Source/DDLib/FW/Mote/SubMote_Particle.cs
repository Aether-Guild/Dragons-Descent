using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class SubMote_Particle : IExposable
    {
        public Vector3 current, start, destination;
        public float progress, increment;

        public Vector3 destinationVariance;
        public float rotationDelta, scaleDelta;

        private Mote_ParticleController controller;

        public bool Finished => progress >= 1;
        public IntVec3 Position => current.ToIntVec3();
        public float RotationAngle => (((Mathf.PI / 2f) - Mathf.Atan2(destination.z - current.z, destination.x - current.x)) * Mathf.Rad2Deg) + rotationDelta;
        public float Alpha
        {
            get
            {
                if(controller.Expired && Finished)
                {
                    return 0f;
                }

                return controller.AlphaCurve?.Evaluate(progress) ?? 1f;
            }
        }

        public SubMote_Particle(Vector3 start, Vector3 destination, Mote_ParticleController controller)
        {
            this.start = start;
            this.destination = destination;
            this.controller = controller;
            Restart();
        }

        public void Restart()
        {
            this.current = start;

            this.progress = 0f;
            this.increment = controller.Speed;

            this.destinationVariance = new Vector3(controller.DestinationVariance.RandomInRange, controller.DestinationVariance.RandomInRange, controller.DestinationVariance.RandomInRange);
            this.rotationDelta = controller.RotationVariance.RandomInRange;
            this.scaleDelta = controller.ScaleVariance.RandomInRange;
        }

        public void Update(float deltaTime)
        {
            progress += (increment * deltaTime);
            current.x = Mathf.Lerp(start.x, destination.x + destinationVariance.x, progress);
            current.y = Mathf.Lerp(start.y, destination.y + destinationVariance.y, progress);
            current.z = Mathf.Lerp(start.z, destination.z + destinationVariance.z, progress);
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref controller, "controller");

            Scribe_Deep.Look(ref current, "current");
            Scribe_Deep.Look(ref start, "start");
            Scribe_Deep.Look(ref destination, "destination");

            Scribe_Values.Look(ref progress, "progress");
            Scribe_Values.Look(ref increment, "increment");

            Scribe_Deep.Look(ref destinationVariance, "destinationVariance");
            Scribe_Deep.Look(ref rotationDelta, "rotationDelta");
        }
    }
}
