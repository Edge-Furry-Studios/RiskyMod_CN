﻿using UnityEngine;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Legendary
{
    public class HeadHunter
    {
        public static bool enabled = true;
        public static bool perfectedTweak = true;

        public static BuffDef Perfected2;
        public HeadHunter()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "HeadHunter")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Headhunter OnCharacterDeath IL Hook failed");
                }
            };

            AssistManager.HandleAssistInventoryActions += OnKillEffect;
            ModifyFinalDamage.ModifyFinalDamageActions += EliteBonus;

            if (perfectedTweak)
            {
                //Use placeholder Perfected icon so it doesn't force you into shieldonly.
                BuffDef affixLunarDef = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/AffixLunar");

                Perfected2 = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_Perfected2",
                false,
                false,
                false,
                affixLunarDef.buffColor,
                affixLunarDef.iconSprite
                );

                IL.RoR2.HealthComponent.TakeDamage += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if(c.TryGotoNext(
                         x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixLunar")
                        ))
                    {
                        c.Index += 2;
                        c.Emit(OpCodes.Ldloc_1);
                        c.EmitDelegate<Func<bool, CharacterBody, bool>>((flag, attackerBody) =>
                        {
                            return flag || attackerBody.HasBuff(Perfected2.buffIndex);
                        });
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("RiskyMod: Headhunter Perfected IL Hook failed");
                    }
                };
                RecalculateStatsAPI.GetStatCoefficients += HandlePerfected2Stats;
            }
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.HeadHunter);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.HeadHunter);
        }

        private void HandlePerfected2Stats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Perfected2.buffIndex))
            {
                args.baseShieldAdd += sender.maxHealth * 0.2f;
            }
        }

        private static void EliteBonus(DamageMult damageMult, DamageInfo damageInfo,
            HealthComponent victimHealth, CharacterBody victimBody,
            CharacterBody attackerBody, Inventory attackerInventory)
        {
            int hhCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
            if (hhCount > 0)
            {
                if (victimBody.isElite)
                {
                    damageMult.damageMult += 0.3f;
                    if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                    {
                        damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                    }
                }
            }
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            if (victimBody.isElite)
            {
                int hhCount = attackerInventory.GetItemCount(RoR2Content.Items.HeadHunter);
                if (hhCount > 0)
                {
                    float duration = 5f + 5f * hhCount;
                    for (int l = 0; l < BuffCatalog.eliteBuffIndices.Length; l++)
                    {
                        BuffIndex buffIndex = BuffCatalog.eliteBuffIndices[l];
                        if (victimBody.HasBuff(buffIndex))
                        {
                            attackerBody.AddTimedBuff(buffIndex != RoR2Content.Buffs.AffixLunar.buffIndex ? buffIndex : Perfected2.buffIndex, duration);
                        }
                    }
                }
            }
        }
    }
}
