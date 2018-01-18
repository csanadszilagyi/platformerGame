using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using SFML.System;
using SFML.Graphics;

namespace platformerGame.Particles
{
    class cExplosionController : cBaseParticleController
    {
        double minScale = 0.3;
        double maxScale = 0.6;

        public cExplosionController(cParticleManager manager, int max_particles = 300) : base(manager, max_particles)
        {
            this.renderStates = new RenderStates(BlendMode.Add);
            this.renderStates.Texture = manager.ExplosionTexture;
        }


        protected override void initParticle(Particle particle, Vector2f position)
        {
            particle.Pos = position;
            particle.LastPos = particle.Pos;
            particle.ViewPos = particle.Pos;
            particle.MaxSpeed = cAppMath.GetRandomNumber(200, 400); //700, 900 | (400, 600); //3, 8//Math->GetRandomNumber(510, 800); // 2000.0f

            //float dir = Math->GetRandomClamped();

            //------------------------------------------------------------------------------------------------
            float a = (float)cAppMath.DegressToRadian(cAppMath.GetRandomNumber(0, 360));//sDivisions * m_Angles;

            particle.Vel = new Vector2f((float)Math.Cos(a) * particle.MaxSpeed, (float)Math.Sin(a) * particle.MaxSpeed);
            //------------------------------------------------------------------------------------------------

            //particle.m_Vel = sf::Vector2f(Math->GetRandomClamped() * particle.m_MaxSpeed, Math->GetRandomClamped() *particle.m_MaxSpeed);
            particle.SlowDown = 0.98f; //0.92f;
                                       //particle.m_SlowDown = (float)Math->GetRandomDoubleInRange(0.55, 0.7); //0.6f;
                                       //phs->AddForce( sf::Vector2f(Math->GetRandomClamped() * phs->MaxSpeed, Math->GetRandomClamped() * phs->MaxSpeed) );

            Vector2u uSize = particleManager.ExplosionTexture.Size;

            particle.Scale = (float)cAppMath.GetRandomDoubleInRange(this.minScale, this.maxScale);
            particle.Dims = new Vector2f(uSize.X * particle.Scale, uSize.Y * particle.Scale);

            particle.ScaleSpeed = -cAppMath.GetRandomNumber(10, 50);
            particle.Color = Utils.GetRandomGreenColor();
            particle.Opacity = 255.0f;
            particle.Life = 1.0f;
            particle.Fade = 60; //Math->GetRandomNumber(100, 240);
        }

        public void LittleBlood(Vector2f position)
        {
            minScale = 0.2;
            maxScale = 0.4;
            Particle p;
            int i = 0;
            while ((p = pool.getNew()) != null && i < 3)
            {
                initParticle(p, position);
                ++i;
            }
        }

        public void NormalBlood(Vector2f position)
        {
            minScale = 0.5;
            maxScale = 0.8;
            Particle p;
            int i = 0;
            while ((p = pool.getNew()) != null && i < 10)
            {
               initParticle(p, position);
                ++i;
            }
            
        }

        public override void Update(float step_time)
        {
            cWorld world = particleManager.Scene.World;

            for(int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);
                // Particle's update code comes here.

                if (p.Life <= 0.0f)
                {
                    p.Opacity -= p.Fade * step_time;

                    if (p.Opacity <= 0.0f)
                    {
                        p.Opacity = 0.0f;

                        pool.deactivate(i);
                        
                    }
                }
                else
                {
                    world.collideParticleRayTrace(p, step_time);



                    /*
                            p.Dims.Y += p.ScaleSpeed * step_time; //+=
                            p.Dims.y += p.ScaleSpeed * step_time; //+=
                    */
                    cAppMath.Vec2Truncate(ref p.Vel, p.MaxSpeed * 1.5f);


                    p.Vel.Y += (Constants.GRAVITY * 40.0f * (step_time * step_time));

                    p.Vel.X *= p.SlowDown;
                    p.Vel.Y *= p.SlowDown;

                    p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);
                    p.LastPos = p.Pos;
                    p.Pos.X += p.Vel.X * step_time;
                    p.Pos.Y += p.Vel.Y * step_time;


                    /*
                    if (!cAppMath.Vec2IsZero(p.Vel))
                    {
                        p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);
                    }

                    p.Rotation = (float)cAppMath.GetAngleOfVector(p.Heading);
                   */

                }

                p.Color.A = (byte)p.Opacity;
            }
        }

        public override void BuildVertexBuffer(float alpha)
        {
            uint multiplier = 4;
            uint vNum = ((uint)pool.CountActive * multiplier) + 1;

            vertices.Clear();
            vertices.Resize(vNum);

            float division = 2.0f;

            Vector2u uSize = particleManager.ExplosionTexture.Size;
            float tsizeX = uSize.X;
            float tsizeY = uSize.Y;

            Vector2f newDims = new Vector2f();

            for (int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);
                p.ViewPos = cAppMath.Interpolate(p.LastPos, p.Pos, alpha);

                uint vertexIndex = (uint)i * multiplier;

                newDims.X = p.Dims.X / division;
                newDims.Y = p.Dims.Y / division;

                // Top-left
                Vertex v0 = new Vertex(
                                    new Vector2f(p.ViewPos.X - newDims.X,
                                                 p.ViewPos.Y - newDims.Y),
                                    p.Color,
                                    new Vector2f(0.0f, 0.0f)
                                    );

                // Top-right
                Vertex v1 = new Vertex(
                                   new Vector2f(p.ViewPos.X + newDims.X,
                                                p.ViewPos.Y - newDims.Y),
                                   p.Color,
                                   new Vector2f(tsizeX, 0.0f)
                                   );

                // Bottom-right
                Vertex v2 = new Vertex(
                                   new Vector2f(p.ViewPos.X + newDims.X,
                                                p.ViewPos.Y + newDims.Y),
                                   p.Color,
                                   new Vector2f(tsizeX, tsizeY)
                                   );

                //Bottom-left
                Vertex v3 = new Vertex(
                                   new Vector2f(p.ViewPos.X - newDims.X,
                                                p.ViewPos.Y + newDims.Y),
                                   p.Color,
                                   new Vector2f(0.0f, tsizeY)
                                   );

                vertices[vertexIndex + 0] = v0;
                vertices[vertexIndex + 1] = v1;
                vertices[vertexIndex + 2] = v2;
                vertices[vertexIndex + 3] = v3;

            }
        }

        public override void Render(RenderTarget destination, float alpha)
        {

            destination.Draw(vertices, renderStates);
            /*
            for (int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);

                p.ViewPos = cAppMath.Interpolate(p.LastPos, p.Pos, alpha);

                cRenderFunctions.DrawTexture(destination, p.ViewPos, particleManager.ExplosionTexture, new IntRect(), p.Color, p.Rotation, p.Scale, false, false, BlendMode.Add, null);

                //cRenderFunctions.DrawSprite(destination, p.ViewPos, particleManager.ExplosionTexture, new IntRect(), p.Color, p.Rotation, new Vector2f(p.Scale, p.Scale), BlendMode.Add, null);
            }
            */
        }

        public int NumActive
        {
            get { return pool.CountActive; }
        }
    }
}
