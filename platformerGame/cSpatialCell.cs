using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cSpatialCell
    {
        const int MAX_OBJECTS = 10;

        Dictionary<int, int> objIDandContainers;

        int[] objects; // store the indices of objects
        int objectCount;

        int cellIndex; // the index of this cell in the spatial grid

        /*
        int gridX, gridY;
        cAABB bounds;
        */

        public cSpatialCell()
        {
            reinit();
            cellIndex = 0;
        }

        public cSpatialCell(int cell_index)
        {
            reinit();
            this.cellIndex = cell_index;
        }

        private void swap(int index_a, int index_b)
        {
            int temp = objects[index_a];
            objects[index_a] = objects[index_b];
            objects[index_b] = temp;
        }

        private void reinit()
        {
            objects = new int[MAX_OBJECTS];
            objectCount = 0;
        }
        public void clear()
        {
            reinit();
        }

        public void add(int obj_index)
        {
            int position = objectCount++;
            objects[position] = obj_index;
        }

        public void removeAt(int index)
        {
            swap(index, objectCount - 1);
            objectCount--;
        }

        public int get(int index)
        {
            return objects[index];
        }

        /// <summary>
        /// Gets the array of object indices contained by this cell.
        /// </summary>
        /// <returns>Returns an array, containing only the number of current count of elements.</returns>
        public int[] getObjectsOnlyActive()
        {
            int[] r = new int[objectCount];
            Array.Copy(objects, r, objectCount);
            return r;
        }

        /// <summary>
        /// Gets the array of object indices contained by this cell.
        /// </summary>
        /// <returns>Returns a tuple-2: item1 is the array with size MAX_OBJECTS, item2 is count of active elements.</returns>
        public Tuple<int[], int> getObjects()
        {
            return new Tuple<int[], int>(objects, objectCount);
        }

        public int CellIndex
        {
            get { return cellIndex; }
        }
    }
}
