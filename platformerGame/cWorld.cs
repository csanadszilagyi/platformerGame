using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

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

        cMapData m_Level1;
        cAABB m_WorldBounds;
        Vector2u windowSize;

        cAABB drawTileBounds;

        RenderTexture m_TextureOfTiles;

        Texture m_TileSetTexture = null;

        Sprite background;
        Texture m_BGtexture = null;

        public cAABB levelStartRegion;
        public cAABB levelEndRegion;

        cGameScene pScene;
        
        public cWorld(cGameScene p_scene, Vector2u window_size)
        {
            pScene = p_scene;
            windowSize = window_size;
            drawTileBounds = new cAABB(0,0,1,1);

            m_Level1 = new cMapData();
            //m_Level1.Create(100, 100);
            this.LoadLevel("levels/level1.txt");  //("levels/level1.txt");

            m_TextureOfTiles = new RenderTexture((uint)m_WorldBounds.dims.X, (uint)m_WorldBounds.dims.Y); //(windowSize.X, windowSize.Y);
            m_TextureOfTiles.SetActive(true);

            m_TileSetTexture = cAssetManager.GetTexture("tileSet_16");
            m_TileSetTexture.Smooth = true;

            m_BGtexture = cAssetManager.GetTexture(Constants.BG_TEXTURE);
            m_BGtexture.Repeated = true;
            m_BGtexture.Smooth = true;

            background = new Sprite(m_BGtexture);
            background.TextureRect = new IntRect(0, 0, (int)m_WorldBounds.dims.X, (int)m_WorldBounds.dims.Y); // (int)m_TextureOfTiles.Size.X, (int)m_TextureOfTiles.Size.Y);
            background.Color = Constants.BACKGROUND_COLOR;

            
        }

        public void LoadLevel(string file_name)
        {
            m_Level1.LoadFromFile(file_name);

            m_WorldBounds = new cAABB(0.0f, 0.0f, m_Level1.Width * Constants.TILE_SIZE, m_Level1.Height * Constants.TILE_SIZE);

            initTileSprites();
        }
        public cAABB LevelStartRegion
        {
            get { return levelStartRegion; }
        }

        public cAABB LevelEndRegion
        {
            get { return levelEndRegion; }
        }

        /// <summary>
        /// Négyzetes mintájú vízeket azonosítja
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

                while (h < m_Level1.Height)
                {

                    cTile currentTile = m_Level1.GetTileAtXY(w, h);
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
                            tile = m_Level1.GetTileAtXY(w, h + heightOffset);
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
                            waters.Add(new cWaterBlock(new cAABB(topLeft, new Vector2f(Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y)))));
                            wasWater = false;
                            h = lastH;
                        }
                    }

                    w++;

                    if (w >= m_Level1.Width)
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

        /// <summary>
        /// Itt választjuk ki és állítjuk be előre a kirjazoláshoz, hogy melyik csempéhez milyen al-textúra tartozik (több féle fal van, attól függ, hol és hány másik fal kapcsolódik az adott falhoz)
        /// </summary>
    private void initTileSprites()
        {
            cTile tempTile = null;
            Vector2i tilePos = new Vector2i();
            Vector2f tileCenterTop = new Vector2f();

            for (int y = 0; y < m_Level1.Height; y++)
            {
                for (int x = 0; x < m_Level1.Width; x++)
                {
                    tempTile = m_Level1.GetTileAtXY(x, y);
                    tilePos.X = x;
                    tilePos.Y = y;

                    tileCenterTop = this.ToWorldPos(tilePos);
                    tileCenterTop.X += Constants.TILE_SIZE_HALF;
                    tileCenterTop.Y += Constants.TILE_SIZE_HALF;

                    tempTile.PosOnTexture = TileMask.NONE;

                    if (tempTile.Type == TileType.WALL)
                    {
                        NeighInfo info = getNumOfLinearNeighsByCode(m_Level1, tilePos, TileType.WALL);
                        
                        if(info.NumNeighs == 1)
                        {
                           /* cLight l = cLightSystem.GetEnvironLight(tileCenterTop, 32.0f, Color.White);
                            l.Bleed = 1.6f;
                            l.LinearizeFactor = 0.4f;
                            l.Color = new Color(246, 205, 105, 255);
                            pScene.LightMap.AddStaticLight(l);*/
                        }

                        if (info.isAlone())
                        {
                            tempTile.PosOnTexture = TileMask.ALONE_BOT_WITH_TOP; // alone;

                           /* cLight l = cLightSystem.GetEnvironLight(tileCenterTop, 32.0f, Color.White);
                            l.Bleed = 1.6f;
                            l.LinearizeFactor = 0.4f;
                            l.Color = new Color(246, 205, 105, 255);
                            pScene.LightMap.AddStaticLight(l);*/
                        }
                        else
                        {
                            //egyedül van-e

                            if (!info.HasTop)
                            {
                                if (!info.HasBottom)
                                {
                                    if (!info.HasLeft)
                                        tempTile.PosOnTexture = TileMask.NO_TOPBOT_LEFT;
                                    else
                                    if (!info.HasRight)
                                        tempTile.PosOnTexture = TileMask.NO_TOPBOT_RIGHT;
                                    else
                                        tempTile.PosOnTexture = TileMask.NO_TOPBOT_MIDDLE;
                                }
                                else
                                {
                                    if (info.HasLeft)
                                    {
                                        if (info.HasRight)
                                            tempTile.PosOnTexture = TileMask.TOP_MIDDLE;
                                        else
                                            tempTile.PosOnTexture = TileMask.TOP_RIGHT;
                                    }
                                    else
                                    {
                                        if (info.HasRight)
                                            tempTile.PosOnTexture = TileMask.TOP_LEFT;
                                        else
                                            tempTile.PosOnTexture = TileMask.ALONE_TOP_WITH_BOT;
                                    }

                                }
                            }
                            else
                            if (!info.HasBottom)
                            {
                                if (info.HasLeft)
                                {
                                    if (info.HasRight)
                                        tempTile.PosOnTexture = TileMask.BOTTOM_MIDDLE;
                                    else
                                        tempTile.PosOnTexture = TileMask.BOTTOM_RIGHT;
                                }
                                else
                                {
                                    if (info.HasRight)
                                        tempTile.PosOnTexture = TileMask.BOTTOM_LEFT;
                                    else
                                        tempTile.PosOnTexture = TileMask.ALONE_BOT_WITH_TOP;
                                }
                            }
                            else
                            if (!info.HasLeft)
                            {
                                if (info.HasRight)
                                    tempTile.PosOnTexture = TileMask.MIDDLE_LEFT;
                                else
                                    tempTile.PosOnTexture = TileMask.ALONE;
                            }
                            else
                            if (!info.HasRight)
                            {
                                tempTile.PosOnTexture = TileMask.MIDDLE_RIGHT;

                            }
                            else
                            {
                                tempTile.PosOnTexture = TileMask.MIDDLE_CENTRE;
                                //tempTile.PosOnTexture = TileMask.EMPTY; //looks weird
                            }
                        }
                        
                    }
                    else
                    if(tempTile.Type == TileType.EMPTY)
                    {
                       
                    }
                    else
                    if (tempTile.Type == TileType.ONEWAY_PLATFORM)
                    {
                        tempTile.PosOnTexture = TileMask.ONE_WAY_PLATFORM;
                        /*
                        cLight l = cLightSystem.GetEnvironLight(tileCenterTop, 32.0f, Color.White);
                        l.Bleed = 2.6f;
                        l.LinearizeFactor = 0.3f;
                        pScene.LightMap.AddStaticLight(l);*/

                        //pScene.LightMap.AddStaticLight(cLightSystem.GetEnvironLight(this.ToWorldPos(tilePos), 50.0f, Color.Yellow));

                    }
                    else
                    if (tempTile.Type == TileType.LEVEL_START)
                    {
                        //top-left
                        Vector2f levelStartGroundPos = this.ToWorldPos(new Vector2i(x, y));
                        
                        Texture t = cAssetManager.GetTexture("door1_sm");

                        Vector2f textureSize = new Vector2f(t.Size.X, t.Size.Y);

                        cAABB bounds = pScene.WolrdEnv.CalcBBOnGroundByTexture(levelStartGroundPos, textureSize);
                        Vector2f texturePos = bounds.topLeft;

                        bounds.Scale(new Vector2f(0.8f, 0.8f), new Vector2f(bounds.center.X, bounds.rightBottom.Y));

                        levelStartRegion = bounds;

                        pScene.WolrdEnv.PlaceOnGround(texturePos, t, bounds);
                        
                        
        
                        pScene.LightMap.AddStaticLight( cLightSystem.GetEnvironLight(bounds.center, bounds.halfDims.X * 3.0f, Color.Green) );

                    }
                    else
                    if (tempTile.Type == TileType.LEVEL_END)
                    {
                        
                        //top-left
                        Vector2f levelEndGroundPos = this.ToWorldPos(new Vector2i(x, y));

                        Texture t = cAssetManager.GetTexture("door1_sm");

                        Vector2f textureSize = new Vector2f(t.Size.X, t.Size.Y);

                        cAABB bounds = pScene.WolrdEnv.CalcBBOnGroundByTexture(levelEndGroundPos, textureSize);
                        Vector2f texturePos = bounds.topLeft;

                        bounds.Scale(new Vector2f(0.8f, 0.8f), new Vector2f(bounds.center.X, bounds.rightBottom.Y));

                        levelEndRegion = bounds;

                        pScene.WolrdEnv.PlaceOnGround(texturePos, t, bounds);


                        pScene.LightMap.AddStaticLight(cLightSystem.GetEnvironLight(bounds.center, bounds.halfDims.X * 3.0f, Color.Red));
                    }

                }

            }
        }

        public void DrawBackground(RenderTarget destination)
        {
            destination.Draw(background);
        }
        private void renderTilesToTexture(RenderTarget destination)
        {
           
            //m_TextureOfTiles.Clear(Color.Black);

            

            cTile tempTile = null;
            Vector2f drawPos = new Vector2f();

            const uint spaceOffset = 0;

            Sprite tempSprite = new Sprite(m_TileSetTexture);
           

            const int subTILE_SIZE = 32; //40
                                         //const int diff = (TILE_SIZE > subTILE_SIZE) ? (TILE_SIZE - subTILE_SIZE) : 0;
            int startTileX = (int)drawTileBounds.topLeft.X;
            int startTileY = (int)drawTileBounds.topLeft.Y;
            int endTileX = (int)drawTileBounds.rightBottom.X;
            int endTileY = (int)drawTileBounds.rightBottom.Y;
            //m_TextureOfTiles.Draw(m_BcgrSprite);
            Vector2f tempSize = new Vector2f(Constants.TILE_SIZE, Constants.TILE_SIZE);


            for (int y = startTileY; y < endTileY; y++)
            {
                for (int x = startTileX; x < endTileX; x++)
                {
                    tempTile = m_Level1.GetTileAtXY(x, y);

                    drawPos.X = (m_WorldBounds.topLeft.X + x * Constants.TILE_SIZE) + spaceOffset;
                    drawPos.Y = (m_WorldBounds.topLeft.Y + y * Constants.TILE_SIZE) + spaceOffset;


                    //tempRS.setPosition(drawPos);
                    if (tempTile.Type != TileType.EMPTY)
                    {
                        if (tempTile.PlayerCollidable)
                            tempSprite.Color = Color.Red;
                        else
                            tempSprite.Color = Color.White;

                        tempSprite.Position = drawPos;
                        tempSprite.TextureRect = tempTile.PosOnTexture;

                        destination.Draw(tempSprite);
                    }

                }
                
            }

            //m_TextureOfTiles.Display();
        }

        private void recalculateDrawBounds(cAABB viewRect)
        {
            drawTileBounds = viewRect.ShallowCopy();
            
            drawTileBounds.topLeft.X = (float)Math.Floor((drawTileBounds.topLeft.X / Constants.TILE_SIZE)-1);
            drawTileBounds.topLeft.Y = (float)Math.Floor((drawTileBounds.topLeft.Y / Constants.TILE_SIZE)-1);

            drawTileBounds.rightBottom.X = (float)Math.Ceiling((drawTileBounds.rightBottom.X / Constants.TILE_SIZE)+1);
            drawTileBounds.rightBottom.Y = (float)Math.Ceiling((drawTileBounds.rightBottom.Y / Constants.TILE_SIZE)+1);

            drawTileBounds.topLeft.X = Math.Max(drawTileBounds.topLeft.X, 0.0f);
            drawTileBounds.topLeft.Y = Math.Max(drawTileBounds.topLeft.Y, 0.0f);
            drawTileBounds.rightBottom.X = Math.Min(drawTileBounds.rightBottom.X, m_Level1.Width);
            drawTileBounds.rightBottom.Y = Math.Min(drawTileBounds.rightBottom.Y, m_Level1.Height);

        }
        public void Render(RenderTarget destination, cAABB view_rect)
        {
            recalculateDrawBounds(view_rect);

            renderTilesToTexture(destination);
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
            return m_Level1.GetTileAtXY(map.X, map.Y).Type;
        }
        public bool IsObastacleAtPos(Vector2f world_pos)
        {
            Vector2i map = ToMapPos(world_pos);
            return !m_Level1.GetTileAtXY(map.X, map.Y).IsWalkAble();
        }
        public cTile GetTileByWorldPos(Vector2f world_pos)
        {
            Vector2i map = ToMapPos(world_pos);
            return m_Level1.GetTileAtXY(map.X, map.Y);
        }

        public cAABB getAABBFromMapPos(Vector2i mapPos)
        {
            Vector2f wp = this.ToWorldPos(mapPos);
            return new cAABB(wp, new Vector2f(Constants.TILE_SIZE, Constants.TILE_SIZE));
        }

        public cAABB getAABBFromWorldPos(Vector2f worldPos)
        {
            Vector2i mp = this.ToMapPos(worldPos);
            Vector2f wp = this.ToWorldPos(mp);
            return new cAABB(wp, new Vector2f(Constants.TILE_SIZE, Constants.TILE_SIZE));
        }

        public cAABB WorldBounds
        {
            get { return m_WorldBounds; }
        }

        public cMapData GetCurrentLevel()
        {
            return m_Level1;
        }

        public bool isRectOnWall(float left, float top, float width, float height)
        {
            Vector2i topLeft = this.ToMapPos(new Vector2f(left, top));
            Vector2i topRight = this.ToMapPos(new Vector2f(left+width, top));
            Vector2i bottomRight = this.ToMapPos(new Vector2f(left + width, top+height));
            Vector2i bottomLeft = this.ToMapPos(new Vector2f(left, top+height));

            return ( m_Level1.IsObstacleAtPos(topLeft) ||
                     m_Level1.IsObstacleAtPos(topRight) ||
                     m_Level1.IsObstacleAtPos(bottomRight) ||
                     m_Level1.IsObstacleAtPos(bottomLeft));
        }

        public void ClearAll()
        {
            m_Level1.Clear();
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

                       p.Life = 0.0f;
                   }

                   return collided;
               }
             )
           );

           // cAABB aabb = this.getAABBFromWorldPos(intersectionPoint);

           // if (collided)
           // {
                /*Vector2f N = cAppMath.Vec2Perp(p.Vel);
                if(intersectionPoint.Y == aabb.topLeft.Y)
                {
                    N = cAppMath.Vec2Perp(new Vector2f(aabb.rightBottom.X, aabb.topLeft.Y) - aabb.topLeft);
                }
                else
                if (intersectionPoint.Y == aabb.rightBottom.Y)
                {
                    N = cAppMath.Vec2Perp( - aabb.topLeft);
                }
                */
                //p.Pos = intersectionPoint; // + (-p.Heading);
                //p.LastPos = p.Pos;
                //if we want to drag to the wall:
                //p.Vel.X = 0.0f;
                //p.Vel.Y = 0.0f;
                //p.Vel = cCollision.processWorldObjCollision(p.Vel, N);
           // }

        }

        public void collideParticle(Particle p, float step_time)
        {
            Vector2f posA = new Vector2f((int)p.LastPos.X, (int)p.LastPos.Y);
            Vector2f posB = new Vector2f((int)p.Pos.X, (int)p.Pos.Y);
            bool collided = false;
            Vector2f intersectionPoint = new Vector2f(0.0f, 0.0f);

            List<cAABB> blocks = this.getCollidableBlocks(p.getBoundingBox());
            int i = 0;
            while(i < blocks.Count && !collided)
            {
                collided = cCollision.testLineVsAABB(p.LastPos, p.Pos, blocks[i], ref intersectionPoint);
                i++;
            }
           
            if(collided)
            {
                //Vector2f N = cAppMath.Vec2Perp(p.Vel);

                p.Pos = intersectionPoint; // + (-p.Heading);
                p.LastPos = p.Pos;
                p.Vel.X = 0.0f;
                p.Vel.Y = 0.0f;
            }
        }
        public List<cAABB> getCollidableBlocks(cAABB with)
        {
            List<cAABB> boxes = new List<cAABB>();

            const int offset = 2; //def: 1

            int minTileX = (int)(with.topLeft.X / Constants.TILE_SIZE) - offset;
            int minTileY = (int)(with.topLeft.Y / Constants.TILE_SIZE) - offset;

            int maxTileX = (int)(with.rightBottom.X / Constants.TILE_SIZE) + offset;
            int maxTileY = (int)(with.rightBottom.Y / Constants.TILE_SIZE) + offset;

            for (int y = minTileY; y <= maxTileY; y++)
            {
                for (int x = minTileX; x <= maxTileX; x++)
                {
                    if ( y >= 0 && x >= 0 && y < this.m_Level1.Height && x < this.m_Level1.Width && !this.m_Level1.GetTileAtXY(x, y).IsWalkAble())
                    {
                      
                        boxes.Add(this.getAABBFromMapPos(new Vector2i(x, y)));
                        
                    }
                }
            }

            return boxes;
        }
    }
}
