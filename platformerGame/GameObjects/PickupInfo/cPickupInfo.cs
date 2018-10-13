using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using platformerGame.Utilities;
using platformerGame.Rendering;

namespace platformerGame.GameObjects.PickupInfo
{
    delegate void PickupEffectFunction(cPlayer player);

    enum PickupType
    {
        UNKNOWN = -1,
        HEALTH = 0,
        ARMOR,
        AMMO,
        COIN_GOLD,
        COIN_SILVER,
        COIN_IRON
    }


    class cPickupInfo
    {
        PickupType pickupType;
        cBaseRenderer renderer;
        Vector2f hitRectSize;
        PickupEffectFunction effect;
        

        public cPickupInfo()
        {
            this.pickupType = PickupType.UNKNOWN;
            this.renderer = null;
            this.hitRectSize = new Vector2f(0, 0);
            this.effect = null;
        }

        public cPickupInfo(PickupType pickup_type, cBaseRenderer renderer, Vector2f hit_rect_size, PickupEffectFunction effect)
        {
            this.pickupType = pickup_type;
            this.renderer = renderer;
            this.hitRectSize = hit_rect_size;
            this.effect = effect;
        }

        public void applyEffect(cPlayer player)
        {
            effect(player);
        }

        public cPickupInfo ShallowCopy()
        {
            return (cPickupInfo)this.MemberwiseClone();
        }

        /*
        public cPickupInfo DeepCopy()
        {
            cPickupInfo copy = this.ShallowCopy();

            return copy;
        }
        */

        public PickupType PickupType
        {
            get { return pickupType; }
        }

        public cBaseRenderer Renderer
        {
            get { return renderer; }
        }

        public PickupEffectFunction Effect
        {
            get { return effect; }
        }

        public Vector2f HitRectSize
        {
           get { return this.hitRectSize; }
        }
    }

    class PickupEffects
    {

        private static Dictionary<PickupType, cPickupInfo> pickupTypes;

        private static ProbabilityRoll<PickupType> roller;

        private static Tuple<int, PickupType>[] pickupProbabilityTable;

        static PickupEffects()
        {
            /*
            int[] tcoords = { 0, 24, 48 };
            int tx = cAppRandom.Chooose<int>(tcoords);
            int ty = cAppRandom.Chooose<int>(tcoords, 2);
            */
            pickupTypes = new Dictionary<PickupType, cPickupInfo>()
            {
               [PickupType.HEALTH] = new cPickupInfo(PickupType.HEALTH, new cSpriteRenderer("pickups", new MyIntRect(0, 0, 24, 24)), new Vector2f(16, 22), AddHealth),
               [PickupType.ARMOR] = new cPickupInfo(PickupType.ARMOR, new cSpriteRenderer("pickups", new MyIntRect(0, 24, 24, 24)), new Vector2f(16, 22), AddArmor),
               [PickupType.AMMO] = new cPickupInfo(PickupType.AMMO, new cSpriteRenderer("pickups", new MyIntRect(24, 0, 24, 24)), new Vector2f(16, 22), AddAmmo),
               [PickupType.COIN_GOLD] = new cPickupInfo(PickupType.COIN_GOLD, new cAnimatedSpriteRenderer("coins-gold", new MyIntRect(0, 0, 16, 16), true), new Vector2f(16, 16), AddCoin),
               [PickupType.COIN_SILVER] = new cPickupInfo(PickupType.COIN_SILVER, new cAnimatedSpriteRenderer("coins-silver", new MyIntRect(0, 0, 16, 16), true), new Vector2f(16, 16), AddCoin),
               [PickupType.COIN_IRON] = new cPickupInfo(PickupType.COIN_IRON, new cAnimatedSpriteRenderer("coins-copper", new MyIntRect(0, 0, 16, 16), true), new Vector2f(16, 16), AddCoin)
            };

            pickupProbabilityTable = new Tuple<int, PickupType>[4]
            {
                new Tuple<int, PickupType>(50, PickupType.HEALTH),
                new Tuple<int, PickupType>(5, PickupType.ARMOR),
                new Tuple<int, PickupType>(5, PickupType.AMMO),
                new Tuple<int, PickupType>(40, PickupType.COIN_GOLD)
            };

            roller = new ProbabilityRoll<PickupType>();
            roller.seed(pickupProbabilityTable);
        }

        public static cPickupInfo get(PickupType type)
        {
            return pickupTypes[type].ShallowCopy();
        }

        public static cPickupInfo getWeighted()
        {
            return get( roller.roll() ).ShallowCopy();
        }

        private static void AddHealth(cPlayer player)
        {
            player.Health += 1;
        }

        private static void AddArmor(cPlayer player)
        {
            player.Health += 1;
        }

        private static void AddAmmo(cPlayer player)
        {
            player.Health += 1;
        }

        private static void AddCoin(cPlayer player)
        {
            AssetManager.playSound("coin_pickup1", 10);
            // TODO: player.Money += 1;
        }
    }
}
