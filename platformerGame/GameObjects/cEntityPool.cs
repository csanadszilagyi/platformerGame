using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

using platformerGame.GameObjects;

namespace platformerGame
{
    class cEntityPool : IPool
    {
        cGameScene pScene;
        List<cBullet> bullets;
        List<cMonster> monsters;

        cSpatialGrid spatialGrid;
        // id - pickable object (id is the id of object, generated when statically when created)
        Dictionary<int, cPickupAble> pickups;

        //List<cPickupAble> pickups;

        cQuadTree<cMonster> treeMonsters;
        //cQuadTree<cBullet> treeBullets;

        Vector2f worldSize;

        cPlayer pPlayer;

        //Dictionary<int, cGameObject> entityMap;

        public cEntityPool(cGameScene scene, Vector2f world_size, cPlayer p_player)
        {
            this.pScene = scene;
            this.worldSize = world_size;
            this.pPlayer = p_player;
            this.bullets = new List<cBullet>();
            this.monsters = new List<cMonster>();

            this.treeMonsters = new cQuadTree<cMonster>(1, scene.World.WorldBounds);
            this.pickups = new Dictionary<int, cPickupAble>();
            this.spatialGrid = new cSpatialGrid(worldSize);
            //this.treeBullets = new cQuadTree<cBullet>(1, scene.World.WorldBounds);
        }
        public void AddBullet(cGameObject owner, Vector2f pos, Vector2f direction)
        {
            cBullet bullet = new cBullet(owner, pos, direction);
            bullets.Add(bullet);
        }

        public void AddPickup(cPickupAble item)
        {
            pickups.Add(item.ID, item);
        }

        public void AddBullet(cBullet bullet)
        {
            bullets.Add(bullet);
        }

        public void AddMonster(cMonster monster)
        {
            this.monsters.Add(monster);
        }

        public void Update(float step_time)
        {
            spatialGrid.ClearAll();

            this.checkBulletVsEntityCollisions(step_time);
            

            treeMonsters.Clear();

            //treeBullets.Clear();


            //update bullets
            int bulletCount = bullets.Count;
            for (int i = 0; i < bulletCount; i++)
            {
                if (bullets[i].isActive())
                {
                    bullets[i].Update(step_time);
                    //treeBullets.AddEntity(bullets[i]);
                }
                else
                {
                    bullets.RemoveAt(i);
                    i--;
                    bulletCount = bullets.Count;
                }
            }

            //update monsters
            int monsterCount = monsters.Count;
            for (int i = 0; i < monsterCount; i++)
            {
                if (monsters[i].isActive())
                {
                    monsters[i].Update(step_time);
                    treeMonsters.AddEntity(monsters[i]);
                }
                else
                {
                    monsters.RemoveAt(i);
                    i--;
                    monsterCount = monsters.Count;
                }
            }

            // update pickups
            List<int> keysToRemove = new List<int>();
            foreach(var item in pickups)
            {
                cPickupAble pickup = item.Value;
                if (pickup.isActive())
                {
                    pickup.Update(step_time);
                }
                else
                {
                    //pickups.Remove(item.Key);
                    keysToRemove.Add(item.Key);
                }
            }

            foreach(int key in keysToRemove)
            {
                pickups.Remove(key);
            }

            this.checkPlayerCanUseOrPickupOrInteract();

            /*
            int pickupCount = pickups.Count;
            for (int i = 0; i < pickupCount; i++)
            {
                if (pickups[i].isActive())
                {
                    pickups[i].Update(step_time);
                }
                else
                {
                    pickups.RemoveAt(i);
                    i--;
                    pickupCount = pickups.Count;
                }
            }
            */
        }

        private int getIndexOfClosestMonsterColliding(List<cMonster> mons, cBullet bul, Vector2f pos_by, float time)
        {
            int index = -1;
            double prevDist = Double.MaxValue;
            double newDist = 0.0;
            for(int i = 0; i < mons.Count; i++)
            {
                if (mons[i].IsKilled)
                    continue;

                // order by distance to find the closest
                if (cSatCollision.checkAndResolve(bul, mons[i], time, false))
                {
                    newDist = cAppMath.Vec2DistanceSqrt(mons[i].Bounds.center, pos_by);

                    if (newDist < prevDist)
                    {
                        prevDist = newDist;
                        index = i;
                    }
                }
                
            }

            return index;
        }

        public void checkPlayerCanUseOrPickupOrInteract()
        {
            int[] ids = spatialGrid.getPossibleCollidableObjectsWithAdjacents(pScene.Player.Bounds.center);

            foreach (var id in ids)
            {
                pickups[id].checkContactWithPlayer();
            }
        }

        public void checkBulletVsEntityCollisions(float step_time)
        {
            List<cMonster> collisionMonsters = new List<cMonster>();

            for (int b = 0; b < bullets.Count; b++)
            {
                
                Vector2f intersection = new Vector2f(0.0f, 0.0f);

                collisionMonsters = treeMonsters.GetEntitiesAtPos(bullets[b].Position);

                
                int monster = getIndexOfClosestMonsterColliding(collisionMonsters, bullets[b], bullets[b].Position, step_time);
                
                if(monster > -1)
                {
                    cCollision.resolveMonsterVsBullet(collisionMonsters[monster], bullets[b], intersection);
                }


                /*
                int m = 0;
                while (m < collisionMonsters.Count
                        // && !cSatCollision.checkAndResolve(bullets[b], collisionMonsters[m], step_time, false)
                        //!cCollision.testBulletVsEntity(bullets[b].Position, bullets[b].LastPosition, collisionMonsters[m].Bounds, ref intersection)
                        )
                {
                    if (bullets[b].Alive == false) break;

                    if (!collisionMonsters[m].Disabled && cSatCollision.checkAndResolve(bullets[b], collisionMonsters[m], step_time, false))
                    {
                        cCollision.resolveMonsterVsBullet(collisionMonsters[m], bullets[b], intersection);
                    }
                    m++;
                }
                */

                /*
                if (m < collisionMonsters.Count)
                {
                    cCollision.resolveMonsterVsBullet(collisionMonsters[m], bullets[b], intersection);
                }
                */
            }
        }
        
        public int getNumOfActiveBullets()
        {
            return bullets.Count;
        }

        public void RenderBullets(RenderTarget destination, float alpha)
        {
            //draw bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].CalculateViewPos(alpha);
                bullets[i].Render(destination);
            }
        }

        public void RenderEntities(RenderTarget destination, float alpha)
        {
            //draw monsters
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].CalculateViewPos(alpha);
                monsters[i].Render(destination);
            }
        }


        public void RenderPickups(RenderTarget destination, float alpha)
        {
            //draw pickups
            foreach (var item in pickups)
            {
                item.Value.CalculateViewPos(alpha);
                item.Value.Render(destination);
            }
            /*
            for (int i = 0; i < pickups.Count; i++)
            {
                pickups[i].CalculateViewPos(alpha);
                pickups[i].Render(destination);
            }
            */
        }

        public void RenderQuadtree(RenderTarget destination)
        {
            this.treeMonsters.DrawBounds(destination);
        }

        public List<IDrawable> ListVisibles(cAABB view_region)
        {
            List<IDrawable> visibleObjects = new List<IDrawable>();

            for (int i = 0; i < bullets.Count; i++)
            {
                if (cCollision.OverlapAABB(bullets[i].Bounds, view_region))
                    visibleObjects.Add(bullets[i]);
            }

            return visibleObjects;

        }

        public cSpatialGrid SpatialGrid
        {
            get { return spatialGrid; }
        }
    }
}
