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
        int numCells;

        public cSpatialGrid(int w, int h)
        {
            this.width = w;
            this.height = h;
            this.numCells = w * h;
            this.grid = new cSpatialCell[numCells];
            initCells();
        }

        public cSpatialGrid(Vector2f world_size)
        {
            this.width = (int)world_size.X / CELL_SIZE;
            this.height = (int)world_size.Y / CELL_SIZE;
            this.numCells = width * height;
            this.grid = new cSpatialCell[numCells];
            initCells();
            
        }

        private void initCells()
        {
            for(int i = 0; i < grid.Length; ++i)
            {
                grid[i] = new cSpatialCell(i);
            }
        }
        public void ClearAll()
        {
            foreach (var cell in grid)
            {
                cell.clear();
            }
        }

        public int GetIndexByXY(int x, int y)
        {
            int index = y * width + x;
            return index;
        }

        public int GetIndexByXY(Vector2i grid_pos)
        {
            int index = grid_pos.Y * width + grid_pos.X;
            return index; // index < 0 ? 0 : index >= numCells ? numCells -1 : index;
        }

        public cSpatialCell GetCellAtXY(int grid_x, int grid_y)
        {
            int index = grid_y * width + grid_x;
            return index >= 0 && index < numCells ? grid[index] : null;
        }

        public int GetCellIndexAtWorldPos(Vector2f world_pos)
        {
            Vector2i gridPos = ToGridPos(world_pos);
            return GetIndexByXY(gridPos);
        }

        public cSpatialCell getCell(Vector2f world_pos)
        {
            Vector2i gridPos = ToGridPos(world_pos);
            int index = GetIndexByXY(gridPos);
            return grid[index];
        }

        public Vector2i ToGridPos(Vector2f world_pos)
        {
            Vector2i ret = new Vector2i();
            ret.X = (int)(world_pos.X / CELL_SIZE);
            ret.Y = (int)(world_pos.Y / CELL_SIZE);
            
            ret.X = ret.X >= width ? width - 1 : ret.X < 0 ? 0 : ret.X;
            ret.Y = ret.Y >= height ? height - 1 : ret.Y < 0 ? 0 : ret.Y;
            return ret;
        }

        public Vector2f ToWorldPos(Vector2i grid_pos)
        {
            Vector2f ret = new Vector2f();
            ret.X = grid_pos.X * CELL_SIZE;
            ret.Y = grid_pos.Y * CELL_SIZE;
            return ret;
        }

        public void DeHandleObject(int id, AABB last_bounds)
        {

        }

        public void GetCells()
        {
        }

        public void HandleObject(int id, AABB bounds)
        {
            int topLeftIndex = GetCellIndexAtWorldPos(bounds.topLeft);
            int topRightIndex = GetCellIndexAtWorldPos(bounds.getTopRight());
            int bottomRightIndex = GetCellIndexAtWorldPos(bounds.rightBottom);
            int bottomLeftIndex = GetCellIndexAtWorldPos(bounds.getLeftBottom());


                if (topLeftIndex == bottomRightIndex)
                {
                    grid[topLeftIndex].add(id);
                }
                else
                {
                    if (topLeftIndex == bottomLeftIndex || bottomLeftIndex == bottomRightIndex)
                    {
                        grid[topLeftIndex].add(id);
                        grid[bottomRightIndex].add(id);
                    }
                    else
                    {
                        grid[topLeftIndex].add(id);
                        grid[topRightIndex].add(id);
                        grid[bottomRightIndex].add(id);
                        grid[bottomLeftIndex].add(id);
                    }
                }
            }

        public int[] getObjectsAtWorldPos(Vector2f world_pos)
        {
            return getCell(world_pos)?.getAll();
        }

        public int[] getObjectsAtGridPos(int grid_x, int grid_y)
        {
            return GetCellAtXY(grid_x, grid_y)?.getAll();
        }

        public int[] getPossibleCollidableObjects(AABB bounds)
        {
            List<int> all = new List<int>();
            int topLeftIndex = GetCellIndexAtWorldPos(bounds.topLeft);
            int topRightIndex = GetCellIndexAtWorldPos(bounds.getTopRight());
            int bottomRightIndex = GetCellIndexAtWorldPos(bounds.rightBottom);
            int bottomLeftIndex = GetCellIndexAtWorldPos(bounds.getLeftBottom());

            if (topLeftIndex == bottomRightIndex)
            {
                all.AddRange(grid[topLeftIndex].getAll());
            }
            else
            {
                if (topLeftIndex == bottomLeftIndex || bottomLeftIndex == bottomRightIndex)
                {
                    all.AddRange(grid[topLeftIndex].getAll());
                    all.AddRange(grid[bottomRightIndex].getAll());
                }
                else
                {
                    all.AddRange(grid[topLeftIndex].getAll());
                    all.AddRange(grid[topRightIndex].getAll());
                    all.AddRange(grid[bottomRightIndex].getAll());
                    all.AddRange(grid[bottomLeftIndex].getAll());
                }
            }
            return all.ToArray();
        }


        public int[] getPossibleCollidableObjectsWithAdjacents(Vector2f position)
        {
            List<int> all = new List<int>();
            Vector2i gridPos = ToGridPos(position);
            int[] objs;

            if(null != (objs = getObjectsAtGridPos(gridPos.X - 1, gridPos.Y - 1)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X, gridPos.Y - 1)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X + 1, gridPos.Y - 1)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X - 1, gridPos.Y)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X, gridPos.Y)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X + 1, gridPos.Y)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X - 1, gridPos.Y + 1)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X, gridPos.Y + 1)))
            {
                all.AddRange(objs);
            }

            if (null != (objs = getObjectsAtGridPos(gridPos.X + 1, gridPos.Y + 1)))
            {
                all.AddRange(objs);
            }
            
            return all.ToArray();
        }

    }
}
