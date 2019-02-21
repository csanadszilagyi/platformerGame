using SFML.System;
using SFML.Graphics;

using platformerGame.Utilities;
using platformerGame.Containers;
using platformerGame.App;

namespace platformerGame.GameObjects
{
    class cGameObject : GridOccupant, IDrawable
    {
        protected int id;
        protected static int m_NextValidID;
      
        protected GameScene pscene;

        protected Vector2f position;
        protected Vector2f lastPosition;
        protected Vector2f viewPosition; // interpolációhoz

        protected Vector2i gridCoordinate;

        protected Vector2f velocity;
        protected Vector2f acceleration;
        protected Vector2f force;

        protected Vector2f facing; // milyen irányba "néz"
        protected Vector2f heading; //milyen irányba halad

        protected Vector2f aabbOffset;
        protected double orientation;
        public float MaxSpeed { get; set; }
        public float SlowDown { get; set; }

        /// <summary>
        /// For Debug purposes.
        /// </summary>
        public bool Marked { get; set; }

        protected Vector2f viewSize;
        //protected cAABB boundingBox;
        

        protected float mass;

        protected bool movAble;

        protected AABB hitCollisionRect;
        protected float boundingRadius = 16.0f; // for separetion

        public cGameObject() : base()
        {
            pscene = null;
            position = new Vector2f(0.0f, 0.0f);
            lastPosition = new Vector2f(0.0f, 0.0f);
            viewPosition = new Vector2f(0.0f, 0.0f);

            gridCoordinate = new Vector2i(0, 0);

            velocity = new Vector2f(0.0f, 0.0f);
            acceleration = new Vector2f(0.0f, 0.0f);
            force = new Vector2f(0.0f, 0.0f);
            MaxSpeed = 0.0f;
            SlowDown = 1.0f;
            orientation = 0.0;
            mass = 1.0f;
            movAble = true;
            hitCollisionRect = new AABB();

            this.Marked = false;
            id = GetNextValidID();
        }

        public cGameObject(GameScene scene, Vector2f pos) : base()
        {
            pscene = scene;
            position = pos;
            lastPosition = pos;
            viewPosition = pos;

            gridCoordinate = new Vector2i(0, 0);

            velocity = new Vector2f(0.0f, 0.0f);
            acceleration = new Vector2f(0.0f, 0.0f);
            force = new Vector2f(0.0f, 0.0f);
            orientation = 0.0;
            mass = 1.0f;
            movAble = true;
            hitCollisionRect = new AABB();
            this.Marked = false;
            id = GetNextValidID();
        }

        public void Create()
        {
            this.gridCoordinate = EntityPool.calcGridPos(this.Bounds.center);
        }

        protected static int GetNextValidID()
        {
            return m_NextValidID++;
        }

        public Vector2f ViewSize
        {
            get { return viewSize; }
            set { viewSize = value; }
        }
        
        public AABB HitCollisionRect
        {
            get { return hitCollisionRect; }
            set { hitCollisionRect = value; }
        }

        public int ID
        {
            get { return id; }
        }
        public GameScene Scene
        {
            get { return pscene; }
        }

        public Vector2f Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public Vector2f LastPosition
        {
            get
            {
                return lastPosition;
            }

            set
            {
                lastPosition = value;
            }
        }

        public Vector2f ViewPosition
        {
            get
            {
                return viewPosition;
            }

            set
            {
                viewPosition = value;
            }
        }
        
        public Vector2i GridCoordinate
        {
            get { return this.gridCoordinate; }
            set { this.gridCoordinate = value; }
        }

        public Vector2f Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
                velocity = value;
            }
        }

        public Vector2f Acceleration
        {
            get
            {
                return acceleration;
            }

            set
            {
                acceleration = value;
            }
        }

        public Vector2f Force
        {
            get
            {
                return force;
            }

            set
            {
                force = value;
            }
        }

        public Vector2f Heading
        {
            get
            {
                return heading;
            }

            set
            {
                heading = value;
            }
        }

        public Vector2f Facing
        {
            get
            {
                return facing;
            }

            set
            {
                facing = value;
            }
        }

        public Vector2f AabbOffset
        {
            get
            {
                return aabbOffset;
            }

            set
            {
               aabbOffset = value;
            }
        }

        public double Orientation
        {
            get
            {
                return orientation;
            }

            set
            {
                orientation = value;
            }
        }

        public float Mass
        {
            get { return this.mass; }
            set { this.mass = value; }
        }

        public bool MovAble
        {
            get { return this.movAble; }
            set { this.movAble = value; }
        }

        public bool Unmovable
        {
           get { return !this.movAble; }
        }

        public float BoundingRadius
        {
            get { return this.boundingRadius; }
            set { this.boundingRadius = value; }
        }

        public void AddForce(Vector2f impulse)
        {
            this.force += impulse;
        }

        public void CalculateViewPos(float alpha)
        {
            viewPosition = AppMath.Interpolate(position, lastPosition, alpha);
        }

        public virtual void Update(float step_time)
        { }

        public virtual void Render(RenderTarget destination)
        { }

        public virtual bool isActive()
        {
            return true;
        }

        public Vector2f GetCenterViewPos()
        {
            return viewPosition + bounds.halfDims;
        }

        public Vector2f GetCenterPos()
        {
            return position + bounds.halfDims;
        }

        public void MoveBy(Vector2f offset)
        {
            //this.lastPosition = position;
            this.position.X += offset.X;
            this.position.Y += offset.Y;
        }

        public void AddVelocity(Vector2f offset)
        {
            this.velocity.X += offset.X;
            this.velocity.Y += offset.Y;
        }

        public virtual void Kill(cGameObject by)
        { }


        public static cGameObject fromParticle(Particle p)
        {
            cGameObject go = new cGameObject();
            go.Position = p.Pos;
            go.LastPosition = p.LastPos;
            go.Velocity = p.Vel;
            go.viewPosition = p.ViewPos;
            go.Bounds.SetDims(new Vector2f(2.0f, 2.0f));
            go.Bounds.SetPosByCenter(p.Pos);
            go.mass = 1.0f;
            go.movAble = true;
            go.HitCollisionRect = go.Bounds.ShallowCopy();
            return go;
        }

        public static cGameObject MakeWall(AABB box)
        {
            cGameObject go = new cGameObject();
            go.Position = box.center;
            go.LastPosition = go.Position;
            go.Velocity = new Vector2f(0.0f, 0.0f);
            go.ViewPosition = go.Position;
            //box.SetDims(new Vector2f(Constants.TILE_SIZE, Constants.TILE_SIZE));
            go.Bounds = box;
            go.HitCollisionRect = box;
            go.MovAble = false; // important!
            go.Mass = 0.0f;
            return go;
        }
    }
}
