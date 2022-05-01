﻿using RoR2;
using UnityEngine;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Items.Boss
{
    public class EmpathyCores
    {
        public static bool enabled = true;
        public static bool ignoreAllyCap = true;

        private static SpawnCard RedBuddyCard = LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallRedBuddy");
        private static SpawnCard GreenBuddyCard = LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallGreenBuddy");

        public EmpathyCores()
        {
            if (!enabled || !ignoreAllyCap) return;

            IL.RoR2.DeployableMinionSpawner.SpawnMinion += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                    );
                c.Emit(OpCodes.Ldarg_0);    //DeployableMinionSpawner (self)
                c.Emit(OpCodes.Ldarg_1);    //SpawnCard
                c.EmitDelegate<Func<DirectorSpawnRequest, DeployableMinionSpawner, SpawnCard, DirectorSpawnRequest>>((directorSpawnRequest, self, spawnCard) =>
                {
                    if (spawnCard == RedBuddyCard || spawnCard == GreenBuddyCard)
                    {
                        directorSpawnRequest.ignoreTeamMemberLimit = EmpathyCores.ignoreAllyCap;
                    }
                    return directorSpawnRequest;
                });
            };
        }
    }
}
