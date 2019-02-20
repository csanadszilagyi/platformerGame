using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using SFML.System;
using SFML.Graphics;

using platformerGame.Utilities;

namespace platformerGame.Particles
{
    class cExplosionController : cBaseParticleController
    {
        double minScale = 0.3;
        double maxScale = 0.6;

        int minSpeed = 200;
        int maxSpeed = 400;

        public cExplosionController(cParticleManager manager, uint max_particles = 300) : base(manager, max_particles)
        {
            this.renderStates.Texture = manager.ExplosionTexture;
        }


        protected override void initParticle(EmissionInfo emission)
        {
            Particle particle = emission.Particle;

            particle.Pos = emission.StartPosition;
            particle.LastPos = particle.Pos;
            particle.ViewPos = particle.Pos;
            particle.MaxSpeed = AppMath.GetRandomNumber(minSpeed, maxSpeed); //700, 900 | (400, 600); //3, 8//Math->GetRandomNumber(510, 800); // 2000.0f

            if( !AppMath.Vec2IsZero(emission.EmitDirection))
            {
                Vector2f particleDirection = AppMath.GetRandomVecBySpread(emission.EmitDirection, AppMath.HALF_PI);
                particle.Vel = new Vector2f(particleDirection.X * particle.MaxSpeed, particleDirection.Y * particle.MaxSpeed);
            }
            else
            {
                // float angle = (float)cAppMath.DegressToRadian(cAppMath.GetRandomNumber(0, 360));//sDivisions * m_Angles;

                particle.Vel = AppMath.GetRandomUnitVec() * particle.MaxSpeed;
                // particle.Vel = new Vector2f((float)Math.Cos(angle) * particle.MaxSpeed, (float)Math.Sin(angle) * particle.MaxSpeed);
            }

            //float dir = Math->GetRandomClamped();
           


            //particle.m_Vel = sf::Vector2f(Math->GetRandomClamped() * particle.m_MaxSpeed, Math->GetRandomClamped() *particle.m_MaxSpeed);
            particle.SlowDown = 0.98f; //0.92f;
                                       //particle.m_SlowDown = (float)Math->GetRandomDoubleInRange(0.55, 0.7); //0.6f;
                                       //phs->AddForce( sf::Vector2f(Math->GetRandomClamped() * phs->MaxSpeed, Math->GetRandomClamped() * phs->MaxSpeed) );

            Vector2u uSize = this.renderStates.Texture.Size;

            particle.Scale = (float)AppMath.GetRandomDoubleInRange(this.minScale, this.maxScale);
            particle.Dims = new Vector2f(uSize.X * particle.Scale, uSize.Y * particle.Scale);

            particle.ScaleSpeed = -AppMath.GetRandomNumber(10, 50);
            particle.Color = Utils.GetRandomGreenColor();
            particle.Opacity = 255.0f;
            particle.Life = 1.0f;
            particle.Fade = 80; // 90; //Math->GetRandomNumber(100, 240);

            particle.Intersects = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction">Normalized vector of the emission direction</param>
        public void LittleBlood(EmissionInfo emission)
        {
            minScale = 0.2;
            maxScale = 0.4;
            minSpeed = 200;
            maxSpeed = 400;
            loopAddition(emission, 3);
        }

        public void NormalBlood(EmissionInfo emission)
        {
            minScale = 0.5; // 0.5
            maxScale = 0.8; // 0.8
            minSpeed = 200;
            maxSpeed = 400;
            loopAddition(emission, 25);
        }

        public void FastBlood(EmissionInfo emission)
        {
            minScale = 0.4;
            maxScale = 0.7;
            minSpeed = 400;
            maxSpeed = 600;
            loopAddition(emission, 20);
        }

        public override void Update(float step_time)
        {
            cWorld world = particleManager.Scene.World;

            for (int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);
                // Particle's update code comes here.

                    /*
                            p.Dims.Y += p.ScaleSpeed * step_time; //+=
                            p.Dims.y += p.ScaleSpeed * step_time; //+=
                    */
                    


                    p.Vel.Y += (Constants.GRAVITY * 40.0f * (step_time * step_time));

                    p.Vel.X *= p.SlowDown;
                    p.Vel.Y *= p.SlowDown;

                    AppMath.Vec2Truncate(ref p.Vel, p.MaxSpeed * 1.5f);

                    world.collideParticleSAT(p, step_time, false);

                    //p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);
                    p.LastPos = p.Pos;
                    p.Pos.X += p.Vel.X * step_time;
                    p.Pos.Y += p.Vel.Y * step_time;



                    if (!AppMath.Vec2IsZero(p.Vel))
                    {
                        p.Heading = AppMath.Vec2NormalizeReturn(p.Vel);
                        p.Rotation = (float)AppMath.GetAngleOfVector(p.Heading);
                    }

                   

                   // world.collideParticleRayTrace(p, step_time);
                    

               
                
                p.Opacity -= p.Fade * step_time;

                if (p.Opacity <= 0.0f)
                {
                    p.Opacity = 0.0f;

                    pool.deactivate(i);

                }

                p.Color.A = (byte)p.Opacity;
            }
        }

