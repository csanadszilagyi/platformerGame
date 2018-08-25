using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cQuadTreeOccupant
    {
        protected AABB bounds;
        public cQuadTreeOccupant()
        {
            bounds = new AABB(0, 0, 0, 0);
        }

        public cQuadTreeOccupant(AABB _bounds)
        {
            bounds = _bounds.ShallowCopy();
        }

        public AABB Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }
    }
}
