﻿using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class NullifyDebuff
    {
        public static bool enabled = true;
        public NullifyDebuff()
        {
            if (!enabled) return;

            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += (orig, self, buffDef, duration) =>
            {
                orig(self, buffDef, duration);
                if (NetworkServer.active)
                {
                    if (buffDef == RoR2Content.Buffs.NullifyStack && !self.HasBuff(RoR2Content.Buffs.Nullified))
                    {
                        int nullifyCount = self.GetBuffCount(buffDef);
                        if (nullifyCount >= 2)
                        {
                            self.ClearTimedBuffs(buffDef);
                            self.AddTimedBuff(RoR2Content.Buffs.Nullified, 3f);
                        }
                    }
                }
            };
        }
    }
}
