using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace platformerGame.Particles
{
    class cEmissionInfo
    {
        Particle particle;
        Vector2f startPosition;
        Vector2f emitDirection;

        public Particle Particle
        {
            get { return particle; }
            set { particle = value; }
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

        public cEmissionInfo(Vector2f start_position)
        {
            this.particle = null;
            this.startPosition = start_position;
            this.emitDirection = new Vector2f(0.0f, 0.0f);
        }

        public cEmissionInfo(Vector2f start_position, Vector2f emit_direction)
        {
            this.particle = null;
            this.startPosition = start_position;
            this.emitDirection = emit_direction;
        }

        public cEmissionInfo(Particle particle, Vector2f start_position)
        {
            this.particle = particle;
            this.startPosition = start_position;
            this.emitDirection = new Vector2f(0.0f, 0.0f);
        }

        public cEmissionInfo(Particle particle, Vector2f start_position, Vector2f emit_direction)
        {
            this.particle = particle;
            this.startPosition = start_position;
            this.emitDirection = emit_direction;
        }
    }
}
