using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

using platformerGame.Utilities;

namespace platformerGame
{
    class cEnvironmentItem
    {
        protected Sprite sprite;
        protected cAABB bounds; //may differ from drawing bounds
        
        public cEnvironmentItem(Texture texture)
        {
            sprite = new Sprite(texture);
            bounds = new cAABB();
        }

        public cEnvironmentItem(Vector2f pos, Texture texture, cAABB _bounds = null)
        {
            sprite = new Sprite(texture);
            sprite.Position = pos;
            bounds = _bounds ?? new cAABB(pos, new Vector2f(texture.Size.X, texture.Size.Y));
        }

        public Sprite Sprite
        {
            get { return sprite; }
            //set { sprite = value; }
        }

        public cAABB Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }
    }
    class cEnvironment
    {
        List<cEnvironmentItem> envItems;

        List<cWaterBlock> waterBlocks;

        public cEnvironment()
        {
            envItems = new List<cEnvironmentItem>();
            
        }

        public void SetWaterBlocks(List<cWaterBlock> wb_from_map)
        {
            waterBlocks = wb_from_map;
        }
        public void PlaceItem(Vector2f centre_pos, Texture texture)
        {
            //TODO: set pos by centre
            /*
            cEnvironmentItem item = new cEnvironmentItem();
            envItems.Add(item);
            */
        }

        public void PlaceOnGround(Vector2f on_ground_pos, Texture texture)
        {
            float x = on_ground_pos.X - texture.Size.X / 2.0f;
            float y = on_ground_pos.Y + Constants.TILE_SIZE - texture.Size.Y;

            cEnvironmentItem item = new cEnvironmentItem(new Vector2f(x,y), texture);
            envItems.Add(item);
        }

        /// <summary>
        /// Calculates bounding box for an item on the ground by giving its texture size.
        /// </summary>
        /// <param name="on_ground_pos"></param>
        /// <param name="texture_size"></param>
        /// <returns></returns>
        public cAABB CalcBBOnGroundByTexture(Vector2f on_ground_pos, Vector2f texture_size)
        {
            float x = on_ground_pos.X - texture_size.X / 2.0f;
            float y = on_ground_pos.Y + Constants.TILE_SIZE - texture_size.Y;

            return new cAABB(x, y, texture_size.X, texture_size.Y);
        }
        public void PlaceOnGround(Vector2f texture_pos, Texture texture, cAABB bounding_rect)
        {

            cEnvironmentItem item = new cEnvironmentItem(texture_pos, texture, bounding_rect);
            envItems.Add(item);
        }


        public void ClearAll()
        {
            envItems.Clear();
            waterBlocks.Clear();
        }
        public void Update(float step_time)
        {
            foreach (cWaterBlock wb in waterBlocks)
            {
                wb.Update(step_time);
            }
        }

        public void RenderWaterBlocks(RenderTarget destination)
        {

            foreach (cWaterBlock wb in waterBlocks)
            {
                wb.Render(destination);
            }
        }
        public void RenderEnvironment(RenderTarget destination)
        {
           
            RenderStates rs = new RenderStates(BlendMode.Alpha);

            foreach (cEnvironmentItem item in envItems)
            {

                //destination.Draw(item.Sprite, rs);
                #if DEBUG
                RectangleShape rect = new RectangleShape();
                rect.Position = item.Bounds.topLeft;
                rect.Size = item.Bounds.dims;
                rect.OutlineColor = Color.Green;
                rect.OutlineThickness = 2.0f;
                rect.FillColor = Color.Transparent;

                destination.Draw(rect, rs);

                #else
                destination.Draw(item.Sprite, rs); 
                #endif
            }
        }
    }
}
