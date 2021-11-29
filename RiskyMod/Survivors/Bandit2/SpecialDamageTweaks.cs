﻿using RiskyMod.SharedHooks;
using RoR2;
using R2API;
using EntityStates.RiskyMod.Bandit2.Revolver;
using MonoMod.Cil;
using System;
using Mono.Cecil.Cil;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class SpecialDamageTweaks
    {
        public SpecialDamageTweaks()
        {
            OnHitEnemy.OnHitNoAttackerActions += ApplyBuff;
            TakeDamage.ModifyInitialDamageActions += RackEmUpBonus;
        }

        private static void RackEmUpBonus(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.RackEmUpDamage))
            {
                float mult = 1f + self.body.GetBuffCount(Bandit2Core.SpecialDebuff) * (FireRackEmUp.bonusDamageCoefficient / FireRackEmUp.damageCoefficient);
                damageInfo.damage *= mult;
            }
        }

        private static void ApplyBuff(DamageInfo damageInfo, CharacterBody victimBody)
        {
            if (damageInfo.HasModdedDamageType(Bandit2Core.SpecialDamage))
            {
                float buffDuration = BanditSpecialGracePeriod.enabled ? BanditSpecialGracePeriod.duration : 1.2f;
                int specialCount = victimBody.GetBuffCount(Bandit2Core.SpecialDebuff) + 1;
                victimBody.ClearTimedBuffs(Bandit2Core.SpecialDebuff);
                for (int i = 0; i < specialCount; i++)
                {
                    victimBody.AddTimedBuff(Bandit2Core.SpecialDebuff, buffDuration);
                }
            }
        }
    }
}
