using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using platformerGame.Utilities;

namespace platformerGame.Map
{
    class cTileMapRenderer
    {
        cWorld world;

        List<cTileLayer> drawTileLayers;

        AABB drawTileBounds;

        Texture tileSetTexture;

        public cTileMapRenderer(cWorld world)
        {
            this.world = world;
            this.drawTileLayers = new List<cTileLayer>();

            this.tileSetTexture = world.CurrentLevel.TilesetTexture;

            drawTileBounds = new AABB();

            this.setupLayers();

            // textureOfTiles = new RenderTexture((uint)m_WorldBounds.dims.X, (uint)m_WorldBounds.dims.Y); //(windowSize.X, windowSize.Y);
            // textureOfTiles.SetActive(true);

        }

        private void setupLayers()
        {
            drawTileLayers.Add(new cTileLayer(TileLayerTypes.BACKGROUND_1, tileSetTexture));
            drawTileLayers.Add(new cTileLayer(TileLayerTypes.BACKGROUND_2, tileSetTexture));
            drawTileLayers.Add(new cTileLayer(TileLayerTypes.WALLS, tileSetTexture));
        }

        public void ClearTileLayers()
        {
            foreach (var layer in drawTileLayers)
            {
                layer.ClearVertexArray();
            }
        }

        /// <summary>
        /// Filters the visible tiles and build the vertex arrays of each layer
        /// </summary>
        private void filterDrawableTiles()
        {
            int startTileX = (int)drawTileBounds.topLeft.X;
            int startTileY = (int)drawTileBounds.topLeft.Y;
            int endTileX = (int)drawTileBounds.rightBottom.X;
            int endTileY = (int)drawTileBounds.rightBottom.Y;

            // important to call !!
            this.ClearTileLayers();


            Vector2f drawPos = new Vector2f();

            const int spaceOffset = 0;

            for (int y = startTileY; y < endTileY; y++)
            {
                for (int x = startTileX; x < endTileX; x++)
                {
                    for (int layer = 0; layer < drawTileLayers.Count; layer++)
                    {
                        cTile tempTile = world.CurrentLevel.GetTileAtXY(x, y, drawTileLayers[layer].TypeID);

                        if (tempTile.Type != TileType.EMPTY)
                        {
                            // tilesToDraw.Add(new DrawTile(x, y, tempTile.PosOnTexture));

                            drawPos.X = (world.WorldBounds.topLeft.X + x * Constants.TILE_SIZE) + spaceOffset;
                            drawPos.Y = (world.WorldBounds.topLeft.Y + y * Constants.TILE_SIZE) + spaceOffset;

                            AABB tileBounds = world.getAABBFromWorldPos(drawPos);

                            drawTileLayers[layer].AddTileVertices(tileBounds, tempTile.PosOnTexture);
                        }
                    }
                }
            }

        }

        public void RecalculateDrawBounds(AABB viewRect)
        {
            drawTileBounds = viewRect.ShallowCopy();

            drawTileBounds.topLeft.X = (float)Math.Floor((drawTileBounds.topLeft.X / Constants.TILE_SIZE) - 1);
            drawTileBounds.topLeft.Y = (float)Math.Floor((drawTileBounds.topLeft.Y / Constants.TILE_SIZE) - 1);

            drawTileBounds.rightBottom.X = (float)Math.Ceiling((drawTileBounds.rightBottom.X / Constants.TILE_SIZE) + 1);
            drawTileBounds.rightBottom.Y = (float)Math.Ceiling((drawTileBounds.rightBottom.Y / Constants.TILE_SIZE) + 1);

            // TODO comment
            drawTileBounds.topLeft.X = Math.Max(drawTileBounds.topLeft.X, 0.0f);
            drawTileBounds.topLeft.Y = Math.Max(drawTileBounds.topLeft.Y, 0.0f);
            drawTileBounds.rightBottom.X = Math.Min(drawTileBounds.rightBottom.X, this.world.CurrentLevel.Width);
            drawTileBounds.rightBottom.Y = Math.Min(drawTileBounds.rightBottom.Y, this.world.CurrentLevel.Height);

            filterDrawableTiles();

        }
        public void Render(RenderTarget destination, AABB view_rect = null)
        {
            foreach (var layer in drawTileLayers)
            {
                layer.Draw(destination);
            }

        }
    }
}
