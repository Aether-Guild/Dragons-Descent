using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DD
{
	public class CompProperties_FleckEmitter : CompProperties
	{

		public float scale = 1f;

		public float scaleMax = 1f;

		public float minRandomOffsetX = 0f;

		public float maxRandomOffsetX = 0f;

		public float minRandomOffsetY = 0f;

		public float maxRandomOffsetY = 0f;

		public float rotationMinRate;

		public float rotationMaxRate;

		public float rotationRate;

		public int minRotate;

		public int maxRotate;

		public float minSpeed;

		public float maxSpeed;

		public float propelSlowdown;

		public Vector3 offset;

		public int minEmissionInterval;

		public int maxEmissionInterval;

		public SoundDef soundOnEmission;

		public List<ThingDef> moteDefs;

		public bool alwaysOn = false;

		public int countPerEmit = 1;

		public bool propelBackward = false;

		public CompProperties_FleckEmitter()
		{
			compClass = typeof(CompFleckEmitter);
		}
	}
}
