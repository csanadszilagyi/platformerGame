using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame.Rendering
{
    abstract class cBaseRenderer
    {
        protected RenderStates renderStates;

        public cBaseRenderer()
        {
            this.renderStates = new RenderStates(BlendMode.Alpha);
        }

        protected cBaseRenderer(cBaseRenderer other)
        {
            this.renderStates = other.renderStates;
        }

        // by: https://stackoverflow.com/questions/19119623/clone-derived-class-from-base-class-method
        public abstract cBaseRenderer DeepCopy();

        public abstract void Update(float step_time);
        public abstract void Draw(RenderTarget destination, Vector2f pos);
    }
}
