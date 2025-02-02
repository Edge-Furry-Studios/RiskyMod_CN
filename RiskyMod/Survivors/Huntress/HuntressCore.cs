﻿using RoR2;
using UnityEngine;
using R2API;
using MonoMod.Cil;
using System;
using EntityStates;
using Mono.Cecil.Cil;
using System.Runtime.CompilerServices;
using EntityStates.RiskyMod.Huntress;
using RoR2.Skills;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RiskyMod.Content;

namespace RiskyMod.Survivors.Huntress
{
    public class HuntressCore
    {
        public static bool enabled = true;

        public static bool strafeChanges = true;
        public static bool flurryChanges = true;

        public static bool laserGlaiveChanges = true;

        public static bool blinkChanges = true;

        public static bool arrowRainChanges = true;
        public static bool ballistaChanges = true;

        public static bool increaseAngle = true;
        public static BullseyeSearch.SortMode HuntressTargetingMode = BullseyeSearch.SortMode.Angle;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/HuntressBody");

        public HuntressCore()
        {
            if (!enabled) return;
            TrackingChanges();
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void TrackingChanges()
        {
            On.RoR2.HuntressTracker.Start += (orig, self) =>
            {
                orig(self);
                if (increaseAngle)
                {
                    self.maxTrackingAngle = 45f;
                }
            };
            IL.RoR2.HuntressTracker.SearchForTarget += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchStfld(typeof(BullseyeSearch), "sortMode")
                    ))
                {
                    c.EmitDelegate<Func<BullseyeSearch.SortMode, BullseyeSearch.SortMode>>(orig =>
                    {
                        return HuntressTargetingMode;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Huntress SearchForTarget IL Hook failed");
                }
            };
        }


        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            HuntressTrackingSkillDef defaultPrimary = Addressables.LoadAssetAsync<HuntressTrackingSkillDef>("RoR2/Base/Huntress/HuntressBodyFireSeekingArrow.asset").WaitForCompletion();
            HuntressTrackingSkillDef altPrimary = Addressables.LoadAssetAsync<HuntressTrackingSkillDef>("RoR2/Base/Huntress/FireFlurrySeekingArrow.asset").WaitForCompletion();
            if (strafeChanges)
            {
                defaultPrimary.skillDescriptionToken = "HUNTRESS_PRIMARY_DESCRIPTION_RISKYMOD";
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.FireSeekingArrow", "orbDamageCoefficient", "2");
            }

            if (flurryChanges)
            {
                altPrimary.skillDescriptionToken = "HUNTRESS_PRIMARY_ALT_DESCRIPTION_RISKYMOD";
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow", "orbDamageCoefficient", "1.2");
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow", "orbProcCoefficient", "1");
            }

            //This fixes Flurry losing arrows at high attack speed.
            On.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.FireOrbArrow += (orig, self) =>
            {
                if (!NetworkServer.active)
                {
                    self.firedArrowCount++;
                }
                orig(self);
            };

