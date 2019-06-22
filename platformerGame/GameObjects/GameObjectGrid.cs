using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;


using platformerGame.Utilities;
using platformerGame.Map;
using tileLoader;
using platformerGame.App;
using platformerGame.GameObjects.PickupInfo;

namespace platformerGame.GameObjects
{
    class GameObjectGrid : EntityGrid<cGameObject>
    {
        GameScene refScene;
        cPlayer player;

        List<cGameObject> visibleEntities;

        public GameObjectGrid(GameScene scene, Vector2f world_size, cPlayer player) : base(world_size)
        {
            this.refScene = scene;
            this.player = player;
            visibleEntities = new List<cGameObject>();
        }

        public void InitLevelEntites(cMapData level)
        {
            // this.monsters.Clear();
            // this.allEntities.RemoveAll((cGameObject g) => g is cMonster );

            TmxMap map = level.GetTmxMap();
            TmxList<TmxObject> entityList = map.ObjectGroups["Entities"].Objects;
            foreach (var tmxEntity in entityList)
            {
                cMonster monster = new cMonster(this.refScene, new Vector2f((float)tmxEntity.X, (float)tmxEntity.Y));
                this.AddMonster(monster);
            }
        }

        public static Vector2i calcGridPos(Vector2f world_pos)
        {
            int x = (int)world_pos.X / ENTITY_GRID_SIZE;
            int y = (int)world_pos.Y / ENTITY_GRID_SIZE;
            return new Vector2i(x, y);
        }

        private List<cGameObject> getInGridRect(int startX, int startY, int endX, int endY)
        {
            var pos = new Vector2i();

            List<cGameObject> returner = new List<cGameObject>();

            for (var y = startY; y <= endY; y++)
            {
                for (var x = startX; x <= endX; x++)
                {
                    pos.X = x;
                    pos.Y = y;

                    List<cGameObject> list;
                    if (entityGrid.TryGetValue(pos, out list))
                    {
                        returner.AddRange(list);
                    }
                }
            }

            return returner;
        }

        public void AddPickup(cPickupAble p)
        {
            this.AddEntity(p);
        }

        public void AddMonster(cMonster m)
        {
            this.AddEntity(m);
        }

        public void AddBullet(cBullet b)
        {
            this.AddEntity(b);
        }


        public void Update(float step_time)
        {

            // bullet collision checks
            checkBulletVsEntityCollisions(step_time);


            // update all entites
            this.gridHandleUpdate(step_time);

            // check if can interact / use / pickup
            checkNearbyObjectsForPlayer();


            separateMonsters();


            // melee attack handling
            var meleeEntities = this.getPossiblePlayerMeleeAttackers();

            foreach (var monster in meleeEntities)
            {
                monster.Marked = true;
                this.checkForOneToOneSeparation(player, monster); // true
                monster.attemptMeleeAttack(player);
                
            }


            // this.checkForSeparation(player, meleeEntities);

            // cleanup grid
            this.checkForCleanup(step_time);
        }


        /*
        /// <summary>
        /// Filters visible entites and performs view position calculation for the visible ones.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<cGameObject> filterVisibles(float alpha, AABB view_region)
        {
            foreach (var e in allEntities)
            {
                if (cCollision.OverlapAABB(e.Bounds, view_region))
                {
                    e.CalculateViewPos(alpha);
                    yield return e;
                }

            }
        }
        */

        public void PreRender(float alpha, AABB view_region)
        {
            // todo: make it work without using "ToList()"

            this.visibleEntities = this.filterVisibles(
                view_region,
                e =>
                    {
                        var o = e as cGameObject;
                        o.CalculateViewPos(alpha);
                    }
            ).ToList();
        }

        public void RenderBullets(RenderTarget target)
        {
            foreach (var b in this.visibleEntities.OfType<cBullet>())
            {
                b.Render(target);
            }
        }

        public void RenderPickups(RenderTarget target)
        {
            foreach (var p in this.visibleEntities.OfType<cPickupAble>())
            {
                p.Render(target);
            }
        }

