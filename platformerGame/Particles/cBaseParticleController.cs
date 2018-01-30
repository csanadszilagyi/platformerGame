using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SFML.System;
using SFML.Graphics;

namespace platformerGame.Particles
{
    abstract class cBaseParticleController
    {
        protected cParticleManager particleManager;
        protected cParticlePoolList pool;
        protected VertexArray vertices;
        protected RenderStates renderStates;

        public cBaseParticleController(cParticleManager manager, uint max_particles = 30)
        {
            this.particleManager = manager;
            this.pool = new cParticlePoolList((int)max_particles); // new ParticlePool()
            this.vertices = new VertexArray(PrimitiveType.Quads);
            this.renderStates = new RenderStates(BlendMode.Add);
        }

        protected void loopAddition(cEmissionInfo emission,  uint num_particles)
        {
            uint i = 0;
            while ((emission.Particle = pool.getNew()) != null && i < num_particles)
            {
                initParticle(emission);
                ++i;
            }
        }

        public int NumActive
        {
            get { return pool.CountActive; }
        }

        protected abstract void initParticle(cEmissionInfo emission);

        public abstract void Update(float step_time);

        public abstract void BuildVertexBuffer(float alpha);

        public abstract void Render(RenderTarget destination, float alpha);
    }
}
