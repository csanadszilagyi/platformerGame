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
    class MyCamera
    {

    }

    class Camera
    {
        /// <summary>
        /// Center point of the camera
        /// </summary>
        public Vector2f Target;

        /// <summary>
        /// If we want to move by offset, we can set here
        /// </summary>
        private Vector2f offset;

        /// <summary>
        /// Toggle for smooth camera transition
        /// </summary>
        public bool Smooth = false;

        /// <summary>
        /// Smoothness determines how quickly the transition will take place. Higher smoothness will reach the target position faster.
        /// </summary>
        public float Smoothness = 0.033f;

        /// <summary>
        /// Toggle for automatic position rounding. Useful if pixel sizes become inconsistent or font blurring occurs.
        /// </summary>
        public bool RoundPosition = false;

        internal View View;
        internal Vector2f ActualPosition;
        private Vector2f originalSize;

        /// <summary>
        /// Gets or sets the current zoom level of the camera
        /// </summary>
        public float Zoom
        {
            get { return View.Size.X / originalSize.X; }
            set
            {
                View.Size = originalSize;
                View.Zoom(value);
            }
        }

        /// <summary>
        /// Calculates the area the camera should display
        /// </summary>
        public AABB Bounds
        {
            get { return new AABB(ActualPosition.X - (View.Size.X / 2.0f), ActualPosition.Y - (View.Size.Y / 2.0f), View.Size.X, View.Size.Y); }

        }

        public Camera(FloatRect rect) : this(new View(rect)) { }

        public Camera(View view)
        {
            View = new View(view);
            Target = View.Size / 2.0f;
            originalSize = View.Size;
            ActualPosition = Target;
        }

        public void SetOffset(Vector2f offset)
        {
            this.offset = offset;
        }

        /*
        public void Move(Vector2f offset)
        {
            ActualPosition += offset;
        }
        */

        public static Vector2f nullVec()
        {
            return new Vector2f(0.0f, 0.0f);
        }

        public void Update(Vector2f target, AABB region_bounds, float step_time = Constants.STEP_TIME)
        {
            this.Target = target;
            
            if (Smooth)
            {
                var dir = cAppMath.Vec2NormalizeReturn(Target - ActualPosition);
                float len = (float)cAppMath.Vec2Distance(ActualPosition, Target);
                ActualPosition += dir * (len * Smoothness);
            }
            else
            {
                ActualPosition = Target + ShakeScreen.Offset;
            }

            checkBounds(region_bounds);
        }

        /*
        public void Apply(RenderTarget target)
        {
            var center = ActualPosition;

            if (RoundPosition)
            {
                var pxSize = 1 * Zoom;
                center.X = cAppMath.RoundToNearest(ActualPosition.X, pxSize);
                center.Y = cAppMath.RoundToNearest(ActualPosition.Y, pxSize);
            }

            // offset fixes texture coord rounding
            var offset = 0.25f * Zoom;
            center.X += offset;
            center.Y += offset;

            View.Center = center;
            target.SetView(View);
        }
        */

        public void checkBounds(AABB region_bounds)
        {
            var cameraBounds = this.Bounds;
            /*
            if (RoundPosition)
            {
                var pxSize = 1 * Zoom;
                center.X = cAppMath.RoundToNearest(ActualPosition.X, pxSize);
                center.Y = cAppMath.RoundToNearest(ActualPosition.Y, pxSize);
            }
            */


            if (cameraBounds.topLeft.X < region_bounds.topLeft.X)
                ActualPosition = new Vector2f(region_bounds.topLeft.X + cameraBounds.halfDims.X, ActualPosition.Y);
            else
            if (cameraBounds.rightBottom.X > region_bounds.rightBottom.X)
                ActualPosition = new Vector2f(region_bounds.rightBottom.X - cameraBounds.halfDims.X, ActualPosition.Y);


            if (cameraBounds.topLeft.Y < region_bounds.topLeft.Y)
                ActualPosition = new Vector2f(ActualPosition.X, region_bounds.topLeft.Y + cameraBounds.halfDims.Y);
            else
             if (cameraBounds.rightBottom.Y > region_bounds.rightBottom.Y)
                ActualPosition = new Vector2f(ActualPosition.X, region_bounds.rightBottom.Y - cameraBounds.halfDims.Y);

           // cameraBounds.SetPosByCenter(ActualPosition);
        }

        public void DeployOn(RenderTarget target, AABB region_bounds = null)
        {
            
            /*
            // offset fixes texture coord rounding
            var offset = 0.25f * Zoom;
            center.X += offset;
            center.Y += offset;
            */
            //Target = center;
            //ActualPosition = center;
            this.View.Center = ActualPosition;
            target.SetView(this.View);
        }
    }
}
