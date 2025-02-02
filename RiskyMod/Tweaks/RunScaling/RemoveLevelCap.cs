﻿

using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Tweaks.RunScaling
{
    public class RemoveLevelCap
    {
        public static bool enabled = true;
		public static float maxLevel = 9999f;
		public static float stopSound = 300f;
        public RemoveLevelCap()
        {
			if (!enabled) return;
			//Remove level capping when calculating monster level
			if (!LinearScaling.enabled)
			{
				IL.RoR2.Run.RecalculateDifficultyCoefficentInternal += (il) =>
				{
					ILCursor c = new ILCursor(il);
					if(c.TryGotoNext(
						x => x.MatchLdsfld<Run>(nameof(Run.ambientLevelCap))
						))
					{
						c.Remove();
						c.Emit<RemoveLevelCap>(OpCodes.Ldsfld, nameof(RemoveLevelCap.maxLevel));
					}
					else
					{
						UnityEngine.Debug.LogError("RiskyMod: RemoveLevelCap IL Hook failed");
					}
				};
			}
			Run.ambientLevelCap = (int)maxLevel;	//This is for RunScaling.

			On.RoR2.LevelUpEffectManager.OnRunAmbientLevelUp += (orig, run) =>
			{
				if (run.ambientLevel <= stopSound)
				{
					orig(run);
				}
			};
		}
    }
}
