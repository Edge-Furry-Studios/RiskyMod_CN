﻿using R2API;
namespace RiskyMod.Enemies.Spawnpools
{
    public class SnowyForest
    {
        public static bool enabled = true;
        public SnowyForest()
        {
            if (!enabled) return;

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.BlindVerminSnowy, DirectorAPI.Stage.SiphonedForest);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.Beetle, DirectorAPI.Stage.SiphonedForest);
            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.LesserWisp, DirectorAPI.Stage.SiphonedForest);

            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.BisonLoop, false, DirectorAPI.Stage.SiphonedForest);
            DirectorAPI.Helpers.AddNewMonsterToStage(DirectorCards.BlindVerminSnowy, false, DirectorAPI.Stage.SiphonedForest);
        }
    }
}
