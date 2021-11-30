﻿using RoR2;
using UnityEngine;
using R2API;
using RiskyMod.Survivors.Commando;

namespace EntityStates.RiskyMod.Commando.Scepter
{
	public class FireBarrageScepter : FireBarrage
	{
		public static new float baseDurationBetweenShots = 0.08f;
		public static new float baseBulletCount = 10;
		public static new float blastRadius = 5f;
		public static new float blastDamageCoefficient = 0.5f;  //Multiply by damage coefficient

        public override void LoadStats()
        {
			internalBaseBulletCount = baseBulletCount;
			internalBaseDurationBetweenShots = baseDurationBetweenShots;
        }

        public override void AddDamageType(BulletAttack ba)
		{
			DamageAPI.AddModdedDamageType(ba, CommandoCore.SuppressiveFireScepterDamage);
		}

		public static void SuppressiveFireScepterAOE(GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
		{
			if (damageInfo.HasModdedDamageType(CommandoCore.SuppressiveFireScepterDamage))
			{
				EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData
				{
					origin = damageInfo.position,
					scale = blastRadius,
					rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
				}, true);
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.position = damageInfo.position;
				blastAttack.baseDamage = damageInfo.damage * blastDamageCoefficient;
				blastAttack.baseForce = 0f;
				blastAttack.radius = blastRadius;
				blastAttack.attacker = damageInfo.attacker;
				blastAttack.inflictor = null;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.crit = damageInfo.crit;
				blastAttack.procChainMask = damageInfo.procChainMask;
				blastAttack.procCoefficient = 0.5f;
				blastAttack.damageColorIndex = DamageColorIndex.Item;
				blastAttack.falloffModel = BlastAttack.FalloffModel.None;
				blastAttack.damageType = damageInfo.damageType;
				blastAttack.Fire();
			}
		}
	}
}
