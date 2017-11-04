using SFML.System;
using SFML.Graphics;

namespace platformerGame
{
    abstract class cGameObject : cQuadTreeOccupant, IDrawable
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

        /*
        public cAABB BoundingBox
        {
            get{ return boundingBox; }
            set { boundingBox = value; } // .ShallowCopy()
        }
        */

        public cAABB MapCollisionRect
        {
            get { return mapCollisionRect; }
            set { mapCollisionRect = value; } // .ShallowCopy()
        }

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
            m_ID = GetNextValidID();
        }

        protected static int GetNextValidID()
        {
            return m_NextValidID++;
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

        public void AddForce(Vector2f impulse)
        {
            this.force += impulse;
        }

        public void CalculateViewPos(float alpha)
        {
            viewPosition = cAppMath.Interpolate(position, lastPosition, alpha);
        }
        public abstract void Update(float step_time);
        public virtual void Render(RenderTarget destination)
        { }

        public virtual bool isActive()
        {
            return true;
        }
    }
}
