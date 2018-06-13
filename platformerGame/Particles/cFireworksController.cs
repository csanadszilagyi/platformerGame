﻿using System;
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
    class cFireworksController : cBaseParticleController
    {
        double minScale = 0.3;
        double maxScale = 0.6;

        public cFireworksController(cParticleManager manager, uint max_particles = 300) : base(manager, max_particles)
        {
            this.renderStates.Texture = manager.FireworksTexture;
        }


        protected override void initParticle(cEmissionInfo emission)
        {
            Particle particle = emission.Particle;

            particle.Pos = emission.StartPosition;
            particle.LastPos = particle.Pos;
            particle.ViewPos = particle.Pos;
            particle.MaxSpeed = cAppMath.GetRandomNumber(600, 800); //200,400 | 700, 900 | (400, 600); //3, 8//Math->GetRandomNumber(510, 800); // 2000.0f

            //------------------------------------------------------------------------------------------------
            float angle = (float)cAppMath.DegressToRadian(cAppMath.GetRandomNumber(0, 360));//sDivisions * m_Angles;

            particle.Vel = new Vector2f((float)Math.Cos(angle) * particle.MaxSpeed, (float)Math.Sin(angle) * particle.MaxSpeed);
            //------------------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------

            //particle.m_Vel = sf::Vector2f(Math->GetRandomClamped() * particle.m_MaxSpeed, Math->GetRandomClamped() *particle.m_MaxSpeed);
            particle.SlowDown = 0.98f; //0.92f;
                                       //particle.m_SlowDown = (float)Math->GetRandomDoubleInRange(0.55, 0.7); //0.6f;
                                       //phs->AddForce( sf::Vector2f(Math->GetRandomClamped() * phs->MaxSpeed, Math->GetRandomClamped() * phs->MaxSpeed) );

            Vector2u uSize = particleManager.FireworksTexture.Size;

            particle.Scale = (float)cAppMath.GetRandomDoubleInRange(this.minScale, this.maxScale);

            particle.Dims = new Vector2f(uSize.X * particle.Scale, uSize.Y * particle.Scale);

            particle.ScaleSpeed = -cAppMath.GetRandomNumber(10, 50);
            particle.Color = Color.White; // Utils.GetRandomRedColor();
            particle.Opacity = 255.0f;
            particle.Life = 1.0f;
            particle.Fade = 60; //Math->GetRandomNumber(100, 240);

            particle.Intersects = false;
        }

        public void LittleExplosion(cEmissionInfo emission)
        {
            minScale = 0.2;
            maxScale = 0.4;
            loopAddition(emission, 3);
        }

        public void NormalExplosion(cEmissionInfo emission)
        {
            minScale = 0.5;
            maxScale = 0.8;
            loopAddition(emission, 30);
        }

        public override void Update(float step_time)
        {
            cWorld world = particleManager.Scene.World;

            for (int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);

                if (!p.Intersects)
                {

                    /*
                       p.Dims.Y += p.ScaleSpeed * step_time; //+=
                       p.Dims.y += p.ScaleSpeed * step_time; //+=
                    */
                    cAppMath.Vec2Truncate(ref p.Vel, p.MaxSpeed * 1.5f);


                    p.Vel.Y += (Constants.GRAVITY * 40.0f * (step_time * step_time));

                    p.Vel.X *= p.SlowDown;
                    p.Vel.Y *= p.SlowDown;

                    //p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);
                    p.LastPos = p.Pos;
                    p.Pos.X += p.Vel.X * step_time;
                    p.Pos.Y += p.Vel.Y * step_time;



                    if (!cAppMath.Vec2IsZero(p.Vel))
                    {
                        p.Heading = cAppMath.Vec2NormalizeReturn(p.Vel);
                        p.Rotation = (float)cAppMath.GetAngleOfVector(p.Heading);
                    }

                    world.collideParticleRayTrace(p, step_time);


                }

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

            Vector2u uSize = particleManager.ExplosionTexture.Size;
            float tSizeX = uSize.X;
            float tSizeY = uSize.Y;

            Vector2f newDims = new Vector2f();

            for (int i = 0; i < pool.CountActive; ++i)
            {
                Particle p = pool.get(i);

                newDims.X = p.Dims.X / division;
                newDims.Y = p.Dims.Y / division;

                p.ViewPos = cAppMath.Interpolate(p.LastPos, p.Pos, alpha);

                uint vertexIndex = (uint)i * multiplier;

                Vector2f v0Pos = new Vector2f(p.ViewPos.X - newDims.X,
                                              p.ViewPos.Y - newDims.Y);

                Vector2f v1Pos = new Vector2f(p.ViewPos.X + newDims.X,
                                              p.ViewPos.Y - newDims.Y);

                Vector2f v2Pos = new Vector2f(p.ViewPos.X + newDims.X,
                                              p.ViewPos.Y + newDims.Y);

                Vector2f v3Pos = new Vector2f(p.ViewPos.X - newDims.X,
                                              p.ViewPos.Y + newDims.Y);


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
                                   new Vector2f(p.Dims.X, 0.0f)
                                   );

                // Bottom-right
                Vertex v2 = new Vertex(
                                   v2Pos,
                                   p.Color,
                                   new Vector2f(p.Dims.X, p.Dims.Y)
                                   );

                //Bottom-left
                Vertex v3 = new Vertex(
                                   v3Pos,
                                   p.Color,
                                   new Vector2f(0.0f, p.Dims.Y)
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
        }
    }
}