        public override void BuildVertexBuffer(float alpha)
        {
            uint multiplier = 4;
            uint vNum = ((uint)pool.CountActive * multiplier) + 1;

            //vertices.Clear();
            vertices.Resize(vNum);

            float division = 2.0f;

            Vector2u uSize = this.renderStates.Texture.Size;
            float tsizeX = uSize.X;
            float tsizeY = uSize.Y;

            Vector2f newDims = new Vector2f();

            for (uint i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get((int)i);

                newDims.X = p.Dims.X / division;
                newDims.Y = p.Dims.Y / division;

                p.ViewPos = AppMath.Interpolate(p.Pos, p.LastPos, alpha);

                uint vertexIndex = i * multiplier;

                Vector2f v0Pos = new Vector2f(p.ViewPos.X - newDims.X,
                                              p.ViewPos.Y - newDims.Y);

                Vector2f v1Pos = new Vector2f(p.ViewPos.X + newDims.X,
                                              p.ViewPos.Y - newDims.Y);

                Vector2f v2Pos = new Vector2f(p.ViewPos.X + newDims.X,
                                              p.ViewPos.Y + newDims.Y);

                Vector2f v3Pos = new Vector2f(p.ViewPos.X - newDims.X,
                                              p.ViewPos.Y + newDims.Y);


                // Top-left
                Vertex v0 = new Vertex(
                                   v0Pos,
                                    p.Color,
                                    new Vector2f(0.0f, 0.0f)
                                    );

                // Top-right
                Vertex v1 = new Vertex(
                                   v1Pos,
                                   p.Color,
                                   new Vector2f(tsizeX, 0.0f)
                                   );

                // Bottom-right
                Vertex v2 = new Vertex(
                                   v2Pos,
                                   p.Color,
                                   new Vector2f(tsizeX, tsizeY)
                                   );

                //Bottom-left
                Vertex v3 = new Vertex(
                                   v3Pos,
                                   p.Color,
                                   new Vector2f(0.0f, tsizeY)
                                   );

                vertices[vertexIndex + 0] = v0;
                vertices[vertexIndex + 1] = v1;
                vertices[vertexIndex + 2] = v2;
                vertices[vertexIndex + 3] = v3;
            }
        }

        /*
        public void BuildVertexBufferOnlyVisibles(float alpha)
        {
            uint multiplier = 4;
            uint vNum = (pool.CountActive * multiplier) + 1;

            vertices.Clear();
            //vertices.Resize(vNum);

            float division = 2.0f;

            Vector2u uSize = particleManager.ExplosionTexture.Size;
            float tsizeX = uSize.X;
            float tsizeY = uSize.Y;

            Vector2f newDims = new Vector2f();
            cAABB bounds = new cAABB();
            for (uint i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);

                newDims.X = p.Dims.X / division;
                newDims.Y = p.Dims.Y / division;

                
                bounds.SetDims(newDims);
                bounds.SetPosByCenter(p.Pos);

                if(particleManager.Scene.onScreen(bounds))
                {
                    p.ViewPos = cAppMath.Interpolate(p.LastPos, p.Pos, alpha);

                    //uint vertexIndex = (uint)i * multiplier;

                    Vector2f v0Pos = new Vector2f( - newDims.X,
                                                   - newDims.Y);

                    Vector2f v1Pos = new Vector2f(+ newDims.X,
                                                     - newDims.Y);

                    Vector2f v2Pos = new Vector2f( + newDims.X,
                                                 + newDims.Y);

                    Vector2f v3Pos = new Vector2f( - newDims.X,
                                                 + newDims.Y);


                    Transform rot = Transform.Identity;
                    rot.Rotate((float)cAppMath.RadianToDegress(p.Rotation), p.ViewPos);


                    v0Pos = rot.TransformPoint(v0Pos);
                    v1Pos = rot.TransformPoint(v1Pos);
                    v2Pos = rot.TransformPoint(v2Pos);
                    v3Pos = rot.TransformPoint(v3Pos);

                    // Top-left
                    Vertex v0 = new Vertex(
                                       v0Pos,
                                        p.Color,
                                        new Vector2f(0.0f, 0.0f)
                                        );

                    // Top-right
                    Vertex v1 = new Vertex(
                                       v1Pos,
                                       p.Color,
                                       new Vector2f(tsizeX, 0.0f)
                                       );

                    // Bottom-right
                    Vertex v2 = new Vertex(
                                       v2Pos,
                                       p.Color,
                                       new Vector2f(tsizeX, tsizeY)
                                       );

                    //Bottom-left
                    Vertex v3 = new Vertex(
                                       v3Pos,
                                       p.Color,
                                       new Vector2f(0.0f, tsizeY)
                                       );

                   
                    
                    //vertices[vertexIndex + 0] = v0;
                    //vertices[vertexIndex + 1] = v1;
                    //vertices[vertexIndex + 2] = v2;
                    //vertices[vertexIndex + 3] = v3;
                    
                    vertices.Append(v0);
                    vertices.Append(v1);
                    vertices.Append(v2);
                    vertices.Append(v3);
                }
            }
        }
*/
        public override void Render(RenderTarget destination, float alpha)
        {

            destination.Draw(vertices, renderStates);
            /*
            for (uint i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);

                p.ViewPos = cAppMath.Interpolate(p.LastPos, p.Pos, alpha);

                cRenderFunctions.DrawTexture(destination, p.ViewPos, particleManager.ExplosionTexture, new IntRect(), p.Color, p.Rotation, p.Scale, false, false, BlendMode.Add, null);

                //cRenderFunctions.DrawSprite(destination, p.ViewPos, particleManager.ExplosionTexture, new IntRect(), p.Color, p.Rotation, new Vector2f(p.Scale, p.Scale), BlendMode.Add, null);
            }
            */
        }
    }
}
