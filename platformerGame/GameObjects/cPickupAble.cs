using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

using platformerGame.Utilities;

namespace platformerGame.GameObjects
{
    class cPickupAble : cGameObject
    {
        Sprite sprite;
        cSpatialGrid grid;

        bool pickedUp;
        bool pulling;

        const float EMIT_SPEED = 150.0f;
        const int MAX_PULL_DISTANCE = 80;
        const int PULL_FORCE = 10;

        List<cAABB> walls;
        public cPickupAble() : base()
        { }

        public cPickupAble(cGameScene scene, cSpatialGrid grid, Vector2f pos, Vector2f emit_direction) : base(scene, pos)
        {
            this.grid = grid;
            this.pickedUp = pulling = false;
            this.heading = emit_direction;

            this.bounds = new cAABB();
            this.bounds.SetDims(new Vector2f(24.0f, 22.0f));
            this.bounds.SetPosByCenter(pos);
            this.HitCollisionRect = bounds;

            this.MaxSpeed = EMIT_SPEED*2;
            this.mass = 1.0f;

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
            walls = new List<cAABB>();
        }

        public override bool isActive()
        {
            return !pickedUp;
        }

        protected void checkCollisionWithWorld(float step_time)
        {
            cWorld world = Scene.World;

            if (false == pulling)
            {
                // check Collisions with world
                walls.Clear();
                Bounds.SetPosByCenter(position);
                walls = world.getCollidableBlocks(Bounds);
                cGameObject wallObject;
                for(int i = 0;  i < walls.Count;  i++)
                {
                    wallObject = cGameObject.MakeWall(walls[i]);

                    if (cSatCollision.checkAndResolve(this, wallObject, step_time, true))
                        continue;
                }
                
            }
        }

        public override void Update(float step_time)
        {
            checkCollisionWithWorld(step_time);

            force.Y += (false == pulling) ? (Constants.GRAVITY * 40.0f * step_time) : 0.0f;

            velocity.X += force.X * step_time;
            velocity.Y += force.Y * step_time;

            //velocity.X = Math.Abs(velocity.X) < 0.05f ? 0.0f : velocity.X;
            //velocity.Y = Math.Abs(velocity.Y) < 0.05f ? 0.0f : velocity.Y;

            double len = cAppMath.Vec2Length(velocity);
            if (len < 0.1)
            {
                velocity = new Vector2f(0.0f, 0.0f);

            }

            cAppMath.Vec2Truncate(ref velocity, MaxSpeed);

            

            lastPosition = position;
            position.X += velocity.X * step_time;
            position.Y += velocity.Y * step_time;

            Bounds.SetPosByCenter(position);
            this.hitCollisionRect = bounds;

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
            
            /*
            RectangleShape rs = new RectangleShape();
            rs.Size = new Vector2f(16, 16);
            rs.FillColor = new Color(255,0,0,100);
            for(int i = 0; i< walls.Count; i++)
            {
                rs.Position = walls[i].topLeft;
                destination.Draw(rs, new RenderStates(BlendMode.Alpha));
            }
            */
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
                float f = PULL_FORCE * (100000.0f / ( d*d) );
                this.AddForce(toPlayer * f);
            }
            else
            {
                this.pulling = false;
            }
                

            if (cCollision.OverlapAABB(player.Bounds, this.HitCollisionRect))
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
