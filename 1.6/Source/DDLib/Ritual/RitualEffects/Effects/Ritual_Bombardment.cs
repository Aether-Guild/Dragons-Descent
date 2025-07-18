using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DD
{
    public class Ritual_Bombardment : Ritual_TargetingTicking
    {
        private IEnumerable<IntVec3> cells;

        private float radius = -1;
        private int bombardmentTick = 0;

        protected ThingDef BombardmentThingDef => Def.GetModExtension<RitualDamageModExtension>() is RitualDamageModExtension ext ? ext.bombardmentThingDef : ThingDefOf.MeteoriteIncoming;
        protected float BombardmentRadius => Def.GetModExtension<RitualDamageModExtension>() is RitualDamageModExtension ext ? ext.bombardmentRadius.RandomInRange : 30f;
        protected int NextBombardmentInterval => GenTicks.SecondsToTicks(Def.GetModExtension<RitualDamageModExtension>() is RitualDamageModExtension ext ? ext.bombardmentInterval.RandomInRange : 0.5f);

        protected override void PreActivation()
        {
            base.PreActivation();
            Unset();
            EnsureInitialized();
        }

        protected override void PostDeactivation()
        {
            base.PostDeactivation();
            Unset();
        }

        private void EnsureInitialized()
        {
            if(radius <= 0)
            {
                radius = BombardmentRadius;
            }

            if(cells == null)
            {
                cells = GenRadial.RadialCellsAround(Target.Cell, radius, true).Where(c => c.IsValid && c.InBounds(Target.Map)).InRandomOrder();
            }
        }

        private void Unset()
        {
            radius = -1;
            cells = null;
        }

        public override void ApplyRitual(TargetInfo target)
        {
            if (bombardmentTick <= 0)
            {
                EnsureInitialized();

                if (cells.TryRandomElement(out IntVec3 cell))
                {
                    if (cell.Roofed(target.Map))
                    {
                        //Replace thick roof when spawned.
                        foreach(IntVec3 rc in GenRadial.RadialCellsAround(cell, BombardmentThingDef.skyfaller.explosionRadius, true).Where(c => c.IsValid && c.InBounds(Target.Map) && c.Roofed(target.Map)))
                        {
                            target.Map.roofGrid.SetRoof(rc, RoofDefOf.RoofConstructed);
                        }
                    }

                    SkyfallerMaker.SpawnSkyfaller(BombardmentThingDef, cell, target.Map);
                }
                else
                {
                    //No targets.
                    Deactivate();
                }
                bombardmentTick = NextBombardmentInterval;
            }
            else
            {
                bombardmentTick--;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref radius, "radius", -1);
            Scribe_Values.Look(ref bombardmentTick, "bombardmentTick", 0);
        }
    }
}
