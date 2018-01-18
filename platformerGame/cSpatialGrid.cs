using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace platformerGame
{
    class cSpatialGrid
    {
        const int CELL_SIZE = 128; // 128 X 128 square

        cSpatialCell[] grid;
        int height;
        int width;

        public cSpatialGrid(int w, int h)
        {
            this.width = w;
            this.height = h;
            this.grid = new cSpatialCell[w * h];
        }


        public int GetIndexByXY(int x, int y)
        {
            int index = y * width + x;
            return index;
        }

        public static Vector2i ToGridPos(Vector2f world_pos)
        {
            Vector2i ret = new Vector2i();
            ret.X = (int)(world_pos.X / CELL_SIZE);
            ret.Y = (int)(world_pos.Y / CELL_SIZE);
            return ret;
        }

        public static Vector2f ToWorldPos(Vector2i grid_pos)
        {
            Vector2f ret = new Vector2f();
            ret.X = grid_pos.X * CELL_SIZE;
            ret.Y = grid_pos.Y * CELL_SIZE;
            return ret;
        }

        public int[] getObjectsAtPos(Vector2f position)
        {
            Vector2i gp = ToGridPos(position);
            int index = GetIndexByXY(gp.X, gp.Y);
            return grid[index].getObjectsOnlyActive();
        }

        public int[] getPossibleCollidableObjects(cAABB entity_bounds)
        {
            List<int> all = new List<int>();
            all.AddRange(getObjectsAtPos(entity_bounds.topLeft));
            all.AddRange(getObjectsAtPos(entity_bounds.topLeft));
            return all.ToArray();
        }

        private Vector2f getTopRight(cAABB rect)
        {
            return new Vector2f(rect.rightBottom.X, rect.topLeft.Y);
        }

        private Vector2f getBottomLeft(cAABB rect)
        {
            return new Vector2f(rect.topLeft.X, rect.rightBottom.Y);
        }

    }
}
