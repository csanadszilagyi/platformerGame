using SFML.System;
using platformerGame.GameObjects;
using platformerGame.Utilities;
using platformerGame.Particles;

namespace platformerGame.Inventory.Weapons
{
    class cMachineGun : cWeapon
    {

        public cMachineGun(cGameObject owner, int firing_frequency, string bullet_breed_id = "simple-bullet")
            : base(owner, firing_frequency, bullet_breed_id)
        {
            this.maxAmmo = 300;
            this.currentAmmo = maxAmmo;
            this.magazineCapacity = 30;
            this.timeToReload = 1.5f;
            this.spread = (float)AppMath.DegressToRadian(4);
            this.bulletsPerShot = 1;
        }

        public override bool Fire(Vector2f target)
        {
            if (this.isReadForNextShot())
            {
                Vector2f dir = AppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
                Vector2f toSpreadTarget = AppMath.GetRandomVecBySpread(dir, spread);
                
                
                this.Shot(toSpreadTarget);
                decreaseAmmo();
                

                owner.Scene.Assets.PlaySound("cg1", 2);

                /*
                this.owner.Scene.QueueAction(() =>
                {
                    var e = this.owner.Scene.ParticleManager["explosions"] as cExplosionController;
                    e.Line(new EmissionInfo(this.owner.Bounds.center, toSpreadTarget));
                
                });
                */
            }

            return false;
            
        }
    }
}
