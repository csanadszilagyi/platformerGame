using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace platformerGame.Weapons
{
    class cShotgun : cWeapon
    {
        public cShotgun(cGameObject owner, int firing_frequency) : base(owner, firing_frequency)
        { 
            this.spread = (float)cAppMath.DegressToRadian(4);
        }

        public override void fire(Vector2f target)
        {
            if (this.isReadForNextShot())
            {
                int numShots = 4;
                Vector2f dir = cAppMath.Vec2NormalizeReturn(target - owner.Bounds.center);
                float dirAngle = (float)cAppMath.GetAngleOfVector(dir);
                float unitAngle = spread / numShots;
                float tangle = dirAngle - ((numShots / 2.0f) * unitAngle);
                for (int i = 0; i < numShots; ++i)
                {
                    tangle += i * unitAngle;
                    //Vector2f toSpreadTarget = cAppMath.GetRandomVecBySpread(dir, spread);
                    Vector2f toSpreadTarget = new Vector2f((float)Math.Cos(tangle), (float)Math.Sin(tangle));

                    cBullet b = new cBullet(this.owner, owner.Bounds.center, toSpreadTarget);
                    owner.Scene.EntityPool.AddBullet(b);
                }

            }
        }
    }
}
