using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

using platformerGame.Utilities;

namespace platformerGame
{
    class cLight
    {
        public Vector2f Pos { get; set; } //Center
        public Vector2f Dir { get; set; } //Direction of the Light

        private Color color;
        private Vector3f glColor;
        public Color OriginalColor { get; set; }

        public float Opacity { get; set; }

        public float Radius { get; set; }
        public float SpreadAngle { get; set; }

        //for light autenuation shader
        public float Bleed { get; set; }
        public float LinearizeFactor { get; set; }

        public bool Active { get; set; }

        public cLight()
        {
            Pos = new Vector2f(0, 0);
            Dir = new Vector2f(0, 0);
            Color = new Color(255, 255, 255, 255);
            OriginalColor = color;
            Opacity = 255.0f;
            Radius = 20.0f;
            SpreadAngle = (float)AppMath.TWO_PI;
            Bleed = 1.0f;
            LinearizeFactor = 0.6f;
            Active = true;
        }

        public cLight(Vector2f pos) : this()
        {
            Pos = pos;
        }

        public void TurnOn()
        {
            Color = new Color(OriginalColor);
        }

        public void TurnOff()
        {
            Color = new Color(0, 0, 0, 0);
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                glColor.X = ((float)color.R / 255.0f);
                glColor.Y = ((float)color.G / 255.0f);
                glColor.Z = ((float)color.B / 255.0f);
            }
        }

        public Vector3f GLcolor
        {
            get { return glColor; }
        }

     }
}
