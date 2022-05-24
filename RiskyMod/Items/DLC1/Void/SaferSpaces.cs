﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC1.Void
{
    public class SaferSpaces
    {
        public static bool enabled = true;
        public SaferSpaces()
        {
            //if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchLdfld<HealthComponent>("body"),
                     x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "BearVoidReady"),
                     x => x.MatchCallvirt<CharacterBody>("RemoveBuff")
                    );
                c.Index++;
                c.EmitDelegate<Func<CharacterBody, CharacterBody>>(body =>
                {
                    body.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.1f);
                    return body;
                });
            };
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.BearVoid);
        }
    }
}
