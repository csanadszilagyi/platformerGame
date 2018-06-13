using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;


namespace platformerGame.Rendering
{
    class cAnimatedSpriteRenderer : cBaseRenderer
    {
        cAnimation animation;

        public cAnimatedSpriteRenderer(string sprite_id, IntRect view_offset_rect = new IntRect()) : base()
        {
            AnimationInfo info = cAnimationAssets.Get(sprite_id);
            animation = new cAnimation( info, view_offset_rect);
        }

        protected cAnimatedSpriteRenderer(cAnimatedSpriteRenderer other) : base(other)
        {
            this.animation = new cAnimation(other.animation.GetAnimInfo(), other.animation.GetViewOffsetRect());
        }

        // by: https://stackoverflow.com/questions/19119623/clone-derived-class-from-base-class-method
        public override cBaseRenderer DeepCopy()
        {
            return new cAnimatedSpriteRenderer(this);
        }

        public override void Update(float step_time)
        {
            animation.Update();
        }

        public override void Draw(RenderTarget destination, Vector2f pos)
        {
            animation.RenderCentered(destination, pos);
        }
    }
}
