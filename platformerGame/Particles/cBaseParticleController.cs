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

        public cBaseParticleController(cParticleManager manager, int max_particles = 30)
        {
            this.particleManager = manager;
            this.pool = new cParticlePool(max_particles);
            this.vertices = new VertexArray(PrimitiveType.Quads);
        }

        protected abstract void initParticle(Particle particle, Vector2f position);

        public abstract void Update(float step_time);

        public abstract void BuildVertexBuffer(float alpha);

        public abstract void Render(RenderTarget destination, float alpha);
    }
}
