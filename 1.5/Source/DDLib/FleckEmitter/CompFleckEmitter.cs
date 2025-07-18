using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DD
{
    public class CompFleckEmitter : ThingComp
    {
        public int ticksSinceLastEmitted;

        private bool active = false;

        private int pawnCount = 0;
        private CompProperties_FleckEmitter Props => (CompProperties_FleckEmitter)props;

        public override void CompTick()
        {
            /*CompPowerTrader comp = parent.GetComp<CompPowerTrader>();
			if (comp != null && !comp.PowerOn)
			{
				return;
			}
			CompSendSignalOnCountdown comp2 = parent.GetComp<CompSendSignalOnCountdown>();
			if ((comp2 != null && comp2.ticksLeft <= 0) || parent == null || parent.Map == null)
			{
				return;
			}*/
            if (!active && Props.alwaysOn)
            {
                active = true;
            }
            if (!active)
            {
                return;
            }
            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
            currentViewRect.ClipInsideMap(parent.Map);
            if (!currentViewRect.Contains(parent.Position))
            {
                return;
            }
            base.CompTick();
            if (ticksSinceLastEmitted <= 0)
            {
                if (Props.moteDefs.Count > 0)
                {
                    for (int i = 0; i < Props.countPerEmit; i++)
                    {
                        Emit();
                    }
                }
                ticksSinceLastEmitted = Rand.Range(Props.minEmissionInterval, Props.maxEmissionInterval);
            }
            else
            {
                ticksSinceLastEmitted--;
            }
        }

        protected void Emit()
        {
            if (parent is Projectile projectile)
            {
                MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(Props.moteDefs.RandomElement());
                moteThrown.Scale = Rand.Range(Props.scale, Props.scaleMax);
                moteThrown.rotationRate = Rand.Range(Props.rotationMinRate, Props.rotationMaxRate);
                moteThrown.exactPosition = projectile.DrawPos + new Vector3(Rand.Range(Props.minRandomOffsetX, Props.maxRandomOffsetX), 0.5f, Rand.Range(Props.minRandomOffsetY, Props.maxRandomOffsetY)); ;
                moteThrown.exactRotation = projectile.Rotation.AsAngle;
                if (Props.propelBackward)
                {
                    moteThrown.Velocity = (parent.Position.ToVector3() - projectile.usedTarget.Cell.ToVector3()) / Props.propelSlowdown;
                }
                else
                {
                    moteThrown.SetVelocity((float)Rand.Range(Props.minRotate, Props.maxRotate) + parent.Rotation.AsAngle, Rand.Range(Props.minSpeed, Props.maxSpeed));
                }
                GenSpawn.Spawn(moteThrown, parent.Position, parent.Map);
            }
        }

    }
}
