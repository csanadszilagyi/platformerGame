using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;
using SFML.Graphics;

namespace platformerGame
{
    class Particle
    {
        public Vector2f Pos;
        public Vector2f LastPos;
        public Vector2f ViewPos;
        public Vector2f StartPos;

        public Vector2f Dims; //width, height
        public Vector2f Vel; // velocity
        public Vector2f Heading;

        public Color Color;
        public float SlowDown;
        public float MaxSpeed;
        public float ScaleSpeed;
        public float Opacity;
        public float Fade;
        public float Life;
        public float Scale;

        public float Rotation; // in radian

        public bool Intersects;

        public Particle()
        {
            Pos = new Vector2f(0.0f, 0.0f);
            LastPos = new Vector2f(0.0f, 0.0f);
            ViewPos = new Vector2f(0.0f, 0.0f);
            StartPos = new Vector2f(0.0f, 0.0f);
            Dims = new Vector2f(2.0f, 2.0f); // 2x2 square
            Vel = new Vector2f(0.0f, 0.0f);
            Heading = new Vector2f(0.0f, 0.0f);
            Color = new Color(255, 255, 255, 255);
            SlowDown = 0.789f;
            MaxSpeed = 200.0f;
            ScaleSpeed = 0.0f;
            Opacity = 255.0f;
            Fade = 255.0f;
            Life = 1.0f;
            Rotation = 0.0f;
            Scale = 1.0f;
            Intersects = false;
        }

        public AABB getBoundingBox()
        {
            AABB b = new AABB();
            b.SetDims(Dims);
            b.SetPosByCenter(Pos);
            return b;
        }
        
        public Particle(Particle p)
        {
            this.Pos = p.Pos;
            this.LastPos = p.LastPos;
            this.ViewPos = p.ViewPos;
            this.StartPos = p.StartPos;
            this.Dims = p.Dims;
            this.Vel = p.Vel;
            this.Heading = p.Heading;
            this.Color = new Color(p.Color);
            this.SlowDown = p.SlowDown;
            this.MaxSpeed = p.MaxSpeed;
            this.ScaleSpeed = p.ScaleSpeed;
            this.Opacity = p.Opacity;
            this.Fade = p.Fade;
            this.Life = p.Life;
            this.Rotation = p.Rotation;
            this.Scale = p.Scale;
            this.Intersects = p.Intersects;
        }

    }
}