        public void RenderMonsters(RenderTarget target)
        {
            foreach (var m in this.visibleEntities.OfType<cMonster>())
            {
                m.Render(target);
            }
        }

        public void RenderTurrets(RenderTarget target)
        {
            foreach (var m in this.visibleEntities.OfType<cTurret>())
            {
                m.Render(target);
            }
        }
        
        /// <summary>
        /// For debug purposes
        /// </summary>
        /// <param name="target"></param>
        public void RenderGrid(RenderTarget target)
        {

            RectangleShape shape = new RectangleShape(new Vector2f(ENTITY_GRID_SIZE, ENTITY_GRID_SIZE));
            shape.OutlineColor = Color.Red;
            shape.OutlineThickness = 1.0f;
            shape.FillColor = new Color(255, 255, 255, 50);
            shape.Scale = new Vector2f { X = 1.0f, Y = 1.0f};
            
            foreach (var item in this.entityGrid)
            {
                Vector2i topLeft = item.Key;
                shape.Position = new Vector2f(topLeft.X*ENTITY_GRID_SIZE, topLeft.Y*ENTITY_GRID_SIZE);
                target.Draw(shape, new RenderStates(BlendMode.Add));
            }
        }

        private void checkForOneToOneSeparation(cGameObject A, cGameObject B, bool onlyFirst = false)
        {
            if (A.ID != B.ID &&
                   cCollision.testCircleVsCirlceOverlap(A.GetCenterPos(), A.BoundingRadius, B.GetCenterPos(), B.BoundingRadius))
            {
                cCollision.SeparateEntites(A, B, onlyFirst);
            }
        }

        private void checkForOneAndGroupSeparation(cGameObject someBody, IEnumerable<cGameObject> others)
        {
            foreach (var o in others)
            {
                this.checkForOneToOneSeparation(someBody, o);
            }
        }

        private void separateMonsters()
        {
            foreach (var m in this.visibleEntities.OfType<cMonster>())
            {
                var others = this.getEntitiesInArea(m.Bounds).OfType<cMonster>();
                this.checkForOneAndGroupSeparation(m, others);
            }
        }

        private cMonster getclosestMonsterColliding(IEnumerable<cMonster> possibleColliders, cBullet bul, Vector2f pos_by, float time)
        {
            // int index = -1;
            double prevDist = Double.MaxValue;
            double newDist = 0.0;
            cMonster returner = null;
            foreach (var mon in possibleColliders)
            {
                if (mon.IsKilled)
                    continue;

                // order by distance to find the closest
                if (cSatCollision.checkAndResolve(bul, mon, time, false))
                {
                    newDist = AppMath.Vec2DistanceSqrt(mon.Bounds.center, pos_by);

                    if (newDist < prevDist)
                    {
                        prevDist = newDist;
                        returner = mon;
                    }
                }

            }

            return returner;
        }


        public void checkBulletVsEntityCollisions(float step_time)
        {
            // List<cMonster> collisionMonsters = new List<cMonster>();
            var bullets = this.allEntities.OfType<cBullet>();

            foreach (var bullet in bullets)
            {

                Vector2f intersection = new Vector2f(0.0f, 0.0f);

                var collisionMonsters = this.getEntitiesNearby(bullet.Position).OfType<cMonster>(); //treeMonsters.GetEntitiesAtPos(bullets[b].Position);


                cMonster monster = getclosestMonsterColliding(collisionMonsters, bullet, bullet.Position, step_time);

                if (monster != null)
                {
                    cCollision.resolveMonsterVsBullet(monster, bullet, intersection);
                }

            }

            // TODO Player Vs Bullet
        }

        public IEnumerable<cMonster> getPossiblePlayerMeleeAttackers()
        {
            return this.getEntitiesInArea(player.Bounds).OfType<cMonster>();
        }

        /// <summary>
        /// Player's pull distance must be smaller then cell_size in the spatial grid.
        /// </summary>
        public void checkNearbyObjectsForPlayer()
        {
            var pickups = this.getEntitiesNearby(this.player.Bounds.center).OfType<cPickupAble>();

            foreach (var p in pickups)
            {
                p.checkContactWithPlayer();
            }
        }
    }
}
