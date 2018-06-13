using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace platformerGame.Map
{
    class cTileLayer
    {
        // TileLayerType
        int typeID;

        VertexArray vertices;

        RenderStates renderStates;

        public cTileLayer(int type_id, Texture tile_set)
        {
            this.typeID = type_id;
            this.vertices = new VertexArray(PrimitiveType.Quads);
            this.renderStates = new RenderStates(BlendMode.Alpha);
            this.renderStates.Texture = tile_set;
        }

        public void ClearVertexArray()
        {
            this.vertices.Clear();
        }

        public void AddTileVertices(cAABB tile_region, cAABB pos_in_tileset)
        {
            vertices.Append(new Vertex(new Vector2f(tile_region.topLeft.X, tile_region.topLeft.Y), Color.White, new Vector2f(pos_in_tileset.topLeft.X, pos_in_tileset.topLeft.Y)));
            vertices.Append(new Vertex(new Vector2f(tile_region.getTopRight().X, tile_region.getTopRight().Y), Color.White, new Vector2f(pos_in_tileset.getTopRight().X, pos_in_tileset.getTopRight().Y)));
            vertices.Append(new Vertex(new Vector2f(tile_region.rightBottom.X, tile_region.rightBottom.Y), Color.White, new Vector2f(pos_in_tileset.rightBottom.X, pos_in_tileset.rightBottom.Y)));
            vertices.Append(new Vertex(new Vector2f(tile_region.getLeftBottom().X, tile_region.getLeftBottom().Y), Color.White, new Vector2f(pos_in_tileset.getLeftBottom().X, pos_in_tileset.getLeftBottom().Y)));
        }

        public void Draw(RenderTarget destination)
        {
            destination.Draw(vertices, renderStates);
        }

        public int TypeID
        {
            get { return typeID; }
        }
    }
}
