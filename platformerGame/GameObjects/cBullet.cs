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
    class BulletBreed
    {
        public Sprite sprite;
        public float startSpeed;
        public uint textureIntersectionOffset;
        public uint slugLength;

        public static Dictionary<string, BulletBreed> breeds;

        public BulletBreed()
        { }

        // must call somewhere
        public static void Init()
        {
            breeds = new Dictionary<string, BulletBreed>();

            BulletBreed b = new BulletBreed()
            {
                sprite = new Sprite(AssetManager.GetTexture("bullet_light_green")),
                startSpeed = Constants.BULLET_START_SPEED,
                textureIntersectionOffset = 30,
                slugLength = 5
            };

            b.sprite.Scale = new Vector2f(0.5f, 0.5f);
            b.sprite.Origin = new Vector2f(b.sprite.TextureRect.Width - b.textureIntersectionOffset, b.sprite.TextureRect.Height / 2.0f);
            breeds.Add("simple-bullet", b);

            BulletBreed b2 = new BulletBreed()
            {
                sprite = new Sprite(AssetManager.GetTexture("bullet3")),
                startSpeed = 250,
                textureIntersectionOffset = 1,
                slugLength = 1
            };

            b2.sprite.Scale = new Vector2f(0.7f, 0.7f);
            b2.sprite.Origin = new Vector2f(b2.sprite.TextureRect.Width, b2.sprite.TextureRect.Height / 2.0f);

            breeds.Add("turret-bullet", b2);
        }

        public static BulletBreed GetBreed(string id)
        {
            BulletBreed b;
            if(breeds.TryGetValue(id, out b))
            {
                return b;
            }

            return null;
        }
    }

    class cBullet : cGameObject
    {
        const uint SLUG_LENGTH = 5;

        // 30, 15 pixels from the edge of the texture
        const uint TEXTURE_INTERSECTION_OFFSET = 30;

        BulletBreed breed;

        RenderStates blendMode = new RenderStates(BlendMode.Add);

        protected cGameObject owner;
        protected Vector2f oppositeDir;
        protected Vector2f intersection;

        protected Sprite sprite;
        protected bool alive;
        protected float alpha;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="pos">position</param>
        /// <param name="owner">owner gameobject ex. cCharacter</param>
        /// <param name="direction">normalized vector of direction</param>
        public cBullet(cGameObject owner, BulletBreed breed, Vector2f pos, Vector2f direction) : base(owner.Scene, pos)
        {
            this.owner = owner;
            this.breed = breed;

            this.alive = true;
            this.alpha = 255.0f;
            
            this.heading = direction;
            this.bounds = new AABB();
            this.bounds.SetDims(new Vector2f(1.0f, 1.0f));
            this.oppositeDir = new Vector2f(-this.heading.X * breed.slugLength, -this.heading.Y * breed.slugLength);
            this.intersection = new Vector2f(0.0f, 0.0f);
            this.bounds.SetPosByTopLeft(pos);
            this.velocity.X = this.heading.X * breed.startSpeed;
            this.velocity.Y = this.heading.Y * breed.startSpeed;
            orientation = AppMath.GetAngleOfVector(heading);
            
            this.sprite = new Sprite(this.breed.sprite); // bullet_yellow_sm; bullet_light_gree
            this.sprite.Rotation = (float)AppMath.RadianToDegress(this.orientation);
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
            AppMath.Raytrace(posA.X, posA.Y, posB.X, posB.Y, 
                (x, y) =>
                    {

                        collided = world.IsWallAtPos(new Vector2f(x, y)); //world.GetCurrentLevel.IsObstacleAtPos(x, y);

                        intersectionPoint.X = x; // = world.ToWorldPos(new Vector2i(x, y));
                        intersectionPoint.Y = y;
                    

                        return collided;
                    }
            );

            intersectionOut.X = intersectionPoint.X;
            intersectionOut.Y = intersectionPoint.Y;
            return collided;
        }

        public override void Update(float step_time)
        {
            lastPosition.X = position.X;
            lastPosition.Y = position.Y;
            //velocity.Y += (Constants.GRAVITY/2.0f) * step_time;
            position.X += velocity.X * step_time;
            position.Y += velocity.Y * step_time;

          

            this.bounds.SetPosByTopLeft(position);
            this.hitCollisionRect = bounds;

            var world = Scene.World;

            world.collideSAT(this, step_time, false, () => {
                velocity = new Vector2f(0.0f, 0.0f);
                this.Scene.QueueAction(() =>
                {
                    this.Scene.Effects.Place(position, "simple-explosion2");
                });

                this.kill();
            });

            /*
            if (this.checkCollisionWithWorld(owner.Scene.World, ref intersection))
            {
                //this.alive = false; // if not set to false, bullets will be shown
                position.X = intersection.X;
                position.Y = intersection.Y;
                velocity = new Vector2f(0.0f, 0.0f);
                //cAssetManager.playSound("wallhit", 5);

                this.Scene.QueueAction(() =>
                {
                    this.Scene.Effects.Place(intersection, "simple-explosion2");
                });

                this.kill();
            }
            */

            if (!AppMath.Vec2IsZero(velocity))
            {
                heading = AppMath.Vec2NormalizeReturn(velocity);
                //orientation = cAppMath.GetAngleOfVector(heading);
                //Side = Math->Vec2Perp(Heading);
            }
        }

        public void kill()
        {
            this.alive = false;
        }

        public bool Alive
        {
            get { return this.alive; }
        }

        public override bool isActive()
        {
            return this.alive; 
            // && this.alpha > 1.0f;
            //&& cCollision.IsPointInsideBox(this.position, this.owner.Scene.World.WorldBounds);
        }

        public override void Render(RenderTarget destination)
        {
            //this.bounds.SetPosByTopLeft(this.viewPosition);
            this.sprite.Position = this.viewPosition;
            Color c = this.sprite.Color;
            c.A = (byte)this.alpha;
            this.sprite.Color = c;

            //sprite.Rotation = (float)cAppMath.RadianToDegress(orientation);

            destination.Draw(this.sprite, this.blendMode);
            //cRenderFunctions.DrawLine(destination, this.viewPosition, this.viewPosition + this.oppositeDir, 2.0f, new Color(237, 247, 89, 255), BlendMode.Add);
            //cRenderFunctions.DrawRectangleShape(destination, this.Bounds, Color.Green, BlendMode.Add); //cAppMath.RadianToDegress(this.orientation)
        }

    }
}
