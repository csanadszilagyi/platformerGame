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
        float speed; // should be normalized

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

        public float Speed
        {
            get { return this.speed; }
        }

        public EmissionInfo(Vector2f start_position)
        {
            this.particleRef = null;
            this.startPosition = start_position;
            this.emitDirection = new Vector2f(0.0f, 0.0f);
            this.speed = 0.0f;
        }

        public EmissionInfo(Vector2f start_position, Vector2f emit_direction, float speed = 0.0f)
        {
            this.particleRef = null;
            this.startPosition = start_position;
            this.emitDirection = emit_direction;
            this.speed = speed;
        }

        public EmissionInfo(Particle particle, Vector2f start_position)
        {
            this.particleRef = particle;
            this.startPosition = start_position;
            this.emitDirection = new Vector2f(0.0f, 0.0f);
            this.speed = 0.0f;
        }

        public EmissionInfo(Particle particle, Vector2f start_position, Vector2f emit_direction, float speed = 0.0f)
        {
            this.particleRef = particle;
            this.startPosition = start_position;
            this.emitDirection = emit_direction;
            this.speed = speed;
        }

        public bool hasSpeed()
        {
            return speed > 0.0f;
        }
    }
}
