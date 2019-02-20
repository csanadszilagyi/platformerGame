using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace platformerGame.Particles
{
    class EmissionInfo
    {
        Particle particleRef;
        Vector2f startPosition;
        Vector2f emitDirection;

        public Particle Particle
        {
            get { return particleRef; }
            set { particleRef = value; }
        }

        public Vector2f StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
        }

        public Vector2f EmitDirection
        {
            get { return emitDirection; }
            set { emitDirection = value; }
        }

        public EmissionInfo(Vector2f start_position)
        {
            this.particleRef = null;
            this.startPosition = start_position;
            this.emitDirection = new Vector2f(0.0f, 0.0f);
        }

        public EmissionInfo(Vector2f start_position, Vector2f emit_direction)
        {
            this.particleRef = null;
            this.startPosition = start_position;
            this.emitDirection = emit_direction;
        }

        public EmissionInfo(Particle particle, Vector2f start_position)
        {
            this.particleRef = particle;
            this.startPosition = start_position;
            this.emitDirection = new Vector2f(0.0f, 0.0f);
        }

        public EmissionInfo(Particle particle, Vector2f start_position, Vector2f emit_direction)
        {
            this.particleRef = particle;
            this.startPosition = start_position;
            this.emitDirection = emit_direction;
        }
    }
}
