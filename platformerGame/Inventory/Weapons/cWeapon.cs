using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;
using platformerGame.GameObjects;

using platformerGame.Utilities;

namespace platformerGame.Inventory.Weapons
{
    // TODO: make inherited from Inventory.cItem
    abstract class cWeapon
    {
        protected cGameObject owner;
        protected cRegulator regulator;
        protected int firingFrequency;
        protected float spread; // in radian

        protected int currentAmmo;
        protected int maxAmmo;
        protected int magazineCapacity;
        protected int bulletsPerShot;
        protected float timeToReload;

        protected string bulletBreedID;

        public cWeapon(cGameObject owner, int firing_frequency, string bullet_breed_id)
        {
            this.owner = owner;
            this.firingFrequency = firing_frequency;
            this.regulator = new cRegulator();
            this.regulator.resetByFrequency(firing_frequency);
            this.spread = (float)AppMath.DegressToRadian(2);
            this.bulletsPerShot = 1;
            this.bulletBreedID = bullet_breed_id;
        }

        protected bool isReadForNextShot()
        {
            return regulator.isReady();
        }

        public abstract bool Fire(Vector2f target);
        /*
        {
            if(this.isReadForNextShot())
            {
               Vector2f dir = AppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
               Vector2f toSpreadTarget = AppMath.GetRandomVecBySpread(dir, spread);
               cBullet b = new cBullet(this.owner, owner.Bounds.center, toSpreadTarget);
               owner.Scene.EntityPool.AddBullet(b);
            }
        }
        */

        // in every weapon, the same method used to add a bullet to the scene
        protected void Shot(Vector2f direction)
        {
            cBullet b = new cBullet(this.owner, BulletBreed.GetBreed(this.bulletBreedID), owner.Bounds.center, direction);
            owner.Scene.EntityPool.AddBullet(b);
        }

        protected void decreaseAmmo()
        {
            this.currentAmmo -= bulletsPerShot;
        }

        public int CurrentAmmo
        {
            get { return this.currentAmmo; }
        }

        public int MaxAmmo
        {
            get { return this.maxAmmo; }
        }

        public int FiringFrequency
        {
            get {return firingFrequency; }
            set { firingFrequency = value; }
        }
    }
}
