using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.Containers
{
    class GridOccupant
    {
        protected AABB bounds;
        public GridOccupant()
        {
            bounds = new AABB(0, 0, 0, 0);
        }

        public GridOccupant(AABB _bounds)
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
