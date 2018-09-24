using System;

using SFML.System;

using platformerGame.Utilities;
using platformerGame.GameObjects;

namespace platformerGame.Inventory.Weapons
{
    class cShotgun : cWeapon
    {

        public cShotgun(cGameObject owner, int firing_frequency) : base(owner, firing_frequency)
        { 
            this.spread = (float)cAppMath.DegressToRadian(2);
            this.bulletsPerShot = 4;
        }

        public override void fire(Vector2f target)
        {
            if (this.isReadForNextShot())
            {
                Vector2f dir = cAppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
                float dirAngle = (float)cAppMath.GetAngleOfVector(dir);
                float unitAngle = spread / bulletsPerShot;
                float tangle = dirAngle - ((bulletsPerShot / 2.0f) * unitAngle);
                for (int i = 0; i < bulletsPerShot; ++i)
                {
                    tangle += i * unitAngle;
                    //Vector2f toSpreadTarget = cAppMath.GetRandomVecBySpread(dir, spread);
                    Vector2f toSpreadTarget = new Vector2f((float)Math.Cos(tangle), (float)Math.Sin(tangle));

                    cBullet b = new cBullet(this.owner, owner.Bounds.center, toSpreadTarget);
                    owner.Scene.EntityPool.AddBullet(b);
                }
                
                AssetManager.playSound("shotgun", 3);

            }
        }
    }
}
