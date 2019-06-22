using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using platformerGame.Utilities;

namespace platformerGame
{
    class AABB
    {
        public Vector2f topLeft;
        public Vector2f rightBottom;
        public Vector2f center;
        public Vector2f dims;           // full (not half) width and height
        public Vector2f halfDims;       // half width and height

        
        public AABB()
        {
            topLeft = new Vector2f();
            rightBottom = new Vector2f();
            center = new Vector2f();
            dims = new Vector2f(0, 0);
            halfDims = new Vector2f(0, 0);
        }
        
        public AABB(float left, float top, float width, float height)
        {
            topLeft = new Vector2f(left, top);
            dims = new Vector2f(width, height);
            halfDims = dims / 2.0f;
            rightBottom = new Vector2f(topLeft.X + dims.X, topLeft.Y + dims.Y);
            center = new Vector2f(topLeft.X + halfDims.X, topLeft.Y + halfDims.Y);
        }

        public AABB(FloatRect rect) : this(rect.Left, rect.Top, rect.Width, rect.Height) { }

        public AABB(Vector2f top_left, Vector2f dims)
        {
            topLeft = top_left;
            this.dims = dims;
            halfDims = dims / 2.0f;
            rightBottom = new Vector2f(topLeft.X + dims.X, topLeft.Y + dims.Y);
            center = new Vector2f(topLeft.X + halfDims.X, topLeft.Y + halfDims.Y);
        }

        public AABB(AABB other)
        {
            SetDims(other.dims);
            SetPosByTopLeft(other.topLeft);
        }

        public void SetDims(Vector2f dims)
        {
            this.dims = dims;
            halfDims.X = dims.X / 2.0f;
            halfDims.Y = dims.Y / 2.0f;
        }

        public void SetPosByRightBottom(Vector2f new_right_bottom)
        {
            this.rightBottom = new_right_bottom;

            this.topLeft.X = new_right_bottom.X - dims.X;
            this.topLeft.Y = new_right_bottom.Y - dims.Y;

            this.center.X = new_right_bottom.X - halfDims.X;
            this.center.Y = new_right_bottom.Y - halfDims.Y;
        }

        public void SetPosByTopLeft(Vector2f top_left)
        {
            topLeft.X = top_left.X;
            topLeft.Y = top_left.Y;

            rightBottom.X = topLeft.X + dims.X;
            rightBottom.Y = topLeft.Y + dims.Y;

            center.X = topLeft.X + halfDims.X;
            center.Y = topLeft.Y + halfDims.Y;
        }

        public void SetPosByCenter(Vector2f center)
        {
            this.center.X = center.X;
            this.center.Y = center.Y;

            topLeft.X = this.center.X - halfDims.X;
            topLeft.Y = this.center.Y - halfDims.Y;

            rightBottom.X = this.center.X + halfDims.X;
            rightBottom.Y = this.center.Y + halfDims.Y;
        }

        public void Scale(Vector2f factors, Vector2f relative_to)
        {
            Transform t = Transform.Identity;
            t.Scale(factors, relative_to);
            FloatRect result = t.TransformRect(this.AsSfmlFloatRect());
            this.SetDims(new Vector2f(result.Width, result.Height));
            this.SetPosByTopLeft( new Vector2f(result.Left, result.Top));
        }

        public MyFloatRect AsMyFloatRect()
        {
            return new MyFloatRect(topLeft.X, topLeft.Y, dims.X, dims.Y);
        }

        public MyIntRect AsMyIntRect()
        {
            return new MyIntRect((int)topLeft.X, (int)topLeft.Y, (int)dims.X, (int)dims.Y);
        }

        public FloatRect AsSfmlFloatRect()
        {
            
            return new FloatRect(topLeft, dims);
        }

        public IntRect AsSfmlIntRect()
        {
            return new IntRect((int)topLeft.X, (int)topLeft.Y, (int)dims.X, (int)dims.Y);
        }

        public AABB ShallowCopy()
        {
            return (AABB)this.MemberwiseClone();
        }

        public AABB DeepCopy()
        {
            return new AABB(this);
        }

        /// <summary>
        /// for fast SA. collision detection (see @cSatCollision.cs)
        /// </summary>
        /// <returns></returns>
        public Vector2f[] getLocalVertices()
        {
           // return Utilities.cSatCollision.BuildBox(dims.X, dims.Y);
            
            Vector2f[] b = new Vector2f[4]
            {
               new Vector2f(-halfDims.X, -halfDims.Y),
               new Vector2f(halfDims.X, -halfDims.Y),
               new Vector2f(halfDims.X, halfDims.Y),
               new Vector2f(-halfDims.X, halfDims.Y)
            };

            return b;
            
        }

        public Vector2f getTopRight()
        {
            return new Vector2f(topLeft.X + dims.X, topLeft.Y);
        }

        public Vector2f getLeftBottom()
        {
            return new Vector2f(topLeft.X, topLeft.Y + dims.Y);
        }

        public float diagonalLength()
        {
            return AppMath.getDiagonal(this.dims);
        }

    }
}
