using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;
using platformerGame.GameObjects;

namespace platformerGame.Utilities
{
    enum Side
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
        INSIDE
    }

    class cCollision
    {
        public static bool isPointInsideCircle(Vector2f centre, float radius, Vector2f point)
        {
            return AppMath.Vec2DistanceSqrt(point, centre) <= (radius * radius);
        }

        public static bool testCircleVsCirlceOverlap(Vector2f centreA, float radiusA, Vector2f centreB, float radiusB)
        {
            float R = radiusA + radiusB;
            return (AppMath.Vec2DistanceSqrt(centreA, centreB) <= R * R);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A">First gameObject</param>
        /// <param name="B">Second gameObject</param>
        /// <param name="onlyFirst">only seperate the first object</param>
        public static void SeparateEntites(cGameObject A, cGameObject B, bool onlyFirst = false)
        {
            Vector2f to = A.GetCenterPos() - B.GetCenterPos();
            float dist = (float)AppMath.Vec2Length(to);
            float amountOfOverlap = (dist - A.BoundingRadius - B.BoundingRadius); // *0.5f
            Vector2f offset = amountOfOverlap * (to / dist);

            if(onlyFirst)
            {
                A.Position -= offset;
                return;
            }

            offset *= 0.5f;
            A.Position -= offset;
            B.Position += offset;

        }

        public static bool CircleAABBOverlap(Vector2f centre, float radius, AABB rect)
        {
            return (IsPointInsideBox(centre, rect) ||
                    isPointInsideCircle(centre, radius, rect.topLeft) ||
                    isPointInsideCircle(centre, radius, rect.rightBottom) ||
                    isPointInsideCircle(centre, radius, new Vector2f(rect.rightBottom.X, rect.topLeft.Y)) ||
                    isPointInsideCircle(centre, radius, new Vector2f(rect.topLeft.X, rect.rightBottom.Y)));
        }

        public static bool IsPointInsideBox(Vector2f point, AABB box)
        {
                if (point.X <= box.topLeft.X)
                    return false;
                if (point.X >= box.rightBottom.X)
                    return false;
                if (point.Y <= box.topLeft.Y)
                    return false;
                if (point.Y >= box.rightBottom.Y)
                    return false;

                return true;
        }

        public static bool OverlapAABB(AABB A, AABB B)
        {
	        if(A.rightBottom.Y <= B.topLeft.Y)
              {
                  return false; 
              }

	        if(A.topLeft.Y >= B.rightBottom.Y)
              { 
                  return false; 
              }
    
	        if(A.rightBottom.X <= B.topLeft.X) 
              { 
                  return false;
              }

	        if(A.topLeft.X >= B.rightBottom.X)
              { 
                  return false; 
              }

            return true; 
        }

        public static bool lineVsLineCollision(Vector2f LineA1, Vector2f LineA2, Vector2f LineB1, Vector2f LineB2, ref Vector2f intersection)
        {
	              //  The point of the collision, or null if no collision exists.
	                float denom = ((LineB2.Y - LineB1.Y) * (LineA2.X - LineA1.X)) - ((LineB2.X - LineB1.X) * (LineA2.Y - LineA1.Y));
	                if (denom == 0)
		                return false;
	                else
                    {
		                float ua = (((LineB2.X - LineB1.X) * (LineA1.Y - LineB1.Y)) -
			                ((LineB2.Y - LineB1.Y) * (LineA1.X - LineB1.X))) / denom;

		                // The following 3 lines are only necessary if we are checking line
		                //	segments instead of infinite-length lines 

                        float ub = (((LineA2.X - LineA1.X) *(LineA1.Y - LineB1.Y)) -
			                ((LineA2.Y - LineA1.Y) *(LineA1.X - LineB1.X))) / denom;

                        if ((ua < 0) || (ua > 1) || (ub < 0) || (ub > 1))
                        {
                            return false;
                        }

                        intersection = LineA1 + ua * (LineA2 - LineA1);
                        return true;
                    }
        }

        public static bool LineVsLine(Vector2f start_a, Vector2f end_a, Vector2f start_b, Vector2f end_b, out Vector2f intersection)
        {
            intersection = new Vector2f(0.0f, 0.0f);

            Vector2f b = end_a - start_a;
            Vector2f d = end_b - start_b;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2f c = start_b - start_a;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = start_a + t * b;

            return true;
        }

        public static bool testLineVsAABB(Vector2f line_start, Vector2f line_end, AABB box, ref Vector2f intersection)
        {
           Vector2f topLeft = box.topLeft;
           Vector2f topRight = new Vector2f(box.rightBottom.X, box.topLeft.Y);
           Vector2f rightBottom = box.rightBottom;
           Vector2f leftBottom = new Vector2f(box.topLeft.X, box.rightBottom.Y);

            
            // LineVsLine
            return lineVsLineCollision(line_start, line_end, topLeft, topRight, ref intersection) ||
                  lineVsLineCollision(line_start, line_end, topRight, rightBottom, ref intersection) ||
                  lineVsLineCollision(line_start, line_end, rightBottom, leftBottom, ref intersection) ||
                  lineVsLineCollision(line_start, line_end, leftBottom, topLeft, ref intersection);
        }
        public static bool testBulletVsEntity(Vector2f bulPos, Vector2f bulLastPos, AABB entityBounds, ref Vector2f intersection)
        {
            int x0 = (int)bulLastPos.X;
            int y0 = (int)bulLastPos.Y;
            int x1 = (int)bulPos.X;
            int y1 = (int)bulPos.Y;
            bool collision = false;
            Vector2f temp = new Vector2f(0.0f, 0.0f);
            AppMath.Raytrace(x0, y0, x1, y1,
                (x, y) =>
                {
                    collision = IsPointInsideBox(new Vector2f(x, y), entityBounds);
                    temp.X = x;
                    temp.Y = y;
                    return collision;
                }
            );

            intersection = temp;
            return collision;
        }

        public static void resolvePlayerVsBullet(cPlayer player, cBullet bullet, Vector2f intersection)
        {
            bullet.kill();
        }

        public static void resolveMonsterVsBullet(cMonster m, cBullet bullet, Vector2f intersection)
        {
            Vector2f impulse = new Vector2f();
            impulse.X = bullet.Heading.X * Constants.BULLET_HIT_FORCE;
            impulse.Y = bullet.Heading.Y * Constants.BULLET_HIT_FORCE;
            m.AddForce(impulse);
            m.Hit(1, bullet);
            
           // bullet.Position = intersection;
           // bullet.LastPosition = intersection;
            
            bullet.kill();
        }

        public static float pointLineDistance(Vector2f lineA, Vector2f lineB, Vector2f point)
        {
            Vector2f vP = point - lineA;
            Vector2f vAB = lineB - lineA;
            float d = vP.X * vAB.Y - vAB.X * vP.Y;
            return d;
        }
        
        public static Vector2f processWorldObjCollision(Vector2f velocity, Vector2f N)
        {
            Vector2f D = velocity;

            float n = AppMath.Vec2Dot(D, N);

            Vector2f Dn = N * n;
            Vector2f Dt = D - Dn;

            if (n > 0.0f) Dn = new Vector2f(0, 0);

            float dt = AppMath.Vec2Dot(Dt, Dt);
            float CoF = Constants.FRICTION;

            if (dt < Constants.GLUE * Constants.GLUE) CoF = 1.01f;

            //D = -(1.0f + s_fRestitution) * Dn - (CoF) * Dt;

            D.X = -(1.0f + Constants.RESTITUTION) * Dn.X - (CoF) * Dt.X;
            D.Y = -(1.0f + Constants.RESTITUTION) * Dn.Y - (CoF) * Dt.Y;

            float m0 = 1.0f;
            float m1 = 1.0f;
            float m = m0 + m1;
            float r0 = m0 / m; //m
            float r1 = m1 / m; //m

            //if (m0 > 0.0f)
            {
                velocity.X += D.X * r0;
                velocity.Y += D.Y * r0;

            }

            return velocity;
            /*
            if (m1 > 0.0f)
            {
                objB->Velocity.x += D.x * -r1;
                objB->Velocity.y += D.y * -r1;
            }
            */
        }
        
        public static Vector2f getNormalOfClosestSide(Vector2f point, AABB box)
        {
            /*if (IsPointInsideBox(point, box))
                return Side.INSIDE;*/

            Vector2f topLeft = box.topLeft;
            Vector2f topRight = new Vector2f(box.rightBottom.X, box.topLeft.Y);
            Vector2f rightBottom = box.rightBottom;
            Vector2f leftBottom = new Vector2f(box.topLeft.X, box.rightBottom.Y);

            Vector2f ntop = AppMath.Vec2Perp(topRight - topLeft);
            Vector2f nright = AppMath.Vec2Perp(rightBottom - topRight);
            Vector2f nbottom = AppMath.Vec2Perp(leftBottom - rightBottom);
            Vector2f nleft = AppMath.Vec2Perp(topLeft - rightBottom);

            return ntop;
        }
        

    }
}
