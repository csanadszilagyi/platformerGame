using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace platformerGame.GameObjects.PickupInfo
{
    delegate void PickupEffectFunction(cPlayer player);

    class cPickupInfo
    {
        PickupEffectFunction effect;

        IntRect textureRect;

        public cPickupInfo(PickupEffectFunction effect)
        {
            this.effect = effect;
        }

        public void applyEffect(cPlayer player)
        {
            effect(player);
        }

        public IntRect TextureRect
        {
            get { return textureRect; }
        }
    }

    class PickupEffects
    {
        public static void AddHealth(cPlayer player)
        {
            // player.Health += 2;
        }
    }
}
