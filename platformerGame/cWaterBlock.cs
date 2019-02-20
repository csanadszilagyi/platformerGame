using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.System;
using SFML.Graphics;

using platformerGame.Utilities;

namespace platformerGame
{
    class cWaterNode
    {
        public Vector2f startPosition;
        public Vector2f position;
        public Vector2f lastPosition;
        public Vector2f velocity;
        public Vector2f acceleration;
        public Vector2f force;

        //private float factor;

        const float MAX_FORCE = 500.0f;
        const float MAX_VELOCITY = 4500.0f;
        float SPEED = 0.0f;
        public cWaterNode(Vector2f pos, float start_Y)
        {
            position = pos;
            lastPosition = position;
            startPosition = new Vector2f(pos.X, start_Y);
            //Random r = new Random();
           /* factor = (float)random.Next(-10, 10);*/
            velocity = new Vector2f(0.0f, 0.0f);
            force = new Vector2f();
            acceleration = new Vector2f();
            
            SPEED = cAppRandom.GetRandomNumber(40, 300);

            
        }

        public void Update(float step_time)
        {
            force.Y = (startPosition.Y - position.Y) * SPEED;
            //cAppMath.Vec2Truncate(ref force, MAX_FORCE);

            acceleration.Y = force.Y * step_time;
            velocity.Y += acceleration.Y * step_time;

            cAppMath.Vec2Truncate(ref velocity, MAX_VELOCITY);

            lastPosition = position;

            position.Y += velocity.Y * step_time; // * factor;

            force.Y = 0.0f;
        }
    }

    class cWaterBlock
    {
        AABB area;
        VertexArray vertices;

        const int MAX_OFFSET_Y = 4; //4
        const int MIN_OFFSET_Y = -3; //-3

        int DIVISION = 6;

        float unitLength;

        RenderStates rs = new RenderStates(BlendMode.Alpha);

        List<cWaterNode> waterNodes;

        public cWaterBlock()
        {

            vertices = new VertexArray(PrimitiveType.TrianglesStrip);

            waterNodes = new List<cWaterNode>();

        }

        public cWaterBlock(AABB area)
        {
            this.area = area.ShallowCopy();

            int divider = Constants.TILE_SIZE; // / 2
            DIVISION = (int)(this.area.dims.X / divider);
            unitLength = divider; // this.area.dims.X / (float)DIVISION;

            vertices = new VertexArray(PrimitiveType.TrianglesStrip);

            waterNodes = new List<cWaterNode>();
            
            Init();
        }

        public void Init()
        {
            vertices.Clear();

            //Color col = new Color(0, 140, 186); //a víz felső színe  new Color(239,28,57);
            //Color col2 = new Color(20, 92, 147);  // alsó színe  new Color(217,37,50);

            //Color col = new Color(239,28,57);
            //Color col2 = new Color(217,37,50);

            Color col = new Color(15,151,219);
            Color col2 = new Color(11,115,163);

            int bottomY = (int)area.rightBottom.Y;

            Vector2f[] points = new Vector2f[DIVISION + 1];


            points[0] = new Vector2f(area.topLeft.X, area.topLeft.Y);
            points[DIVISION] = new Vector2f(area.rightBottom.X, area.topLeft.Y);

            for (int i = 1; i < points.Length-1; i++)
            {
                int add = cAppRandom.GetRandomNumber(MIN_OFFSET_Y, MAX_OFFSET_Y);
                points[i] = new Vector2f(area.topLeft.X + i * unitLength, area.topLeft.Y + add);
            }

            vertices.Append(new Vertex(points[0], col));

            //mindig kettőt adunk hozzá: az alsót és a következő felsőt
            for (int i = 0; i < points.Length-1; i++)
            {
                //bottom
                vertices.Append(new Vertex(new Vector2f(points[i].X, bottomY), col2) );

                //következő
                vertices.Append(new Vertex(points[i+1], col));

                //Itt adunk hozzá egy waternode-ot
                waterNodes.Add(new cWaterNode(points[i + 1], area.topLeft.Y));
            }

            vertices.Append(new Vertex(new Vector2f(area.rightBottom.X, bottomY), col2));
        }
        public void Update(float step_time)
        {
            uint vIndex = 0;
            uint i = 0;

            do
            {
                waterNodes[(int)i].Update(step_time * 1.75f);

                //csak a felső csatlakozó vertexeket akarjuk mozgatni, így azokkal foglalkozunk
                vIndex = 2 + (i * 2);
               
                Vector2f vpoint = vertices[vIndex].Position;
                vpoint.Y = waterNodes[(int)i].position.Y;

                Vertex vert = vertices[vIndex];
                vert.Position = vpoint;
                vertices[vIndex] = vert;

                i++;
                
            }
            while (i < waterNodes.Count-1 && vIndex < vertices.VertexCount);
        }

        public AABB Area
        {
            get { return area; }
        }
        public void Render(RenderTarget destination)
        {
            
            destination.Draw(vertices, rs);
        }
    }
}
