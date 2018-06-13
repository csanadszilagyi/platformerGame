using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.Particles
{
    interface IParticleContainer
    {
        Particle getNew();
        Particle get(uint index);
        void activate(uint index);
        void deactivate(uint index);
    }
}
