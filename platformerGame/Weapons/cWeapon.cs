using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace platformerGame.Weapons
{
    class cWeapon
    {
        protected cGameObject owner;
        protected cRegulator regulator;
        protected int firingFrequency;
        protected float spread; // in radian

        protected int ammo;
        protected int maxAmmo;
        protected int magazineCapacity;
        protected float timeToReload;

        public cWeapon(cGameObject owner, int firing_frequency)
        {
            this.owner = owner;
            this.firingFrequency = firing_frequency;
            this.regulator = new cRegulator();
            this.regulator.resetByFrequency(firing_frequency);
            this.spread = (float)cAppMath.DegressToRadian(2);
        }

        protected bool isReadForNextShot()
        {
            return regulator.isReady();
        }

        public virtual void fire(Vector2f target)
        {
            if(this.isReadForNextShot())
            {
               Vector2f dir = cAppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
               Vector2f toSpreadTarget = cAppMath.GetRandomVecBySpread(dir, spread);
               cBullet b = new cBullet(this.owner, owner.Bounds.center, toSpreadTarget);
               owner.Scene.EntityPool.AddBullet(b);
            }
        }

        public int FiringFrequency
        {
            get {return firingFrequency; }
            set { firingFrequency = value; }
        }
    }
}
