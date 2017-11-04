using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;
using SFML.Graphics;

namespace platformerGame
{
    class cAABB
    {
        public Vector2f topLeft;
        public Vector2f rightBottom;
        public Vector2f center;
        public Vector2f dims;        //full (not half) width and height
        public Vector2f halfDims;

        
        public cAABB()
        {
            topLeft = new Vector2f();
            rightBottom = new Vector2f();
            center = new Vector2f();
            dims = new Vector2f(0, 0);
            halfDims = new Vector2f(0, 0);
        }
        
        public cAABB(float left, float top, float width, float height)
        {
            topLeft = new Vector2f(left, top);
            dims = new Vector2f(width, height);
            halfDims = dims / 2.0f;
            rightBottom = new Vector2f(topLeft.X + dims.X, topLeft.Y + dims.Y);
            center = new Vector2f(topLeft.X + halfDims.X, topLeft.Y + halfDims.Y);
        }

        public cAABB(Vector2f top_left, Vector2f dims)
        {
            topLeft = top_left;
            this.dims = dims;
            halfDims = dims / 2.0f;
            rightBottom = new Vector2f(topLeft.X + dims.X, topLeft.Y + dims.Y);
            center = new Vector2f(topLeft.X + halfDims.X, topLeft.Y + halfDims.Y);
        }

        public void SetDims(Vector2f dims)
        {
            this.dims = dims;
            halfDims.X = dims.X / 2.0f;
            halfDims.Y = dims.Y / 2.0f;
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
            FloatRect result = t.TransformRect(this.AsFloatRect());
            this.SetDims(new Vector2f(result.Width, result.Height));
            this.SetPosByTopLeft( new Vector2f(result.Left, result.Top));
           
        }
        public FloatRect AsFloatRect()
        {
            return new FloatRect(topLeft, dims);
        }

        public cAABB ShallowCopy()
        {
            return (cAABB)this.MemberwiseClone();
        }

    }
}
