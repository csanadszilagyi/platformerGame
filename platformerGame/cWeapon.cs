using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace platformerGame
{
    class cWeapon
    {
        cGameObject owner;
        cRegulator regulator;
        int firingFrequency;
        float spread; // in radian
        public cWeapon(cGameObject owner, int firing_frequency)
        {
            this.owner = owner;
            this.firingFrequency = firing_frequency;
            this.regulator = new cRegulator();
            this.regulator.resetByFrequency(firing_frequency);
            this.spread = (float)(cAppMath.PI / 45.0);
        }

        public bool isReadForNextShot()
        {
            return regulator.isReady();
        }

        public void fire(Vector2f target)
        {
            if(this.isReadForNextShot())
            {
                Vector2f dir = cAppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
                Vector2f toTarget = cAppMath.GetRandomVecBySpread(dir, spread);
                cBullet b = new cBullet(this.owner, owner.Bounds.center, toTarget);
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
