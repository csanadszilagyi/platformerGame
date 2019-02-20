using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

using platformerGame.Utilities;
using platformerGame.GameObjects.PickupInfo;
using platformerGame.Rendering;
using platformerGame.App;

namespace platformerGame.GameObjects
{
    class cPickupAble : cGameObject
    {

        cPickupInfo pickup;
        
        cBaseRenderer renderer;

        bool pickedUp;
        bool pulling;

        const float EMIT_SPEED = 150.0f;
        const int MAX_PULL_DISTANCE = 80;
        const int PULL_FORCE = 10;

        public cPickupAble() : base()
        { }

        /// <summary>
        /// If no pickup type is unknown (not specified), generate one by weights
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="grid"></param>
        /// <param name="pos"></param>
        /// <param name="emit_direction"></param>
        public cPickupAble(GameScene scene, Vector2f pos, Vector2f emit_direction, PickupType type = PickupType.UNKNOWN) : base(scene, pos)
        {
            pickup = type == PickupType.UNKNOWN ? PickupEffects.getWeighted() : PickupEffects.get(type); 
            this.init(pos, emit_direction);
        }

        private void init(Vector2f pos, Vector2f emit_direction)
        {
            this.pickedUp = pulling = false;
            this.heading = emit_direction;

            this.renderer = pickup.Renderer.DeepCopy();

            this.bounds = new AABB();
            this.bounds.SetDims(this.pickup.HitRectSize);
            this.bounds.SetPosByCenter(pos);
            this.HitCollisionRect = bounds.ShallowCopy();

            this.MaxSpeed = EMIT_SPEED * 2; //*2
            this.mass = 100.0f;

            this.velocity.X = this.heading.X * EMIT_SPEED;
            this.velocity.Y = this.heading.Y * EMIT_SPEED;
            orientation = AppMath.GetAngleOfVector(heading);
        }

        public override bool isActive()
        {
            return (false == pickedUp);
        }

        protected void checkCollisionWithWorld(float step_time)
        {
            cWorld world = Scene.World;

            if (false == pulling)
            {
                pscene.World.collideSAT(this, step_time);

                /*
                // check collisions with world
                List<AABB> wallsPossibleColliding = world.getCollidableBlocks(Bounds);

                // we must check this, because we need to iterate through the possible
                // colliding tiles from other direction according to this condition
                if (velocity.X > 0.0f)
                {
                    for (int i = 0; i < wallsPossibleColliding.Count; ++i)
                    {
                        cGameObject wallObject = cGameObject.MakeWall(wallsPossibleColliding[i]);
                        if (cSatCollision.checkAndResolve(this, wallObject, step_time, true))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    // we have to iterate from "end" to the "start" in order to have the last colliding block below us
                    for (int i = wallsPossibleColliding.Count-1; i >= 0; --i)
                    {
                        cGameObject wallObject = cGameObject.MakeWall(wallsPossibleColliding[i]);
                        if (cSatCollision.checkAndResolve(this, wallObject, step_time, true))
                        {
                            return;
                        }

                    }
                }
                */
            }
        }

        public override void Update(float step_time)
        {
            this.renderer.Update(step_time);

            cPlayer player = Scene.Player;
            if (this.pulling)
            {
                Vector2f predicted = AppMath.Vec2NormalizeReturn((player.Bounds.center + (player.Velocity * step_time)) - this.Bounds.center);
                // pull pickup
                //Vector2f toPlayer = cAppMath.Vec2NormalizeReturn(player.Bounds.center - this.Bounds.center);

                // G * ((m1*m2) / (d*d))
                float f = PULL_FORCE * 150f; // * (100000.0f / ( d*d) );
                this.AddForce(predicted * f);
            }
            /*
            else
            {
                this.pulling = false;
            }
            */

            // applying some gravity
            force.Y += (false == pulling) ? (Constants.GRAVITY * 40.0f * step_time) : 0.0f;

            velocity.X += force.X * step_time;
            velocity.Y += force.Y * step_time;

            //velocity.X = Math.Abs(velocity.X) < 0.05f ? 0.0f : velocity.X;
            //velocity.Y = Math.Abs(velocity.Y) < 0.05f ? 0.0f : velocity.Y;

            double len = AppMath.Vec2Length(velocity);
            if (len < 0.1)
            {
                velocity = new Vector2f(0.0f, 0.0f);

            }

            AppMath.Vec2Truncate(ref velocity, MaxSpeed);

            // get more precise result calling it here (because of the updated velocity)
            // instead of calling at the beginning of this update method
            checkCollisionWithWorld(step_time);

            lastPosition = position;
            position.X += velocity.X * step_time;
            position.Y += velocity.Y * step_time;

            Bounds.SetPosByCenter(position);
            this.hitCollisionRect = bounds;

            if (!AppMath.Vec2IsZero(velocity))
            {
                heading = AppMath.Vec2NormalizeReturn(velocity);
                orientation = AppMath.GetAngleOfVector(heading); 
            }

            force = new Vector2f(0.0f, 0.0f);
        }

        public override void Render(RenderTarget destination)
        {
            //sprite.Position = ViewPosition;
            //sprite.Rotation = (float)cAppMath.RadianToDegress(orientation);
            //destination.Draw(this.sprite, new RenderStates(BlendMode.Alpha));
            
            this.renderer.Draw(destination, ViewPosition);

            /*
            RectangleShape rs = new RectangleShape();
            rs.Size = collidingTile.dims;
            rs.Position = collidingTile.topLeft;
            rs.FillColor = new Color(255, 0, 0, 150);
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

            float d = (float)AppMath.Vec2Distance(player.Bounds.center, this.Bounds.center);

            if (!this.pulling)
            {
                this.pulling = d <= MAX_PULL_DISTANCE;
            }

            if (!pickedUp && cCollision.OverlapAABB(player.Bounds, this.HitCollisionRect))
            {
                player.pickUp(pickup);
                pickedUp = true;
            }
        }
    }
}
