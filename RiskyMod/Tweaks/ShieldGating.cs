﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using R2API;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    public class ShieldGating
    {
        public static DamageAPI.ModdedDamageType IgnoreShieldGateDamage;
        public static bool enabled = true;
        public ShieldGating()
        {
            if (!enabled) return;
            SetupIgnoreShieldGate();

            //Remove OSP in SharedHooks.RecalculateStats

            //Add Shield Gating
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdloc(7),
                     x => x.MatchLdarg(0),
                     x => x.MatchLdfld<HealthComponent>("shield"),
                     x => x.MatchSub(),
                     x => x.MatchStloc(7),
                     x => x.MatchLdarg(0)
                    );
                c.Index += 4;
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((remainingDamage, self, damageInfo) =>
                {
                    if ((damageInfo.damageType & DamageType.BypassArmor) != DamageType.BypassArmor
                    && (damageInfo.damageType & DamageType.BypassOneShotProtection) != DamageType.BypassOneShotProtection
                    && !DamageAPI.HasModdedDamageType(damageInfo, IgnoreShieldGateDamage)
                    && (self.body && self.body.teamComponent && (self.body.teamComponent.teamIndex == TeamIndex.Player || self.body.isPlayerControlled)))
                    {
                        float duration = 0.1f;

                        //ShieldOnly increases grace period since it's your only form of defense against 1shots.
                        if (self.body.HasBuff(RoR2Content.Buffs.AffixLunar) || (self.body.inventory && self.body.inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0))
                        {
                            duration = 0.3f;
                        }

                        self.body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility.buffIndex, duration);
                        return 0f;
                    }
                    return remainingDamage;
                });
            };
        }

        private void SetupIgnoreShieldGate()
        {
            IgnoreShieldGateDamage = DamageAPI.ReserveDamageType();

            IL.EntityStates.VagrantMonster.FireMegaNova.Detonate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                {
                    blastAttack.AddModdedDamageType(IgnoreShieldGateDamage);
                    return blastAttack;
                });
            };

            IL.EntityStates.VagrantNovaItem.DetonateState.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                {
                    blastAttack.AddModdedDamageType(IgnoreShieldGateDamage);
                    return blastAttack;
                });
            };

            IL.EntityStates.ImpBossMonster.BlinkState.ExitCleanup += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                {
                    blastAttack.AddModdedDamageType(IgnoreShieldGateDamage);
                    return blastAttack;
                });
            };

            Resources.Load<GameObject>("prefabs/projectiles/RoboBallDelayKnockupProjectile").AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(IgnoreShieldGateDamage);

            On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.isAuthority)
                {
                    DamageAPI.AddModdedDamageType(self.weaponAttack, IgnoreShieldGateDamage);
                }
            };

            IL.EntityStates.BrotherMonster.WeaponSlam.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                {
                    blastAttack.AddModdedDamageType(IgnoreShieldGateDamage);
                    return blastAttack;
                });
            };

            Resources.Load<GameObject>("prefabs/projectiles/BrotherUltLineProjectileRotateLeft").AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(IgnoreShieldGateDamage);
            Resources.Load<GameObject>("prefabs/projectiles/BrotherUltLineProjectileRotateRight").AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(IgnoreShieldGateDamage);
        }
    }
}
