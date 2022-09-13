﻿using RoR2;
using R2API;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM2Bite
    {
        public ModifyM2Bite()
        {
            On.EntityStates.Croco.Bite.AuthorityModifyOverlapAttack += (orig, self, overlapAttack) =>
            {
                orig(self, overlapAttack);
                overlapAttack.damageType = DamageType.BonusToLowHealth;
                overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoBiteHealOnKill);

                if (CrocoCore.HasDeeprot(self.skillLocator))
                {
                    overlapAttack.damageType = DamageType.PoisonOnHit | DamageType.BlightOnHit;
                }
                else
                {
                    overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoBlight6s);
                }
            };
        }

        private AnimationCurve BuildBiteVelocityCurve()
        {
            Keyframe kf1 = new Keyframe(0f, 1.5f, -8.182907104492188f, -3.3333332538604738f, 0f, 0.058712735772132876f);
            kf1.weightedMode = WeightedMode.None;
            kf1.tangentMode = 65;

            Keyframe kf2 = new Keyframe(0.3f, 0f, -3.3333332538604738f, -3.3333332538604738f, 0.3333333432674408f, 0.3333333432674408f);    //Time should match up with SlashBlade min duration (hitbox length)
            kf2.weightedMode = WeightedMode.None;
            kf2.tangentMode = 34;

            Keyframe[] keyframes = new Keyframe[2];
            keyframes[0] = kf1;
            keyframes[1] = kf2;

            return new AnimationCurve
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = keyframes
            };
        }
    }
}
