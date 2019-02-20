using System;

using SFML.System;

using platformerGame.Utilities;
using platformerGame.GameObjects;

namespace platformerGame.Inventory.Weapons
{
    class cShotgun : cWeapon
    {

        public cShotgun(cGameObject owner, int firing_frequency, string bullet_breed_id = "simple-bullet") 
            : base(owner, firing_frequency, bullet_breed_id)
        { 
            this.spread = (float)AppMath.DegressToRadian(2);
            this.bulletsPerShot = 4;
        }

        public override bool Fire(Vector2f target)
        {
            if (this.isReadForNextShot())
            {
                Vector2f dir = AppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
                float dirAngle = (float)AppMath.GetAngleOfVector(dir);
                float unitAngle = spread / bulletsPerShot;
                float tangle = dirAngle - ((bulletsPerShot / 2.0f) * unitAngle);
                for (int i = 0; i < bulletsPerShot; ++i)
                {
                    tangle += i * unitAngle;
                    //Vector2f toSpreadTarget = cAppMath.GetRandomVecBySpread(dir, spread);
                    Vector2f toSpreadTarget = new Vector2f((float)Math.Cos(tangle), (float)Math.Sin(tangle));

                    this.Shot(toSpreadTarget);
                }
                
                AssetManager.playSound("shotgun", 3);

                return true;

            }

            return false;
        }
    }
}