            IL.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCall<EntityState>("get_isAuthority")
                    ))
                {
                    c.Index++;
                    c.Emit(OpCodes.Ldarg_0);    //the entitystate
                    c.EmitDelegate<Func<bool, EntityStates.Huntress.HuntressWeapon.FireSeekingArrow, bool>>((flag, self) =>
                    {
                        return flag && self.firedArrowCount >= self.maxArrowCount;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Huntress FireSeekingArrow IL Hook failed");
                }
            };

            On.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.OnExit += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    int remainingArrows = self.maxArrowCount - self.firedArrowCount;
                    for (int i = 0; i < remainingArrows; i++)
                    {
                        self.FireOrbArrow();
                    }
                }
            };
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (!laserGlaiveChanges) return;
            SkillDef glaive = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyGlaive.asset").WaitForCompletion();
            glaive.baseRechargeInterval = 6f;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.ThrowGlaive", "baseDuration", "0.8");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Huntress.HuntressWeapon.ThrowGlaive", "glaiveProcCoefficient", "1");
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (blinkChanges)
            {
                SkillDef blink = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyBlink.asset").WaitForCompletion();
                blink.baseRechargeInterval = 6f;
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (arrowRainChanges)
            {
                SkillDef arrowRain = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyArrowRain.asset").WaitForCompletion();
                arrowRain.baseRechargeInterval = 10f;
                arrowRain.beginSkillCooldownOnSkillEnd = true;
                arrowRain.skillDescriptionToken = "HUNTRESS_SPECIAL_DESCRIPTION_RISKYMOD";
                Skills.ArrowRain = arrowRain;
                new ArrowRainBuff();

                if (SoftDependencies.ScepterPluginLoaded || SoftDependencies.ClassicItemsScepterLoaded)
                {
                    BuildScepterSkillDefs(sk);
                    if (SoftDependencies.ScepterPluginLoaded) SetupScepter();
                    if (SoftDependencies.ClassicItemsScepterLoaded) SetupScepterClassic();
                }
            }

            if (ballistaChanges)
            {
                SkillDef ballista = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/AimArrowSnipe.asset").WaitForCompletion();
                ballista.baseRechargeInterval = 10f;
                ballista.beginSkillCooldownOnSkillEnd = true;
            }
        }

        private void BuildScepterSkillDefs(SkillLocator sk)
        {
            SkillDef orig = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Huntress/HuntressBodyArrowRain.asset").WaitForCompletion();

            ArrowRainBuff.ScepterProjectileSetup();

            Content.Content.entityStates.Add(typeof(BeginArrowRainScepter));
            Content.Content.entityStates.Add(typeof(ArrowRainScepter));

            ArrowRainScepter.muzzleFlashEffect = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Huntress.ArrowRain", "muzzleFlashEffect");
            ArrowRainScepter.areaIndicatorPrefab = (GameObject)SneedUtils.SneedUtils.GetEntityStateFieldObject("EntityStates.Huntress.ArrowRain", "areaIndicatorPrefab");

            SkillDef arrowRainDef = SkillDef.CreateInstance<SkillDef>();
            arrowRainDef.activationState = new SerializableEntityStateType(typeof(BeginArrowRainScepter));
            arrowRainDef.activationStateMachineName = orig.activationStateMachineName;
            arrowRainDef.baseMaxStock = 1;
            arrowRainDef.baseRechargeInterval = 10f;
            arrowRainDef.beginSkillCooldownOnSkillEnd = orig.beginSkillCooldownOnSkillEnd;
            arrowRainDef.canceledFromSprinting = orig.canceledFromSprinting;
            arrowRainDef.dontAllowPastMaxStocks = true;
            arrowRainDef.forceSprintDuringState = false;
            arrowRainDef.fullRestockOnAssign = true;
            arrowRainDef.icon = Assets.ScepterSkillIcons.HuntressArrowRainScepter;
            arrowRainDef.interruptPriority = orig.interruptPriority;
            arrowRainDef.isCombatSkill = true;
            arrowRainDef.keywordTokens = new string[] { };
            arrowRainDef.mustKeyPress = false;
            arrowRainDef.cancelSprintingOnActivation = true;
            arrowRainDef.rechargeStock = 1;
            arrowRainDef.requiredStock = 1;
            arrowRainDef.skillName = "ArrowRainScepter";
            arrowRainDef.skillNameToken = "ANCIENTSCEPTER_HUNTRESS_RAINNAME";
            arrowRainDef.skillDescriptionToken = "HUNTRESS_SPECIAL_SCEPTER_DESCRIPTION_RISKYMOD";
            arrowRainDef.stockToConsume = 1;
            Content.Content.skillDefs.Add(arrowRainDef);

            Skills.ArrowRainScepter = arrowRainDef;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.ArrowRainScepter, "HuntressBody", SkillSlot.Special, 0);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepterClassic()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.ArrowRainScepter, "HuntressBody", SkillSlot.Special, Skills.ArrowRain);
        }
    }
    public static class Skills
    {
        public static SkillDef ArrowRain;
        public static SkillDef ArrowRainScepter;
    }
}
