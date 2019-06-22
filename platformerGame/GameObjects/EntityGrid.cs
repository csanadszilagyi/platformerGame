using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using platformerGame.Utilities;
using platformerGame.Containers;

namespace platformerGame.GameObjects
{
    
    class EntityGrid<T> where T : GridOccupant
    {
        public const int ENTITY_GRID_SIZE = 128;
        public const int ENTITY_OVERSCAN = 128;

        protected List<T> allEntities;
        //protected List<T> visibleEntities; // on screen

        protected Dictionary<Vector2i, List<T>> entityGrid;
        protected float cleanupTimer = 0.0f;

        public AABB GridArea { get; set; }

        public EntityGrid(Vector2f world_size)
        {
            GridArea = new AABB(0.0f, 0.0f, world_size.X, world_size.Y);
            this.initGrid();
        }

        public EntityGrid(AABB grid_bounds)
        {
            GridArea = grid_bounds;
            this.initGrid();
        }

        protected void initGrid()
        {
            this.allEntities = new List<T>();
            //this.visibleEntities = new List<T>();
            this.entityGrid = new Dictionary<Vector2i, List<T>>();
        }

        public static Vector2i calcGridPos(Vector2f world_pos)
        {
            int x = (int)world_pos.X / ENTITY_GRID_SIZE;
            int y = (int)world_pos.Y / ENTITY_GRID_SIZE;
            return new Vector2i(x, y);
        }

        public List<T> getInGridRect(int startX, int startY, int endX, int endY)
        {
            var pos = new Vector2i();

            List<T> returner = new List<T>();

            for (var y = startY; y <= endY; y++)
            {
                for (var x = startX; x <= endX; x++)
                {
                    pos.X = x;
                    pos.Y = y;

                    List<T> list;
                    if (entityGrid.TryGetValue(pos, out list))
                    {
                        returner.AddRange(list);
                    }
                }
            }

            return returner;
        }

        public List<T> getEntitiesInRadius(Vector2f centre, float radius)
        {
            float sideLen = 2.0f * radius;
            AABB circleBoundRect = new AABB();
            circleBoundRect.SetDims(new Vector2f(sideLen, sideLen));
            circleBoundRect.SetPosByCenter(centre);

            return getEntitiesInArea(circleBoundRect);
        }

        public List<T> getEntitiesNearby(Vector2f pos)
        {
            Vector2i gridPos = calcGridPos(pos);
            return this.getInGridRect(gridPos.X - 1, gridPos.Y - 1, gridPos.X + 1, gridPos.Y + 1);
        }

        public List<T> getEntitiesInArea(AABB area)
        {
            FloatRect areaRect = area.AsSfmlFloatRect();

            var overscan = ENTITY_OVERSCAN;
            var gridSize = ENTITY_GRID_SIZE;

            FloatRect rect = new FloatRect(areaRect.Left - overscan, areaRect.Top - overscan,
                                 areaRect.Width + (overscan * 2), areaRect.Height + (overscan * 2));

            var startX = (int)rect.Left / gridSize;
            var startY = (int)rect.Top / gridSize;
            var width = (int)rect.Width / gridSize + 1;
            var height = (int)rect.Height / gridSize + 1;

            return this.getInGridRect(startX, startY, startX + width, startY + height);
        }

        public List<T> GetAllEntities()
        {
            return this.allEntities;
        }

        public void AddEntities(IEnumerable<T> entities)
        {
            foreach(var e in entities)
            {
                this.allEntities.Add(e);
                this.GridAdd(e);
            }
        }

        public void AddEntity(T e)
        {
            this.allEntities.Add(e);
            this.GridAdd(e);
        }

        public void RemoveEntity(T e)
        {
            // e.Destroy();

            GridRemove(e);
            this.allEntities.Remove(e);
        }

        public void RemoveAll()
        {
            foreach (var e in this.entityGrid)
            {
                e.Value.Clear();
            }

            this.entityGrid.Clear();
            this.allEntities.Clear();
        }

        protected void checkForCleanup(float step_time)
        {
            // cleanup grid
            cleanupTimer += step_time;
            // System.Diagnostics.Debug.WriteLine(cleanupTimer);

            if (cleanupTimer >= 6.0f)
            {
                entityGrid.RemoveAll(kv => kv.Value.Count == 0);
                cleanupTimer = 0.0f;
            }
        }

        protected void gridHandleUpdate(float step_time)
        {
            // update all entites
            Vector2i newGridPos = new Vector2i(0, 0);

            int eCount = allEntities.Count;
            for (int i = 0; i < eCount; ++i)
            {
                T entity = allEntities[i];

                if (entity.isActive())
                {

                    // updateFunc?.Invoke(entity, step_time);

                    entity.Update(step_time);

                    var bounds = entity.Bounds;
                    newGridPos = calcGridPos(bounds.center);

                    if (!entity.GridPosition.Equals(newGridPos))
                    {
                        GridRemove(entity);
                        entity.GridPosition = newGridPos;
                        GridAdd(entity);
                    }

                    continue;
                }

                GridRemove(entity);
                allEntities.RemoveAt(i);
                i--;
                eCount = allEntities.Count;
            }
        }

        public virtual void Update(float step_time)
        {
            this.gridHandleUpdate(step_time);
            this.checkForCleanup(step_time);
        }

        protected void GridAdd(T e)
        {
            List<T> list;

            if (false == entityGrid.TryGetValue(e.GridPosition, out list))
            {
                list = new List<T>();
                list.Add(e);
                entityGrid.Add(e.GridPosition, list);
                return;
            }

            list.Add(e);
        }

        protected bool GridRemove(T e)
        {
            List<T> list;

            if (entityGrid.TryGetValue(e.GridPosition, out list))
            {
                return list.Remove(e);
            }


            #if DEBUG
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Remove - FALSE"));
            #endif

            return false;
        }

        /// <summary>
        /// Filters visible entites and performs view position calculation for the visible ones.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> filterVisibles(AABB view_region, Action<GridOccupant> executeOnTrue = null)
        {
            return this.allEntities.Where( e => 
                {
                    executeOnTrue?.Invoke(e);
                    return cCollision.OverlapAABB(e.Bounds, view_region);
                }
            );

            /*
            foreach (var e in allEntities)
            {
                if (cCollision.OverlapAABB(e.Bounds, view_region))
                {
                    // e.CalculateViewPos(alpha);
                    yield return e;
                }
            }
            */
        }
    }
    
}
