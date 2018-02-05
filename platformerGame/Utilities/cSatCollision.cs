using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace platformerGame.Utilities
{
    /// <summary>
    /// Collision detection based on the Separating Axis Theorem (for fast moving objects, like projectiles).
    /// </summary>
    class cSatCollision
    {
        public static bool checkOnly(cGameObject objA, cGameObject objB, ref float dt, out Vector2f mtd)
        {
            mtd = new Vector2f(0.0f, 0.0f);
            Vector2f RelPos = objA.Bounds.center - objB.Bounds.center;
            Vector2f RelVel = objA.Velocity - objB.Velocity;

            return Collide(objA.HitCollisionRect.getLocalVertices(), 4,
                            objB.HitCollisionRect.getLocalVertices(), 4,
                            RelPos, RelVel,
                            ref mtd, ref dt);
        }

        public static bool checkAndResolve(cGameObject objA, cGameObject objB, float dt, bool resolve = false)
        {
            float t = dt; // 1.0f / 200.0f
            Vector2f N = new Vector2f(0.0f, 0.0f);

            //objA.GetBoundingBox().m_Center - objB.GetBoundingBox().m_Center;

            Vector2f RelPos =  objA.Bounds.center - objB.Bounds.center; //objA.Position - objB.Position;
            Vector2f RelVel = objA.Velocity - objB.Velocity;

            if( Collide(    objA.HitCollisionRect.getLocalVertices(), 4,
                            objB.HitCollisionRect.getLocalVertices(), 4,
                            RelPos, RelVel,
                            ref N, ref t ))
            {
                if(resolve)
                {
                    handleCollision(objA, objB, N, t);
                }

                return true;
            }

            return false;
        }

        public static void handleCollision(cGameObject objA, cGameObject objB, Vector2f N, float t)
        {
            if (t < 0.0f)
            {
                ProcessOverlap(objA, objB, N * (-t));
            }
            else
            {
                ProcessCollision(objA, objB, N, t);
            }
        }

        // two objects overlapped. push them away from each other
        private static void ProcessCollision(cGameObject objA, cGameObject objB, Vector2f N, float t)
        {
            //Vector2f D = m_xDisplacement - xBody.m_xDisplacement;
            Vector2f D = objA.Velocity - objB.Velocity;

            float n = cAppMath.Vec2Dot(D, N);

            Vector2f Dn = N * n;
            Vector2f Dt = D - Dn;

            if (n > 0.0f) Dn = new Vector2f(0, 0);
            
            float dt = cAppMath.Vec2Dot(Dt, Dt);
            float CoF = Constants.FRICTION;
            
            if (dt < Constants.GLUE * Constants.GLUE)
                CoF = 1.01f;
               
            //D = -(1.0f + Constants.RESTITUTION) * Dn - (CoF) * Dt;
            D.X = -(1.0f + Constants.RESTITUTION) * Dn.X - (CoF) * Dt.X;
            D.Y = -(1.0f + Constants.RESTITUTION) * Dn.Y - (CoF) * Dt.Y;
            

            float m0 = objA.Mass;
            float m1 = objB.Mass;
            float m = m0 + m1;
            float r0 = m0 / m; //m
            float r1 = m1 / m; //m

            if (m0 > 0.0f)
            {
                objA.AddVelocity(D * r0);
                /*
                Vector2f velA = objA.Velocity + D * r0;
                objA.Velocity = velA;
                */
                /*
                objA.Velocity.X += D.X * r0;
                objA.Velocity.Y += D.Y * r0;
                */
            }

            if (m1 > 0.0f)
            {
                objB.AddVelocity(D * (-r1));
                /*
                Vector2f velB = objB.Velocity + D * -r1;
                objB.Velocity = velB;
                */
                /*
                objB.Velocity.X += D.X * -r1;
                objB.Velocity.Y += D.Y * -r1;
                */
            }
        }

        // two objects collided at time t. stop them at that time
        private static void ProcessOverlap(cGameObject objA, cGameObject objB, Vector2f xMTD)
        {
            Vector2f mtd = xMTD;

            float time = (1.0f / 200.0f); // 0.0166f; // 1.0f / 120.0f;

            //cAppMath.Vec2Truncate(ref mtd, 0.1f);

            //mtd.X = Math.Clamp<float>(mtd.X, 3.0f, -3.0f);
            //mtd.Y = Math.Clamp<float>(mtd.Y, 3.0f, -3.0f);

            if (objA.Unmovable)
            {
                objB.MoveBy(mtd * -time);
                /*
                Vector2f p = objB.Position;
                p.X -= mtd.X * time; // *0.5f;
                p.Y -= mtd.Y * time; // *0.5f;
                objB.Position = p;
                */
            }
            else
            if (objB.Unmovable)
            {

                objA.MoveBy(mtd * time);

                //objA.Position.X += mtd.X * 0.0166f;// * 0.5f;
                //objA.Position.Y += mtd.Y * 0.0166f;// * 0.5f;
                //return;
            }
            else
            {
                
                objA.MoveBy(mtd * time);
                objB.MoveBy(mtd * -time);
                

                /*
                objA.Position.X += mtd.X * 0.5f;
                objA.Position.Y += mtd.Y * 0.5f;

                objB.Position.X -= mtd.X * 0.5f;
                objB.Position.Y -= mtd.Y * 0.5f;
                */
            }

           
            Vector2f N = mtd;
            cAppMath.Vec2Normalize(ref N);
            ProcessCollision(objA, objB, N, 0.0f);
            
        }


        public static Vector2f[] BuildBox(float width, float height)
        {
            Vector2f[] axVertices = new Vector2f[4];
            axVertices[0] = new Vector2f(-width / 2.0f, -height / 2.0f);
            axVertices[1] = new Vector2f(width / 2.0f, -height / 2.0f);
            axVertices[2] = new Vector2f(width / 2.0f, height / 2.0f);
            axVertices[3] = new Vector2f(-width / 2.0f, height / 2.0f);
            return axVertices;
        }

        private static bool Collide( Vector2f[] A, int Anum,
                                     Vector2f[] B, int Bnum,
                                     Vector2f xOffset,
                                     Vector2f xVel,
                                     ref Vector2f N,
                                     ref float t )
        {
            if (A == null || B == null) return false;

            // All the separation axes
            const int _max = 64; //64
            Vector2f[] xAxis = new Vector2f[_max]; // note : a maximum of (if 64 - 32) 4 vertices per poly is supported
            float[] taxis = new float[_max];
            int iNumAxes = 0;

            xAxis[iNumAxes] = new Vector2f(-xVel.Y, xVel.X);

            float fVel2 = cAppMath.Vec2Dot(xVel, xVel);

            if (fVel2 > 0.000001f)
            {
                if (!IntervalIntersect(A, Anum,
                                        B, Bnum,
                                        xAxis[iNumAxes],
                                        xOffset, xVel,
                                        ref taxis[iNumAxes], t))
                {
                    return false;
                }
                iNumAxes++;
            }

            // test separation axes of A
            for (int j = Anum - 1, i = 0; i < Anum; j = i, i++)
            {
                Vector2f E0 = A[j];
                Vector2f E1 = A[i];
                Vector2f E = E1 - E0;
                xAxis[iNumAxes] = new Vector2f(-E.Y, E.X);

                if (!IntervalIntersect(A, Anum,
                                        B, Bnum,
                                        xAxis[iNumAxes],
                                        xOffset, xVel,
                                        ref taxis[iNumAxes], t))
                {
                    return false;
                }

                iNumAxes++;
            }

            // test separation axes of B
            for (int j = Bnum - 1, i = 0; i < Bnum; j = i, i++)
            {
                Vector2f E0 = B[j];
                Vector2f E1 = B[i];
                Vector2f E = E1 - E0;
                xAxis[iNumAxes] = new Vector2f(-E.Y, E.X);

                if (!IntervalIntersect(A, Anum,
                                        B, Bnum,
                                        xAxis[iNumAxes],
                                        xOffset, xVel,
                                        ref taxis[iNumAxes], t))
                {
                    return false;
                }

                iNumAxes++;
            }

            // special case for segments
            if (Bnum == 2)
            {
                Vector2f E = B[1] - B[0];
                xAxis[iNumAxes] = E;

                if (!IntervalIntersect(A, Anum,
                                        B, Bnum,
                                        xAxis[iNumAxes],
                                        xOffset, xVel,
                                        ref taxis[iNumAxes], t))
                {
                    return false;
                }

                iNumAxes++;
            }

            // special case for segments
            if (Anum == 2)
            {
                Vector2f E = A[1] - A[0];
                xAxis[iNumAxes] = E;

                if (!IntervalIntersect(A, Anum,
                                        B, Bnum,
                                        xAxis[iNumAxes],
                                        xOffset, xVel,
                                        ref taxis[iNumAxes], t))
                {
                    return false;
                }

                iNumAxes++;
            }

            if (!FindMTD(xAxis, taxis, iNumAxes, ref N, ref t))
                return false;

            // make sure the polygons gets pushed away from each other.
            if (cAppMath.Vec2Dot(N, xOffset) < 0.0f)
                N = -N;

            return true;
        }

        // calculate the projection range of a polygon along an axis
        private static void GetInterval(Vector2f[] axVertices, int iNumVertices, Vector2f xAxis, out float min, out float max)
        {
            min = max = cAppMath.Vec2Dot(axVertices[0], xAxis);

            for (int i = 1; i < iNumVertices; i++)
            {
                float d = cAppMath.Vec2Dot(axVertices[i], xAxis);
                if (d < min)
                    min = d;
                else if (d > max)
                    max = d;
            }
        }

        private static bool IntervalIntersect(Vector2f[] A, int Anum,
                               Vector2f[] B, int Bnum,
                               Vector2f xAxis,
                               Vector2f xOffset, Vector2f xVel,
                               ref float taxis, float tmax)
        {
            float min0, max0;
            float min1, max1;
            GetInterval(A, Anum, xAxis, out min0, out max0);
            GetInterval(B, Bnum, xAxis, out min1, out max1);

            float h = cAppMath.Vec2Dot(xOffset, xAxis);
            min0 += h;
            max0 += h;

            float d0 = min0 - max1; // if overlapped, do < 0
            float d1 = min1 - max0; // if overlapped, d1 > 0

            // separated, test dynamic intervals
            if (d0 > 0.0f || d1 > 0.0f)
            {
                float v = cAppMath.Vec2Dot(xVel, xAxis);

                // small velocity, so only the overlap test will be relevant. 
                if (Math.Abs(v) < 0.000001f) //0.0000001f
                    return false;

                float t0 = -d0 / v; // time of impact to d0 reaches 0

                float t1 = d1 / v; // time of impact to d0 reaches 1
                if (t0 > t1)
                {
                    float temp = t0;
                    t0 = t1;
                    t1 = temp;
                }

                taxis = (t0 > 0.0f) ? t0 : t1;

                if (taxis < 0.0f || taxis > tmax)
                    return false;

                return true;
            }
            else
            {
                // overlap. get the interval, as a the smallest of |d0| and |d1|
                // return negative number to mark it as an overlap
                taxis = (d0 > d1) ? d0 : d1;
                return true;
            }
        }

        private static bool FindMTD(Vector2f[] xAxis, float[] taxis, int iNumAxes, ref Vector2f N, ref float t)
        {
            // find collision first
            int mini = -1;
            t = 0.0f;
            for (int i = 0; i < iNumAxes; i++)
            {
                if (taxis[i] > 0)
                {
                    if (taxis[i] > t)
                    {
                        mini = i;
                        t = taxis[i];
                        N = xAxis[i];
                        cAppMath.Vec2Normalize(ref N); //.Normalise();
                    }
                }
            }

            // found one
            if (mini != -1)
                return true;

            // nope, find overlaps
            mini = -1;
            for (int i = 0; i < iNumAxes; i++)
            {
                float n = (float)cAppMath.Vec2Length(cAppMath.Vec2NormalizeReturn(xAxis[i]));   //xAxis[i].Normalise();
                taxis[i] /= n;

                if (taxis[i] > t || mini == -1)
                {
                    mini = i;
                    t = taxis[i];
                    N = xAxis[i];
                }
            }

            if (mini == -1)
                throw new Exception("Collision exception.");

            return (mini != -1);
        }

    }
}
