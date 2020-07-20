using RimWorld;
using Verse;

namespace DD
{
    public class CompTargetEffect_SpawnMate : CompTargetEffect
    {
        private CompProperties_SpawnMate smProps => props is CompProperties_SpawnMate ? props as CompProperties_SpawnMate : default(CompProperties_SpawnMate);

        public override void DoEffectOn(Pawn user, Thing target)
        {
            Pawn pawn = (Pawn)target;
            if (!pawn.Dead && pawn.ageTracker.CurLifeStage.reproductive)
            {
                //Penalty is only considered when the targeted pawn has the hediff and theres a penalty mental state def set.
                bool penalty = HasHediff(pawn) && smProps.penaltyMentalDef != null;
                Pawn mate = null;

                //If (allowed to spawn during penalty then spawn) or (not allowed to spawn and not in penalty)
                if (smProps.spawnDuringPenalty || !penalty)
                {
                    mate = MateUtils.SpawnMate(pawn, smProps.spawnTamed, smProps.spawnedAgeRange.RandomInRange);
                }

                //Handle the target's effects.
                if (penalty && smProps.targetPenalty)
                {
                    TryStartMentalState(pawn, user);
                }
                else
                {
                    //If the we should give the target hediff, give it the normal hediff, otherwise, give it the pack hediff.
                    TryApplyHediff(pawn, !smProps.targetHediff); //The targetted pawn.
                }

                //If the mate has been spawned, process the mate's effects.
                if (mate != null)
                {
                    if (penalty && smProps.matePenalty)
                    {
                        TryStartMentalState(mate, user);
                    }
                    else
                    {
                        //If the we should give the mate hediff, give it the normal hediff, otherwise, give it the pack hediff.
                        TryApplyHediff(mate, !smProps.mateHediff); //The spawned pawn.
                    }
                }
            }
        }

        private void TryApplyHediff(Pawn pawn, bool packVersion = false)
        {
            HediffDef selectedHediff = null;

            switch (pawn.gender)
            {
                case Gender.Male:
                    selectedHediff = packVersion ? DD_HediffDefOf.DraconicPackFerocity_Male : DD_HediffDefOf.DraconicFerocity_Male;
                    break;
                case Gender.Female:
                    selectedHediff = packVersion ? DD_HediffDefOf.DraconicPackFerocity_Female : DD_HediffDefOf.DraconicFerocity_Female;
                    break;
            }

            if (selectedHediff != null)
            {
                HediffGiverUtility.TryApply(pawn, selectedHediff, null);
            }
        }

        private void TryStartMentalState(Pawn pawn, Pawn cause)
        {
            if (smProps.penaltyMentalDef != null)
            {
                pawn.mindState.mentalStateHandler.TryStartMentalState(smProps.penaltyMentalDef, smProps.penaltyReason, true, false);
                pawn.mindState.enemyTarget = cause;
            }
        }

        private bool HasHediff(Pawn pawn) =>
               pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicFerocity_Male)
            || pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicFerocity_Female)
            || pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicPackFerocity_Male)
            || pawn.health.hediffSet.HasHediff(DD_HediffDefOf.DraconicPackFerocity_Female);
    }
}
