using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD

{
	//public class Ability_Shoot : VFECore.Abilities.Ability
	//{
	//	AbilityExtension_Roll1D20 modExtension => def.GetModExtension<AbilityExtension_Roll1D20>();
	//	public override void Cast(params GlobalTargetInfo[] targets)
	//	{
	//		base.Cast(targets);
	//		RollInfo rollinfo = new RollInfo();
	//		rollinfo = MakaiUtility.Roll1D20(pawn, modExtension.skillBonus, rollinfo);
	//		if (rollinfo.roll >= modExtension.successThreshold && rollinfo.roll < modExtension.greatSuccessThreshold)
	//		{
	//			foreach (GlobalTargetInfo globalTargetInfo in targets)
	//			{
	//				for (int i = 0; i < modExtension.projectileBurstCount; i++)
	//				{
	//					IntVec3 spawnPosition;
	//					if (modExtension.spawnProjectileAtRandom)
	//					{
	//						spawnPosition = pawn.Position.RandomAdjacentCell8Way();
	//					}
	//					else
	//					{
	//						spawnPosition = pawn.Position;
	//					}
	//					Projectile projectile = (Projectile)GenSpawn.Spawn(modExtension.projectileWhenSuccess, pawn.Position, pawn.Map);
	//					if (globalTargetInfo.HasThing)
	//					{
	//						projectile.Launch(pawn, spawnPosition.ToVector3(), globalTargetInfo.Thing, globalTargetInfo.Thing, ProjectileHitFlags.IntendedTarget);
	//					}
	//					else
	//					{
	//						projectile.Launch(pawn, spawnPosition.ToVector3(), globalTargetInfo.Cell, globalTargetInfo.Cell, ProjectileHitFlags.IntendedTarget);
	//					}
	//				}
	//				Messages.Message("Makai_PassArollcheck".Translate(pawn.LabelShort, rollinfo.baseRoll, rollinfo.cumulativeBonusRoll, pawn.Named("USER")), pawn, MessageTypeDefOf.PositiveEvent);
	//				Messages.Message("Makai_PassArollcheckShoot".Translate(pawn.LabelShort, globalTargetInfo.Label, pawn.Named("USER")), pawn, MessageTypeDefOf.PositiveEvent);
	//			}
	//		}
	//		if (rollinfo.roll >= modExtension.greatSuccessThreshold)
	//		{
	//			foreach (GlobalTargetInfo globalTargetInfo in targets)
	//			{
	//				for (int i = 0; i < modExtension.projectileBurstCount * 2; i++)
	//				{
	//					Projectile projectile = (Projectile)GenSpawn.Spawn(modExtension.projectileWhenGreatSuccess ?? modExtension.projectileWhenSuccess, pawn.Position, pawn.Map);
	//					if (globalTargetInfo.HasThing)
	//					{
	//						projectile.Launch(pawn, pawn.DrawPos, globalTargetInfo.Thing, globalTargetInfo.Thing, ProjectileHitFlags.IntendedTarget);
	//					}
	//					else
	//					{
	//						projectile.Launch(pawn, pawn.DrawPos, globalTargetInfo.Cell, globalTargetInfo.Cell, ProjectileHitFlags.IntendedTarget);
	//					}
	//				}
	//				Messages.Message("Makai_GreatPassArollcheck".Translate(pawn.LabelShort, rollinfo.baseRoll, rollinfo.cumulativeBonusRoll, pawn.Named("USER")), pawn, MessageTypeDefOf.PositiveEvent);
	//				Messages.Message("Makai_GreatPassArollcheckShoot".Translate(pawn.LabelShort, globalTargetInfo.Label, pawn.Named("USER")), pawn, MessageTypeDefOf.PositiveEvent);
	//			}
	//		}
	//		if (rollinfo.roll < modExtension.successThreshold)
	//		{
	//			foreach (GlobalTargetInfo globalTargetInfo in targets)
	//			{
	//				for (int i = 0; i < modExtension.projectileBurstCount; i++)
	//				{
	//					Projectile projectile = (Projectile)GenSpawn.Spawn(modExtension.projectileWhenFail ?? modExtension.projectileWhenSuccess, pawn.Position, pawn.Map);
	//					if (globalTargetInfo.HasThing)
	//					{
	//						projectile.Launch(pawn, pawn.DrawPos, globalTargetInfo.Thing.RandomAdjacentCell8Way(), globalTargetInfo.Thing.RandomAdjacentCell8Way(), ProjectileHitFlags.IntendedTarget);
	//					}
	//					else
	//					{
	//						projectile.Launch(pawn, pawn.DrawPos, globalTargetInfo.Cell.RandomAdjacentCell8Way(), globalTargetInfo.Cell.RandomAdjacentCell8Way(), ProjectileHitFlags.IntendedTarget);
	//					}
	//				}
	//				Messages.Message("Makai_FailArollcheck".Translate(pawn.LabelShort, rollinfo.baseRoll, rollinfo.cumulativeBonusRoll, pawn.Named("USER")), pawn, MessageTypeDefOf.PositiveEvent);
	//				Messages.Message("Makai_FailArollcheckShoot".Translate(pawn.LabelShort, globalTargetInfo.Label, pawn.Named("USER")), pawn, MessageTypeDefOf.PositiveEvent);
	//			}
	//		}
	//	}
	//}
}
