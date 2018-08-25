using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

using platformerGame.Utilities;
using platformerGame.GameObjects;
using platformerGame.Map;

namespace platformerGame
{
    class TileMask
    {
        public static readonly IntRect TOP_LEFT = new IntRect(0, 0, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect TOP_MIDDLE = new IntRect(Constants.TILE_SIZE, 0, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect TOP_RIGHT = new IntRect(2*Constants.TILE_SIZE, 0, Constants.TILE_SIZE-1, Constants.TILE_SIZE);
        public static readonly IntRect MIDDLE_LEFT = new IntRect(0, Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect MIDDLE_CENTRE = new IntRect(Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect MIDDLE_RIGHT = new IntRect(2*Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect BOTTOM_LEFT = new IntRect(0, 2*Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect BOTTOM_MIDDLE = new IntRect(Constants.TILE_SIZE, 2* Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect BOTTOM_RIGHT = new IntRect(2* Constants.TILE_SIZE, 2* Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect ALONE = new IntRect(3* Constants.TILE_SIZE, 0, Constants.TILE_SIZE, Constants.TILE_SIZE);

        public static readonly IntRect ALONE_TOP = new IntRect(3 * Constants.TILE_SIZE, 2 * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect ALONE_TOP_WITH_BOT = new IntRect(4 * Constants.TILE_SIZE, 2 * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect ALONE_BOT_WITH_TOP = new IntRect(5 * Constants.TILE_SIZE, 2 * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);

        public static readonly IntRect NO_TOPBOT_LEFT = new IntRect(0, 3 * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect NO_TOPBOT_MIDDLE = new IntRect(Constants.TILE_SIZE, 3 * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        public static readonly IntRect NO_TOPBOT_RIGHT = new IntRect(2 * Constants.TILE_SIZE, 3 * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);

        public static readonly IntRect ONE_WAY_PLATFORM = new IntRect(4 * Constants.TILE_SIZE, 0, Constants.TILE_SIZE, Constants.TILE_SIZE/2);

        public static readonly IntRect NONE = new IntRect(0,0,0,0);
        public static readonly IntRect EMPTY = new IntRect(3 * Constants.TILE_SIZE, 3 * Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
    }

    class DrawTile
    {
        public int Left, Top;
        public AABB PosOnTexture;

        public DrawTile(int left, int top, AABB pos_on_texture)
        {
            this.Left = left;
            this.Top = top;
            this.PosOnTexture = pos_on_texture;
        }
    }

    class NeighInfo
    {
        private bool hasLeft, hasTop, hasRight, hasBottom;
        public bool HasLeft { get { return hasLeft; } set { hasLeft = value;  processChange(value); } }
        public bool HasRight { get { return hasRight; } set { hasRight = value; processChange(value); } }
        public bool HasTop { get { return hasTop; } set { hasTop = value; processChange(value); } }
        public bool HasBottom { get { return hasBottom; } set { hasBottom = value; processChange(value); } }
        public bool isAlone() { return (!HasLeft && !HasTop && !HasRight && !HasBottom); }
        public uint NumNeighs { get; set; }

        private void processChange(bool value)
        {
            NumNeighs = value ? NumNeighs + 1 : NumNeighs - 1;
        }
        public NeighInfo()
        {
            NumNeighs = 0;
            HasLeft = false;
            HasTop = false;
            HasRight = false;
            HasBottom = false;  
        }

        public NeighInfo(bool left, bool top, bool right, bool bottom)
        {
            NumNeighs = 0;
            HasLeft = left;
            HasTop = top;
            HasRight = right;
            HasBottom = bottom;
        }
    }

    class cWorld
    {

        cMapData currentLevel;
        AABB m_WorldBounds;
        Vector2u windowSize;

        Sprite background;
        Texture m_BGtexture = null;

        public AABB levelStartRegion;
        public AABB levelEndRegion;


        cGameScene pScene;


        cTileMapRenderer mapRenderer;

        public cWorld(cGameScene p_scene, Vector2u window_size)
        {
            pScene = p_scene;
            windowSize = window_size;


            currentLevel = new cMapData();
            //m_Level1.Create(100, 100);
            this.LoadLevel("levels/map_test2.tmx"); //"levels/Level1.txt"); //


            m_BGtexture = AssetManager.GetTexture(Constants.BG_TEXTURE);
            m_BGtexture.Repeated = true;
            m_BGtexture.Smooth = true;

            //tempSprite = new Sprite(m_TileSetTexture);

            background = new Sprite(m_BGtexture);
            background.TextureRect = new IntRect(0, 0, (int)m_WorldBounds.dims.X, (int)m_WorldBounds.dims.Y); // (int)m_TextureOfTiles.Size.X, (int)m_TextureOfTiles.Size.Y);
            background.Color = Constants.BACKGROUND_COLOR;

            mapRenderer = new cTileMapRenderer(this);

        }

        public void LoadLevel(string file_name)
        {
            // m_Level1.LoadFromFile(file_name);
            currentLevel.LoadFromTMX(file_name);

            m_WorldBounds = new AABB(0.0f, 0.0f, currentLevel.Width * Constants.TILE_SIZE, currentLevel.Height * Constants.TILE_SIZE);

            levelStartRegion = GetCurrentLevel().LevelStartRegion;
            levelEndRegion = GetCurrentLevel().LevelEndRegion;
            //initTileSprites();
        }
        public AABB LevelStartRegion
        {
            get { return levelStartRegion; }
        }

        public AABB LevelEndRegion
        {
            get { return levelEndRegion; }
        }

        /// <summary>
        /// Detecting water blocks located as a square in the tile map.
        /// </summary>
        /// <returns></returns>
        public List<cWaterBlock> GetWaterBlocks()
        {
            List<cWaterBlock> waters = new List<cWaterBlock>();

            try
            {

                int h = 0;
                int lastH = 0;
                int maxHOffset = 0;

                Vector2f topLeft = new Vector2f();
                Vector2f bottomRight = new Vector2f();

                bool wasWater = false;
                int w = 0;

                while (h < currentLevel.Height)
                {

                    cTile currentTile = currentLevel.GetTileAtXY(w, h);
                    if (currentTile.Type == TileType.WATER && !currentTile.IsCheckedWater)
                    {
                        if (!wasWater)
                        {
                            wasWater = true;
                            topLeft.X = w * Constants.TILE_SIZE;
                            topLeft.Y = h * Constants.TILE_SIZE;
                            lastH = h;

                        }

                        int heightOffset = 0;

                        bool water = true;
                        cTile tile = null;
                        while (water)
                        {
                            tile = currentLevel.GetTileAtXY(w, h + heightOffset);
                            if (tile.Type == TileType.WATER)
                            {
                                tile.IsCheckedWater = true;
                                heightOffset += 1;
                            }
                            else
                                water = false;

                        }

                        bottomRight.X = (w + 1) * Constants.TILE_SIZE;
                        int a = 0; // heightOffset <= 1 ? 1 : 0;
                        bottomRight.Y = (h + heightOffset + a) * Constants.TILE_SIZE;

                        if (heightOffset > maxHOffset)
                            maxHOffset = heightOffset;

                    }
                    else
                    {
                        if (wasWater)
                        {
                            waters.Add(new cWaterBlock(new AABB(topLeft, new Vector2f(Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y)))));
                            wasWater = false;
                            h = lastH;
                        }
                    }

                    w++;

                    if (w >= currentLevel.Width)
                    {
                        w = 0;

                        h += 1; // (maxHOffset + 1);
                        maxHOffset = 0;
                    }


                }
            }
            catch(Exception e)
            {
                throw e;
            }

            return waters;
        }

        private NeighInfo getNumOfLinearNeighsByCode(cMapData map, Vector2i tile_pos, TileType type)
        {

            NeighInfo returner = new NeighInfo();

            int tx = tile_pos.X;
            int ty = tile_pos.Y;

            cTile left = map.GetTileAtXY(tx - 1, ty);
            cTile top = map.GetTileAtXY(tx, ty - 1);
            cTile right = map.GetTileAtXY(tx + 1, ty);
            cTile bot = map.GetTileAtXY(tx, ty + 1);

            if (left != null && left.Type == type)
                returner.HasLeft = true;

		    if(top != null && top.Type == type)
                returner.HasTop = true;

            if (right != null && right.Type == type)
                returner.HasRight = true;
            
            if (bot != null && bot.Type == type)
                returner.HasBottom = true;

            return returner;
        }


        public void DrawBackground(RenderTarget destination)
        {
            destination.Draw(background);
        }

        

        public void PreRender(AABB view_rect)
        {
            mapRenderer.RecalculateDrawBounds(view_rect);

        }

        public void Render(RenderTarget destination, AABB view_rect)
        {
            //recalculateDrawBounds(view_rect);
           // DrawBackground(destination);

            mapRenderer.Render(destination, view_rect);

            //renderTilesToTexture(destination);
            /*
            FloatRect ft = view_rect.AsFloatRect();
            Vector2f pos = new Vector2f(ft.Left, ft.Top);

            cRenderFunctions.DrawTextureSimple(destination,
                            m_WorldBounds.topLeft + pos,
                            m_TextureOfTiles.Texture,
                            new IntRect((int)pos.X, (int)pos.Y, (int)ft.Width, (int)ft.Height), //(int)m_TextureOfTiles.Texture.Size.X, (int)m_TextureOfTiles.Texture.Size.Y), //20, 20, 200, 200),
                            Color.White,
                            BlendMode.Add);
            */
        }


        public Vector2i ToMapPos(Vector2f world_pos)
        {
            Vector2i ret = new Vector2i();
            ret.X = (int)(Math.Abs(world_pos.X - m_WorldBounds.topLeft.X) / Constants.TILE_SIZE);
            ret.Y = (int)(Math.Abs(world_pos.Y - m_WorldBounds.topLeft.Y) / Constants.TILE_SIZE);
            return ret;
        }

        public Vector2f ToWorldPos(Vector2i map_pos)
        {
            Vector2f ret = new Vector2f();
            ret.X = map_pos.X * Constants.TILE_SIZE + m_WorldBounds.topLeft.X;
            ret.Y = map_pos.Y * Constants.TILE_SIZE + m_WorldBounds.topLeft.Y;
            return ret;
        }

        public TileType GetTileTypeAtPos(Vector2f world_pos)
        {
            Vector2i map = ToMapPos(world_pos);
            return currentLevel.GetTileAtXY(map.X, map.Y).Type;
        }

        public bool isWallOrPlatform(Vector2f world_pos)
        {
            Vector2i map = ToMapPos(world_pos);
            cTile t = currentLevel.GetTileAtXY(map.X, map.Y);
            return t != null ? t.Type == TileType.WALL || t.Type == TileType.ONEWAY_PLATFORM : true;
        }

        public bool IsObastacleAtPos(Vector2f world_pos)
        {
            // TODO: make it elegant
            Vector2i map = ToMapPos(world_pos);
            cTile t = currentLevel.GetTileAtXY(map.X, map.Y);
            return t != null ? !t.IsWalkAble() : true;
        }

        public bool IsWallAtPos(Vector2f world_pos)
        {
            Vector2i map = ToMapPos(world_pos);
            TileType tt = GetCurrentLevel().GetTypeAtPos(map);
            return tt == TileType.WALL;
        }

        public cTile GetTileByWorldPos(Vector2f world_pos)
        {
            Vector2i map = ToMapPos(world_pos);
            return currentLevel.GetTileAtXY(map.X, map.Y);
        }

        public AABB getAABBFromMapPos(Vector2i mapPos)
        {
            Vector2f wp = this.ToWorldPos(mapPos);
            return new AABB(wp, new Vector2f(Constants.TILE_SIZE, Constants.TILE_SIZE));
        }

        public AABB getAABBFromWorldPos(Vector2f worldPos)
        {
            Vector2i mp = this.ToMapPos(worldPos);
            Vector2f wp = this.ToWorldPos(mp);
            return new AABB(wp, new Vector2f(Constants.TILE_SIZE, Constants.TILE_SIZE));
        }

        public AABB WorldBounds
        {
            get { return m_WorldBounds; }
        }

        public cMapData GetCurrentLevel()
        {
            return currentLevel;
        }

        public bool isRectOnWall(float left, float top, float width, float height)
        {
            Vector2i topLeft = this.ToMapPos(new Vector2f(left, top));
            Vector2i topRight = this.ToMapPos(new Vector2f(left+width, top));
            Vector2i bottomRight = this.ToMapPos(new Vector2f(left + width, top+height));
            Vector2i bottomLeft = this.ToMapPos(new Vector2f(left, top+height));

            return ( currentLevel.IsObstacleAtPos(topLeft) ||
                     currentLevel.IsObstacleAtPos(topRight) ||
                     currentLevel.IsObstacleAtPos(bottomRight) ||
                     currentLevel.IsObstacleAtPos(bottomLeft));
        }

        public void ClearAll()
        {
            currentLevel.Clear();
        }

        public void collideSAT(cGameObject obj, float step_time)
        {
            // check collisions with world
            List<AABB> wallsPossibleColliding = this.getCollidableBlocks(obj.Bounds);

            // we must check this, because we need to iterate through the possible
            // colliding tiles from other direction according to this condition
            if (obj.Velocity.X > 0.0f)
            {
                for (int i = 0; i < wallsPossibleColliding.Count; i++)
                {
                    cGameObject wallObject = cGameObject.MakeWall(wallsPossibleColliding[i]);
                    if (cSatCollision.checkAndResolve(obj, wallObject, step_time, true))
                    {
                        break;
                    }
                }
            }
            else
            {
                // we have to iterate from "end" to the "start" in order to have the last colliding block below us
                for (int i = wallsPossibleColliding.Count - 1; i >= 0; i--)
                {
                    cGameObject wallObject = cGameObject.MakeWall(wallsPossibleColliding[i]);
                    if (cSatCollision.checkAndResolve(obj, wallObject, step_time, true))
                    {
                        break;
                    }

                }
            }
        }

        public void collideParticleSAT(Particle particle, float step_time)
        {
            cGameObject obj = cGameObject.fromParticle(particle);
            this.collideSAT(obj, step_time);
            particle.Pos = obj.Position;
            particle.Vel = obj.Velocity;
        }

        public void collideParticleRayTrace(Particle p, float step_time)
        {
            Vector2i posA = new Vector2i((int)p.LastPos.X, (int)p.LastPos.Y);
            Vector2i posB = new Vector2i((int)p.Pos.X, (int)p.Pos.Y);
            bool collided = false;
            Vector2f intersectionPoint = new Vector2f(0.0f, 0.0f);

            cAppMath.Raytrace(posA.X, posA.Y, posB.X, posB.Y, new VisitMethod(
               (int x, int y) =>
               {
                   collided = this.IsObastacleAtPos(new Vector2f(x, y));
                   if (collided)
                   {
                       intersectionPoint.X = x;
                       intersectionPoint.Y = y;

                       p.Pos = intersectionPoint;
                       p.LastPos = p.Pos;
                       //if we want to drag to the wall:
                       p.Vel.X = 0.0f;
                       p.Vel.Y = 0.0f;

                       p.Intersects = true;
                       p.Life = 0.0f;
                   }

                   return collided;
               }
             )
           );
        }

        public List<AABB> getCollidableBlocks(AABB with)
        {
            List<AABB> boxes = new List<AABB>();

            const int offset = 1; //def: 1

            //Vector2i ctile = ToMapPos(with.center);
            Vector2i minTile = ToMapPos(with.topLeft);
            Vector2i maxTile = ToMapPos(with.rightBottom);
            
            //int lastTileID = -1;

            for (int y = minTile.Y - offset; y <= maxTile.Y + offset; y++)
            {
                for (int x = minTile.X - offset; x <= maxTile.X + offset; x++)
                {
                    if ( y >= 0 && x >= 0 && y < this.currentLevel.Height && x < this.currentLevel.Width )
                    {
                        cTile tile = this.GetCurrentLevel().GetTileAtXY(x, y);
                        
                        TileType tt = tile.Type;
                        //int tid = tile.IdCode;

                        if (/*lastTileID != tid &&*/ (tt == TileType.WALL || tt == TileType.ONEWAY_PLATFORM) )
                        {
                            // tile.PlayerCollidable = true;
                            //lastTileID = tid;
                            boxes.Add( this.getAABBFromMapPos(new Vector2i(x, y) ));
                        }
                        
                    }
                }
            }

            return boxes;
        }
    }
}
