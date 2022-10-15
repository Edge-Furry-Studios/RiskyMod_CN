﻿using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class IncineratorDrone
    {
        public IncineratorDrone()
        {
            //Debug.Log("Dumping Drone flamer");
            //SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Drones/EntityStates.Drone.DroneWeapon.Flamethrower.asset").WaitForCompletion());
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Drones/EntityStates.Drone.DroneWeapon.Flamethrower.asset", "maxDistance", "25");    //12 vanilla
            SkillDef incineratorDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Drones/FlameDroneBodyFlamethrower.asset").WaitForCompletion();
            incineratorDef.baseRechargeInterval = 5f;   //10 vanilla

            CharacterBody body = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/flamedronebody").GetComponent<CharacterBody>();
            body.baseMaxHealth = 340f;
            body.levelMaxHealth = body.baseMaxHealth * 0.3f;    //Gets Overwritten by AllyScaling.cs, need to tidy up code some more later.
            body.baseArmor = 20f;

            body.baseRegen = body.baseMaxHealth / 20f;
            body.levelRegen = body.baseRegen * 0.2f;

            GameObject megaDroneBrokenObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/FlameDroneBroken.prefab").WaitForCompletion();
            PurchaseInteraction pi = megaDroneBrokenObject.GetComponent<PurchaseInteraction>();
            pi.cost = 80;	//Vanilla is 100
        }
    }
}
