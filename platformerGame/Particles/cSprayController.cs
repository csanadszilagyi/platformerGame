using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;
using SFML.Graphics;

using platformerGame.Utilities;

namespace platformerGame.Particles
{
    class cSprayController : cBaseParticleController
    {
        double minScale = 0.3;
        double maxScale = 0.6;

        public cSprayController(cParticleManager manager, uint max_particles = 300) : base(manager, max_particles)
        {
            this.renderStates.Texture = manager.SmokeTexture; // smoke
        }

        private void empower(Particle particle)
        {
            particle.Pos = particle.StartPos;
            particle.LastPos = particle.Pos;
            particle.ViewPos = particle.Pos;
            particle.MaxSpeed = cAppMath.GetRandomNumber(30, 50); //200,400 | 700, 900 | (400, 600); //3, 8//Math->GetRandomNumber(510, 800); // 2000.0f

            //------------------------------------------------------------------------------------------------
            float angle = (float)cAppMath.DegressToRadian(cAppMath.GetRandomNumber(0, 360));//sDivisions * m_Angles;
            particle.Rotation = angle;

            // Vector2f dirUnit = cAppMath.GetRandomUnitVec();
            Vector2f dirUnit = cAppMath.GetRandomVecBySpread(new Vector2f(0.0f, -1.0f), cAppMath.DegressToRadian(6));

            particle.Vel = dirUnit * particle.MaxSpeed;
            //------------------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------

            //particle.m_Vel = sf::Vector2f(Math->GetRandomClamped() * particle.m_MaxSpeed, Math->GetRandomClamped() *particle.m_MaxSpeed);
            particle.SlowDown = 1.0f; //0.92f;
                                       //particle.m_SlowDown = (float)Math->GetRandomDoubleInRange(0.55, 0.7); //0.6f;
                                       //phs->AddForce( sf::Vector2f(Math->GetRandomClamped() * phs->MaxSpeed, Math->GetRandomClamped() * phs->MaxSpeed) );

            Vector2u uSize = this.renderStates.Texture.Size;

            particle.Scale = (float)cAppMath.GetRandomDoubleInRange(this.minScale, this.maxScale);

            particle.Dims = new Vector2f(uSize.X * particle.Scale, uSize.Y * particle.Scale);

            particle.ScaleSpeed = (float)cAppMath.GetRandomNumber(15, 25) / 100.0f;

            particle.Color = Utils.GetRandomColor(250, 250, 1, 160, 0, 0); // GetRandomRedColor();
            particle.Opacity = 255.0f;
            particle.Life = 15.0f;
            particle.Fade = cAppRandom.GetRandomNumber(10, 40);

            particle.Intersects = false;
        }

        private void reinit(Particle p)
        {
            // Life hack..
            float prevLife = p.Life;
            this.empower(p);
            p.Life = prevLife;
        }

        protected override void initParticle(EmissionInfo emission)
        {
            Particle particle = emission.Particle;
            particle.StartPos = emission.StartPosition;
            this.empower(particle);
        }

        public void Emit(EmissionInfo emission)
        {
            minScale = 0.1;
            maxScale = 0.35;
            
            loopAddition(emission, 40);
        }

        public override void Update(float step_time)
        {
            cWorld world = particleManager.Scene.World;

            for (int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);

                // p.Vel.Y += (Constants.GRAVITY*30 * (step_time * step_time));

                p.Vel.X *= p.SlowDown;
                p.Vel.Y *= p.SlowDown;

                cAppMath.Vec2Truncate(ref p.Vel, p.MaxSpeed);

                //world.collideParticleSAT(p, step_time);

                //p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);

                Vector2u uSize = this.renderStates.Texture.Size;
                p.Scale -= p.ScaleSpeed * step_time;
                p.Dims = new Vector2f(uSize.X * p.Scale, uSize.Y * p.Scale);

                p.LastPos = p.Pos;
                p.Pos.X += p.Vel.X * step_time;
                p.Pos.Y += p.Vel.Y * step_time;

                if (!cAppMath.Vec2IsZero(p.Vel))
                {
                    p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);
                    // p.Rotation = (float)cAppMath.GetAngleOfVector(p.Heading);
                }

                world.collideParticleRayTrace(p, step_time, true, false);             

                p.Opacity -= p.Fade * step_time;

                if (p.Opacity <= 0.0f || p.Scale <= 0.0f)
                {
                    p.Opacity = 0.0f;
                    p.Scale = 0.0f;
                    p.Life -= 1.0f;

                    if (p.Life <= 0.0f)
                        pool.deactivate(i);
                    else
                        this.reinit(p);
                    

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
            float tSizeX = uSize.X;
            float tSizeY = uSize.Y;

            Vector2f halfDims = new Vector2f();

            for (int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);

                halfDims.X = p.Dims.X / division;
                halfDims.Y = p.Dims.Y / division;

                p.ViewPos = cAppMath.Interpolate(p.LastPos, p.Pos, alpha);

                uint vertexIndex = (uint)i * multiplier;

                Vector2f v0Pos = new Vector2f(p.ViewPos.X - halfDims.X,
                                              p.ViewPos.Y - halfDims.Y);

                Vector2f v1Pos = new Vector2f(p.ViewPos.X + halfDims.X,
                                              p.ViewPos.Y - halfDims.Y);

                Vector2f v2Pos = new Vector2f(p.ViewPos.X + halfDims.X,
                                              p.ViewPos.Y + halfDims.Y);

                Vector2f v3Pos = new Vector2f(p.ViewPos.X - halfDims.X,
                                              p.ViewPos.Y + halfDims.Y);


                // if want rotation included:
                Transform rot = Transform.Identity;
                rot.Rotate((float)cAppMath.RadianToDegress(p.Rotation), p.ViewPos);
               
                v0Pos = rot.TransformPoint(v0Pos);
                v1Pos = rot.TransformPoint(v1Pos);
                v2Pos = rot.TransformPoint(v2Pos);
                v3Pos = rot.TransformPoint(v3Pos);

                // END of rotation code

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
                                   new Vector2f(tSizeX, 0.0f)
                                   );

                // Bottom-right
                Vertex v2 = new Vertex(
                                   v2Pos,
                                   p.Color,
                                   new Vector2f(tSizeX, tSizeY)
                                   );

                //Bottom-left
                Vertex v3 = new Vertex(
                                   v3Pos,
                                   p.Color,
                                   new Vector2f(0.0f, tSizeY)
                                   );

                vertices[vertexIndex + 0] = v0;
                vertices[vertexIndex + 1] = v1;
                vertices[vertexIndex + 2] = v2;
                vertices[vertexIndex + 3] = v3;
            }
        }

        public override void Render(RenderTarget destination, float alpha)
        {
            if (this.pool.CountActive > 0)
                DrawingBase.DrawTextureSimple(destination, this.pool.get(0).StartPos, this.renderStates.Texture, new MyIntRect(), Color.Yellow, BlendMode.Alpha);
            destination.Draw(vertices, renderStates);
        }
    }
}
