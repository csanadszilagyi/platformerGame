using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame.Rendering
{
    class cSpriteRenderer : cBaseRenderer
    {
        Sprite sprite;

        public cSpriteRenderer(string texture_name, IntRect texture_rect) : base()
        {
            this.sprite = new Sprite(AssetManager.GetTexture(texture_name));

            this.sprite.TextureRect = texture_rect;
            //this.sprite.Scale = new Vector2f(0.5f, 0.5f);
            //this.sprite.Rotation = (float)cAppMath.RadianToDegress(this.orientation);

            this.sprite.Origin = new Vector2f(texture_rect.Width / 2.0f, texture_rect.Height / 2.0f);
        }

        protected cSpriteRenderer(cSpriteRenderer other) : base(other)
        {
            this.sprite = new Sprite(other.sprite);
            //this.sprite.TextureRect = other.sprite.TextureRect;
        }

        public override cBaseRenderer DeepCopy()
        {
            return new cSpriteRenderer(this);
        }

        public override void Update(float step_time)
        {
            // empty function
        }

        public override void Draw(RenderTarget destination, Vector2f pos)
        {
            sprite.Position = pos;
            destination.Draw(sprite, renderStates);
        }
    }
}
