using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using platformerGame.Utilities;

namespace platformerGame.Rendering
{
    class cAnimatedSpriteRenderer : cBaseRenderer
    {
        public cAnimation AnimationSrc { get; set; }
        
        public cAnimatedSpriteRenderer(string sprite_id, MyIntRect view_offset_rect = null, bool repeat = false) : base()
        {
            AnimationInfo info = cAnimationAssets.Get(sprite_id);

            if(view_offset_rect != null)
            {
                info.ViewOffsetRect = view_offset_rect;
            }

            info.Repeat = repeat;
            AnimationSrc = new cAnimation(info);
        }

        protected cAnimatedSpriteRenderer(cAnimatedSpriteRenderer other) : base(other)
        {
            this.AnimationSrc = new cAnimation(other.AnimationSrc.AnimData);
        }

        // by: https://stackoverflow.com/questions/19119623/clone-derived-class-from-base-class-method
        public override cBaseRenderer DeepCopy()
        {
            return new cAnimatedSpriteRenderer(this);
        }

        public override void Update(float step_time)
        {
            AnimationSrc.Update();
        }

        public override void Draw(RenderTarget destination, Vector2f pos)
        {
            AnimationSrc.RenderCentered(destination, pos);
        }
    }
}
