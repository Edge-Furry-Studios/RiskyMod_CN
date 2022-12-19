﻿using EntityStates.Loader;

namespace RiskyMod.Survivors.Loader
{
    public class BiggerSlamHitbox
    {
        public static bool enabled = true;
        public BiggerSlamHitbox()
        {
            if (!enabled) return;

            //Slam Hitbox
            On.EntityStates.Loader.GroundSlam.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.isAuthority && self.characterMotor)
                {
                    if (!(self.detonateNextFrame || self.characterMotor.Motor.GroundingStatus.IsStableOnGround)
                    && self.fixedAge >= GroundSlam.minimumDuration)
                    {
                        if (SneedUtils.SneedUtils.IsEnemyInSphere(5f, self.characterBody.footPosition, self.GetTeam(), true))
                        {
                            self.detonateNextFrame = true;
                        }
                    }
                }
            };
        }
    }
}
