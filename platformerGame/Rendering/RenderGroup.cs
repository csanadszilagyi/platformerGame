using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace platformerGame.Rendering
{
    class RenderGroup : Drawable
    {
        List<IDrawable> objects = new List<IDrawable>();
        string id = "";

        public RenderGroup()
        {

        }

        // Ha valaki nem tudja hova kell hajózni, egyik szél se jó neki.

        public IEnumerable<IDrawable> Objects
        {
            get { return this.objects;}  
        }

        public void Add(IDrawable obj)
        {
            
            this.objects.Add(obj);
        }

        public virtual void PreRender(float alpha)
        {

        }

        public void Draw(RenderTarget target, RenderStates states)
        {

        }
    
    }
}
