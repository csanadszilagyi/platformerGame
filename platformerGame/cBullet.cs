using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cBullet : cGameObject
    {
        const uint SLUG_LENGTH = 5;
        const uint TEXTURE_INTERSECTION_OFFSET = 23; //30 // 15; // pixel from the edge of the texture

        private const float START_SPEED = 2000.0f; // 900

        cGameObject owner;
        Vector2f oppositeDir;
        Vector2f intersection;

        Sprite sprite;
        bool alive;
        bool intersects;
        float alpha;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="pos">position</param>
        /// <param name="owner">owner gameobject ex. cCharacter</param>
        /// <param name="direction">normalized vector of direction</param>
        public cBullet(cGameObject owner, Vector2f pos, Vector2f direction) : base(owner.Scene, pos)
        {
            this.alive = true;
            this.intersects = false;
            this.alpha = 255.0f;
            this.owner = owner;
            this.heading = direction;
            this.bounds = new cAABB();
            this.bounds.SetDims(new Vector2f(1.0f, 1.0f));
            this.oppositeDir = new Vector2f(-this.heading.X*SLUG_LENGTH, -this.heading.Y*SLUG_LENGTH);
            this.intersection = new Vector2f(0.0f, 0.0f);
            this.bounds.SetPosByTopLeft(pos);
            this.velocity.X = this.heading.X * START_SPEED;
            this.velocity.Y = this.heading.Y * START_SPEED;
            orientation = cAppMath.GetAngleOfVector(heading);

            this.sprite = new Sprite(cAssetManager.GetTexture("bullet3")); // bullet_yellow_sm; bullet_light_gree
            this.sprite.Scale = new Vector2f(0.5f, 0.5f);
            this.sprite.Rotation = (float)cAppMath.RadianToDegress(this.orientation);
            this.sprite.Origin = new Vector2f(this.sprite.TextureRect.Width - TEXTURE_INTERSECTION_OFFSET, this.sprite.TextureRect.Height/2.0f);
        }

        public cGameObject Owner
        {
            get { return owner; }
        }

        public bool checkCollisionWithWorld(cWorld world, ref Vector2f intersectionOut)
        {
            Vector2i posA = new Vector2i((int)this.lastPosition.X, (int)this.lastPosition.Y); //world.ToMapPos(this.lastPosition); 
            Vector2i posB = new Vector2i((int)this.position.X, (int)this.position.Y); // world.ToMapPos(this.Position);
            bool collided = false;
            Vector2f intersectionPoint = new Vector2f(0.0f, 0.0f);
            cAppMath.Raytrace(posA.X, posA.Y, posB.X, posB.Y, new VisitMethod(
                (int x, int y) =>
                {

                    collided = world.IsObastacleAtPos(new Vector2f(x, y)); //world.GetCurrentLevel().IsObstacleAtPos(x, y);

                    intersectionPoint.X = x; // = world.ToWorldPos(new Vector2i(x, y));
                    intersectionPoint.Y = y;
                    

                    return collided;
                }
              )
            );
            intersectionOut.X = intersectionPoint.X;
            intersectionOut.Y = intersectionPoint.Y;
            return collided;
        }
        public override void Update(float step_time)
        {
            lastPosition.X = position.X;
            lastPosition.Y = position.Y;

            if (!this.intersects)
            {
                
                position.X += velocity.X * step_time;
                position.Y += velocity.Y * step_time;

                if (this.checkCollisionWithWorld(owner.Scene.World, ref intersection))
                {
                    //this.alive = false; // if not set to false, bullets will be shown
                    position.X = intersection.X;
                    position.Y = intersection.Y;
                    velocity = new Vector2f(0.0f, 0.0f);
                    this.intersects = true;
                }

                if (cAppMath.Vec2IsZero(velocity))
                {
                    cAppMath.Vec2NormalizeReturn(velocity);

                    //Side = Math->Vec2Perp(Heading);
                }
            }
            else
            {
                this.alpha -= 120.0f * step_time;
            }

            
        }

        public void kill()
        {
            this.alive = false;
        }
        public bool isActive()
        {
            return this.alive && this.alpha > 1.0f; //&& cCollision.IsPointInsideBox(this.position, this.owner.Scene.World.WorldBounds);
        }
        public override void Render(RenderTarget destination)
        {
            //this.bounds.SetPosByTopLeft(this.viewPosition);
            this.sprite.Position = this.viewPosition;
            Color c = this.sprite.Color;
            c.A = (byte)this.alpha;
            this.sprite.Color = c;

            destination.Draw(this.sprite, new RenderStates(BlendMode.Add));
            //cRenderFunctions.DrawLine(destination, this.viewPosition, this.viewPosition + this.oppositeDir, 2.0f, new Color(237, 247, 89, 255), BlendMode.Add);
            //cRenderFunctions.DrawRectangleShape(destination, this.bounds, Color.Green, BlendMode.Alpha, cAppMath.RadianToDegress(this.orientation));
        }

    }
}
