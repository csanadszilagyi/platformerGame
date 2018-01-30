using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace platformerGame.GameObjects
{
    class cPickupAble : cGameObject
    {
        Sprite sprite;
        cSpatialGrid grid;

        bool pickedUp;
        bool pulling;

        const float EMIT_SPEED = 150.0f;
        const int MAX_PULL_DISTANCE = 40;

        public cPickupAble() : base()
        { }

        public cPickupAble(cGameScene scene, cSpatialGrid grid, Vector2f pos, Vector2f emit_direction) : base(scene, pos)
        {
            this.grid = grid;
            this.pickedUp = pulling = false;
            this.heading = emit_direction;

            this.bounds = new cAABB();
            this.bounds.SetDims(new Vector2f(24.0f, 24.0f));
            this.bounds.SetPosByCenter(pos);
            this.HitBox = bounds;

            this.MaxSpeed = EMIT_SPEED*2.0f;
            this.mass = 100.0f;
            
            this.velocity.X = this.heading.X * EMIT_SPEED;
            this.velocity.Y = this.heading.Y * EMIT_SPEED;
            orientation = cAppMath.GetAngleOfVector(heading);

            this.sprite = new Sprite(cAssetManager.GetTexture("pickups"));

            int[] tcoords = { 0, 24, 48 };
            int tx = cAppRandom.Chooose<int>(tcoords);
            int ty = cAppRandom.Chooose<int>(tcoords, 2);

            this.sprite.TextureRect = new IntRect(tx, ty, 24, 24);
            //this.sprite.Scale = new Vector2f(0.5f, 0.5f);
            //this.sprite.Rotation = (float)cAppMath.RadianToDegress(this.orientation);

            this.sprite.Origin = new Vector2f(12.0f, 12.0f);
        }

        public override bool isActive()
        {
            return !pickedUp;
        }

        protected void checkCollisionWithWold(float step_time)
        {
            cWorld world = Scene.World;

            if (false == pulling)
            {
                // check Collisions with world
                List<cAABB> walls = world.getCollidableBlocks(this.bounds);
                foreach (var wall in walls)
                {
                    cGameObject wallObject = cGameObject.MakeWall(wall);
                    if (cSatCollision.checkAndResolve(wallObject, this, step_time, true))
                    {
                        break;
                    }
                }
            }
        }

        public override void Update(float step_time)
        {

            checkCollisionWithWold(step_time);


            velocity += force * step_time;
            velocity.Y += (false == pulling) ? (Constants.GRAVITY * 40.0f * (step_time * step_time)) : 0.0f;

            velocity.X = Math.Abs(velocity.X) < 0.05f ? 0.0f : velocity.X;
            velocity.Y = Math.Abs(velocity.Y) < 0.05f ? 0.0f : velocity.Y;

            cAppMath.Vec2Truncate(ref velocity, MaxSpeed);

            lastPosition = position;
            position.X += velocity.X * step_time;
            position.Y += velocity.Y * step_time;

            bounds.SetPosByCenter(position);
            this.hitBox = bounds;

            grid.HandleObject(this.ID, bounds);

            if (!cAppMath.Vec2IsZero(velocity))
            {
                heading = cAppMath.Vec2NormalizeReturn(velocity);
                orientation = cAppMath.GetAngleOfVector(heading);
                
            }

            force = new Vector2f(0.0f, 0.0f);
        }

        public override void Render(RenderTarget destination)
        {
            sprite.Position = ViewPosition;
            //sprite.Rotation = (float)cAppMath.RadianToDegress(orientation);
            destination.Draw(this.sprite, new RenderStates(BlendMode.Alpha));
        }

        public virtual void checkContactWithPlayer()
        {
            cPlayer player = Scene.Player;

            float d = (float)cAppMath.Vec2Distance(player.Bounds.center, this.Bounds.center);
            if (d <= MAX_PULL_DISTANCE)
            {
                //pull pickup
                Vector2f toPlayer = cAppMath.Vec2NormalizeReturn(player.Bounds.center - this.Bounds.center);
                this.pulling = true;
                // G * ((m1*m2) / (d*d))
                float f = 500.0f * (10000.0f / ( d*d) );
                this.AddForce(toPlayer * f);
            }
            else
            {
                this.pulling = false;
            }
                

            if (cCollision.OverlapAABB(player.Bounds, this.HitBox))
            {
                player.pickUp(this);
                pickedUp = true;
            }
        }

        
        public void pickUp()
        {
            pickedUp = true;
        }
    }
}
