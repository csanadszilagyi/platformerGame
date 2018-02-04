using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cQuadTreeOccupant
    {
        protected cAABB bounds;
        public cQuadTreeOccupant()
        {
            bounds = new cAABB(0, 0, 0, 0);
        }

        public cQuadTreeOccupant(cAABB _bounds)
        {
            bounds = _bounds.ShallowCopy();
        }

        public cAABB Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }
    }
}
