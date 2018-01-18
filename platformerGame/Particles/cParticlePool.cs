using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;
using SFML.Graphics;

namespace platformerGame.Particles
{
    /// <summary>
    /// Simple class storing the particles.
    /// </summary>
    class cParticlePool
    {
        uint capacity;
        Particle[] particles;
        uint countActive;

        public cParticlePool()
        {
            capacity = 10;
            initParticles();
            countActive = 0;
        }

        public cParticlePool(uint capacity)
        {
            this.capacity = capacity;
            initParticles();
            countActive = 0;
        }

        private void initParticles()
        {
            particles = new Particle[capacity];
            for(int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle();
            }
        }

        void swap(uint indexA, uint indexB)
        {
            Particle temp = particles[indexA];
            particles[indexA] = particles[indexB];
            particles[indexB] = temp;
        }

        void swapR(ref Particle A, ref Particle B)
        {
            Particle C = A;
            A = B;
            B = C;
        }

        /// <summary>
        /// Gets a new particle.
        /// </summary>
        /// <returns>A tuple: Item1 is the index of the particle, Item2 is the particle.</returns>
        public Particle getNew()
        {     
            uint current = countActive++;

            if (countActive > capacity)
            {
                countActive = capacity;
                return null;
            }

            return particles[current];
            
        }

        private bool checkIndex(int index)
        {
            return index >= 0 && index < particles.Length;
        }
        

        public Particle get(uint index)
        {
            //return  checkIndex(index) ? particles[index] : null;
            return particles[index];
        }

        public void activate(uint index)
        {
            swap(index, countActive);
            countActive++;
        }

        public void deactivate(uint index)
        {
            //if (checkIndex(index))
            {
                swap(index, countActive-1);
                countActive--;
            }
        }

        public uint CountActive
        {
            get { return countActive; }
        }
        
    }
}
