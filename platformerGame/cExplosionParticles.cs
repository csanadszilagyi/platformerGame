using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cExplosionParticles : cParticleSystem
    {
        public cExplosionParticles(cGameScene scene, Texture particleTexture) : base(scene, particleTexture)
        {
            this.maxParticles = 20;
        }

        public override Particle CreateParticle()
        {
            //static uint sDivisions = 1;

            Particle particle = new Particle();

            particle.Pos = this.position;
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

            Vector2u uSize = particleTexture.Size;

            particle.Scale = (float)cAppMath.GetRandomDoubleInRange(0.3, 0.6);
            particle.Dims = new Vector2f(uSize.X * particle.Scale, uSize.Y * particle.Scale);

            particle.ScaleSpeed = -cAppMath.GetRandomNumber(10, 50);
            particle.Color = Utils.GetRandomRedColor();
            particle.Opacity = 255.0f;
            particle.Fade = 60; //Math->GetRandomNumber(100, 240);

            return particle;
        }

        public override void Emit(Vector2f pos)
        {
            this.position = pos;
 
            //m_MaxParticles = Math->GetRandomNumber(4, m_MaxParticles);
            //float angles = (float)cAppMath.TWO_PI / this.maxParticles;

            this.clearParticles();

            for (int i = 0; i < this.maxParticles; ++i)
            {
                this.Add(CreateParticle());

                

                //m_Particles.insert(m_Particles.end(), CreateParticle() );
            }
        }

        public override void ReinitParticle(int index)
        {
            this.particles[index] = CreateParticle();
        }

        public override void Update(float step_time)
        {
            cWorld world = this.scene.World;

            foreach (var p in particles.ToArray())
            {
               
                if (p.Opacity <= 0.0f)
                {
                    p.Opacity = 0.0f;
                    this.particles.Remove(p);
                }
                else
                {
                    world.collideParticleRayTrace(p, step_time);

                    p.Opacity -= p.Fade * step_time;
                    /*
                            p.Dims.Y += p.ScaleSpeed * step_time; //+=
                            p.Dims.y += p.ScaleSpeed * step_time; //+=
                    */
                    cAppMath.Vec2Truncate(ref p.Vel, p.MaxSpeed * 1.5f);


                    p.Vel.Y += (Constants.GRAVITY * 40.0f * (step_time * step_time));

                    p.Vel.X *= p.SlowDown;
                    p.Vel.Y *= p.SlowDown;

                    p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);
                    p.LastPos =p.Pos;
                    p.Pos.X += p.Vel.X * step_time;
                    p.Pos.Y += p.Vel.Y * step_time;


                    /*
                    if (!Math->Vec2IsZero(p.Vel))
                    {    
                        p.Heading = Math->Vec2NormalizeReturn(p.Vel);
                    }
                    */

                    p.Color.A = (byte)p.Opacity;
                }
                
            }

            this.active = this.particles.Count > 0;
        }

        public override void Render(RenderTarget target, float alpha)
        {
            //Sprite s = new Sprite(this.particleTexture);
            

            foreach(Particle p in particles)
            {
                p.ViewPos = cAppMath.Interpolate(p.LastPos, p.Pos, alpha);
                cRenderFunctions.DrawSprite(target, p.ViewPos, this.particleTexture, new IntRect(), p.Color, p.Rotation, new Vector2f(p.Scale, p.Scale), BlendMode.Add, null);
                //cRenderFunctions.DrawTexture(target, p.ViewPos, this.particleTexture, new IntRect(), p.Color, 0.0f, p.Scale, false, false, BlendMode.Add, null);

                /*
                s.Position = p.ViewPos;
                s.Color = p.Color;
                s.Rotation = p.Rotation;
                s.Scale = new Vector2f(p.Scale, p.Scale);
                target.Draw(s, this.renderStates);
                */
               
            }
        }
    }
}
