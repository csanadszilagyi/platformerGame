using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    abstract class cParticleSystem
    {
        protected List<Particle> particles;
        protected RenderStates renderStates;
        protected cGameScene scene = null;

        protected Vector2f position;
        protected int numActive;
        protected int maxParticles;
        protected bool active;
        protected bool repeat;

        protected Texture particleTexture = null;

        public cParticleSystem(cGameScene scene, Texture particleTexture)
        {
            this.scene = scene;
            this.particles = new List<Particle>();

            this.particleTexture = particleTexture;
            this.renderStates = new RenderStates(BlendMode.Add);
            this.renderStates.Texture = particleTexture;

            this.active = true;
            this.repeat = false;
            this.numActive = 0;
        }

        protected void clearParticles()
        {
            this.particles.Clear();
        }

        protected void Add(Particle p)
        {
            this.particles.Add(p);
        }

        protected void Remove(Particle p)
        {
            this.particles.Remove(p);
        }
        //public abstract void BuildVertexBuffer(float alpha);
        public abstract Particle CreateParticle();

        public abstract void ReinitParticle(int index);
        public abstract void Emit(Vector2f pos);

        public abstract void Update(float step_time);

        public abstract void Render(RenderTarget target, float alpha);

        public bool Active
        {
            get { return this.active; }
        }
    }
}
