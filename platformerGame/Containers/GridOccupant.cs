using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace platformerGame.Containers
{
    abstract class GridOccupant
    {
        public AABB Bounds { get; set; } = new AABB(0, 0, 1, 1);
        public Vector2i GridPosition { get; set; } = new Vector2i(0, 0);
 
        public GridOccupant()
        {
        }

        public GridOccupant(AABB _bounds)
        {
            Bounds = _bounds.ShallowCopy();
        }

        public virtual void Update(float step_time)
        { }

        public virtual bool isActive()
        {
            return true;
        }
    }
}
