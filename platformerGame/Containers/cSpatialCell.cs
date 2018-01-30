using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cSpatialCell
    {
        List<int> objects; // IDs of the stored objects

        int cellIndex; // the index of this cell in the spatial grid

        /*
        int gridX, gridY;
        cAABB bounds;
        */

        public cSpatialCell()
        {
            cellIndex = 0;
            objects = new List<int>();
        }

        public cSpatialCell(int cell_index)
        {
            this.cellIndex = cell_index;
            objects = new List<int>();
        }

        public void clear()
        {
            objects.Clear();
        }

        public void add(int object_id)
        {
            if(false == objects.Contains(object_id))
                objects.Add(object_id);
        }

        public void remove(int object_id)
        {
            objects.Remove(object_id);
        }

        public int[] getAll()
        {
            return objects.ToArray();
        }
      

        public int CellIndex
        {
            get { return cellIndex; }
        }
    }
}
