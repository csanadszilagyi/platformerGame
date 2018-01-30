using SFML.System;
using SFML.Graphics;

namespace platformerGame
{
    class cGameObject : cQuadTreeOccupant, IDrawable
    {
        protected int m_ID;
        protected static int m_NextValidID;
      
        protected cGameScene m_pScene;

        protected Vector2f position;
        protected Vector2f lastPosition;
        protected Vector2f viewPosition; // interpolációhoz

        protected Vector2f velocity;
        protected Vector2f acceleration;
        protected Vector2f force;

        protected Vector2f facing; // milyen irányba "néz"
        protected Vector2f heading; //milyen irányba halad

        protected Vector2f aabbOffset;
        protected double orientation;
        public float MaxSpeed { get; set; }
        public float SlowDown { get; set; }

        protected Vector2f viewSize;
        //protected cAABB boundingBox;
        protected cAABB mapCollisionRect;

        protected float mass;

        protected bool movAble;

        protected cAABB hitBox;
        public cGameObject() : base()
        {
            m_pScene = null;
            position = new Vector2f(0.0f, 0.0f); ;
            lastPosition = new Vector2f(0.0f, 0.0f); ;
            viewPosition = new Vector2f(0.0f, 0.0f); ;
            velocity = new Vector2f(0.0f, 0.0f);
            acceleration = new Vector2f(0.0f, 0.0f);
            force = new Vector2f(0.0f, 0.0f);
            MaxSpeed = 0.0f;
            SlowDown = 1.0f;
            orientation = 0.0;
            mass = 1.0f;
            movAble = true;
            hitBox = new cAABB();
            m_ID = GetNextValidID();
            
        }
        public cGameObject(cGameScene scene, Vector2f pos) : base()
        {
            m_pScene = scene;
            position = pos;
            lastPosition = pos;
            viewPosition = pos;

            velocity = new Vector2f(0.0f, 0.0f);
            acceleration = new Vector2f(0.0f, 0.0f);
            force = new Vector2f(0.0f, 0.0f);
            orientation = 0.0;
            mass = 1.0f;
            movAble = true;
            hitBox = new cAABB();
            m_ID = GetNextValidID();
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

        public override cAABB Bounds
        {
            get
            {
                return base.Bounds;
            }

            set
            {
                base.Bounds = value;
            }
        }

        
        public cAABB HitBox
        {
            get { return hitBox; }
            set { hitBox = value; }
        }
        

        public cAABB MapCollisionRect
        {
            get { return mapCollisionRect; }
            set { mapCollisionRect = value; } // .ShallowCopy()
        }

        public int ID
        {
            get { return m_ID; }
        }
        public cGameScene Scene
        {
            get { return m_pScene; }
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
            set { this.movAble = true; }
        }

        public bool Unmovable
        {
           get { return !this.movAble; }
        }

        public void AddForce(Vector2f impulse)
        {
            this.force += impulse;
        }

        public void CalculateViewPos(float alpha)
        {
            viewPosition = cAppMath.Interpolate(position, lastPosition, alpha);
        }

        public virtual void Update(float step_time)
        { }

        public virtual void Render(RenderTarget destination)
        { }

        public virtual bool isActive()
        {
            return true;
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

        public static cGameObject fromParticle(Particle p)
        {
            cGameObject go = new cGameObject();
            go.Position = p.Pos;
            go.LastPosition = p.LastPos;
            go.Velocity = p.Vel;
            go.viewPosition = p.ViewPos;
            go.hitBox = new cAABB();
            go.hitBox.SetDims(new Vector2f(1, 1));
            go.hitBox.SetPosByCenter(p.Pos);
            return go;
        }

        public static cGameObject MakeWall(cAABB box)
        {
            cGameObject go = new cGameObject();
            go.Position = box.topLeft;
            go.LastPosition = go.Position;
            go.Velocity = new Vector2f(0.0f, 0.0f);
            go.viewPosition = go.Position;
            //box.SetDims(new Vector2f(Constants.TILE_SIZE, Constants.TILE_SIZE));
            go.bounds = box;
            go.hitBox = box;
            go.MovAble = false; // important!
            go.Mass = 0.0f;
            return go;
        }
    }
}
