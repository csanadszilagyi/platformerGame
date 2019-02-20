using SFML.System;
using platformerGame.GameObjects;
using platformerGame.Utilities;

namespace platformerGame.Inventory.Weapons
{
    class cMachineGun : cWeapon
    {
        public cMachineGun(cGameObject owner, int firing_frequency) : base(owner, firing_frequency)
        {
            this.maxAmmo = 300;
            this.currentAmmo = maxAmmo;
            this.magazineCapacity = 30;
            this.timeToReload = 1.5f;
            this.spread = (float)cAppMath.DegressToRadian(4);
            this.bulletsPerShot = 1;
        }

        public override void fire(Vector2f target)
        {
            if (this.isReadForNextShot())
            {
                Vector2f dir = cAppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
                Vector2f toSpreadTarget = cAppMath.GetRandomVecBySpread(dir, spread);
                cBullet b = new cBullet(this.owner, owner.Bounds.center, toSpreadTarget);
                owner.Scene.EntityPool.AddBullet(b);

                decreaseAmmo();

                AssetManager.playSound("cg1", 2);
            }
            
        }
    }
}
