using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using platformerGame.Rendering;

namespace platformerGame.Effects
{
    class AnimatedEffect
    {
        public Vector2f Centre { get; set; } // centre
        public cAnimation Animation { get; set; }

        public AnimatedEffect()
        {
            this.Centre = new Vector2f();
            this.Animation = null;
        }
    }

    class EffectSystem
    {
        List<AnimatedEffect> effects;

        public EffectSystem()
        {
            effects = new List<AnimatedEffect>();
        }

        public void Place(AnimatedEffect effect)
        {
            this.effects.Add(effect);
        }

        public void Place(Vector2f centre, string animation_name)
        {
            var effect = new AnimatedEffect();
            effect.Animation = new cAnimation(cAnimationAssets.Get(animation_name));
            effect.Centre = centre;
            this.Place(effect);
        }

        public void PlaceGround(float x, float groundY, string animation_name)
        {
            var effect = new AnimatedEffect();
            effect.Animation = new cAnimation(cAnimationAssets.Get(animation_name));

            int h = effect.Animation.AnimData.ViewOffsetRect.Height;
            effect.Centre = new Vector2f(x, groundY - (h / 2.0f));
            this.Place(effect);
        }

        public void Update()
        {
            int eCount = effects.Count;
            for (int i = 0; i < eCount; ++i)
            {
                var e = effects[i];

                if (e.Animation.Active)
                {
                    e.Animation.Update();
                    continue;
                }

                effects.RemoveAt(i);
                i--;
                eCount = effects.Count;

            }
        }

        public void Render(RenderTarget destination)
        {
            foreach (var e in effects)
            {
                e.Animation.RenderCentered(destination, e.Centre);
            }
        }
    }
}
