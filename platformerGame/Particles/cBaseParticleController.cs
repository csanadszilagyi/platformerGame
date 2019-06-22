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
        protected cParticlePool pool;
        protected VertexArray vertices;
        protected RenderStates renderStates;

        public cBaseParticleController(cParticleManager manager, int max_particles = 300)
        {
            this.particleManager = manager;
            this.pool = new cParticlePool(max_particles); // new ParticlePoolList()
            this.vertices = new VertexArray(PrimitiveType.Quads);
            this.renderStates = new RenderStates(BlendMode.Add);
        }

        protected void loopAddition(EmissionInfo emission,  uint num_particles)
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
            get { return (int)pool.CountActive; }
        }

        protected abstract void initParticle(EmissionInfo emission);

        public abstract void Update(float step_time);

        public abstract void BuildVertexBuffer(float alpha);

        public abstract void Render(RenderTarget destination, float alpha);
    }
}
