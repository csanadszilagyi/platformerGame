using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.Particles
{
    class cParticlePoolList
    {
        List<Particle> particles;
        int countActive;

        public cParticlePoolList(int max_particles)
        {
            particles = new List<Particle>(max_particles);
            countActive = 0;
        }

        public Particle getNew()
        {
            Particle p = new Particle();
            particles.Add(p);
            countActive++;
            return p;
        }

        private bool checkIndex(int index)
        {
            return index >= 0 && index < particles.Count;
        }

        public Particle get(int index)
        {
            return particles[index];
        }

        public void deactivate(int index)
        {
            particles.RemoveAt(index);
            countActive--;
        }

        public int CountActive
        {
            get { return countActive; }
        }
    }
}
