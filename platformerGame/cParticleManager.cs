using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cParticleManager
    {
        private cGameScene scene = null;
        private List<cParticleSystem> particleSystems;

        private readonly Texture SIMPLE_PARTICLE_TEXTURE;
        public cParticleManager(cGameScene scene)
        {
            SIMPLE_PARTICLE_TEXTURE = cAssetManager.GetTexture("simple_particle");
            this.scene = scene;
            this.particleSystems = new List<cParticleSystem>();
        }

        public void Add(cParticleSystem particleEffect)
        {
            this.particleSystems.Add(particleEffect);
        }

        public void AddExplosion(Vector2f pos)
        {
            cExplosionParticles ex = new cExplosionParticles(this.scene, SIMPLE_PARTICLE_TEXTURE);
            ex.Emit(pos);
            this.particleSystems.Add(ex);
        }
        public void Update(float step_time)
        {
            for(int i = 0; i < this.particleSystems.Count; i++)
            {
                if (!this.particleSystems[i].Active)
                {
                    this.particleSystems.RemoveAt(i);
                }
                else
                    this.particleSystems[i].Update(step_time);
            }
        }

        /*
        public void PreRender(float alpha)
        {
            foreach (var pSystem in this.particleSystems)
            {
                pSystem.BuildVertexBuffer(alpha);
            }
        }
        */
        public void Render(RenderTarget target, float alpha)
        {
            foreach (var pSystem in this.particleSystems)
            {
                pSystem.Render(target, alpha);
            }
        }
    }
}
