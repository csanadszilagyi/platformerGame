using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using platformerGame.Utilities;

namespace platformerGame.GameObjects.PickupInfo
{
    delegate void PickupEffectFunction(cPlayer player);

    enum PickupType
    {
        UNKNOWN = -1,
        HEALTH = 0,
        ARMOR,
        AMMO
    }

    class cPickupInfo
    {
        PickupType type;
        PickupEffectFunction effect;
        IntRect textureRect;

        public cPickupInfo()
        {
            this.type = PickupType.UNKNOWN;
            this.textureRect = new IntRect(0,0,0,0);
            this.effect = null;
        }

        public cPickupInfo(PickupType type, PickupEffectFunction effect, IntRect texture_rect)
        {
            this.type = type;
            this.textureRect = texture_rect;
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

        public IntRect TextureRect
        {
            get { return textureRect; }
        }

        public PickupType Type
        {
            get { return type; }
        }

        public PickupEffectFunction Effect
        {
            get { return effect; }
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
               [PickupType.HEALTH] = new cPickupInfo(PickupType.HEALTH, AddHealth, new IntRect(0,0,24,24)),
               [PickupType.ARMOR] = new cPickupInfo(PickupType.ARMOR, AddArmor, new IntRect(0,24,24,24)),
               [PickupType.AMMO] = new cPickupInfo(PickupType.AMMO, AddAmmo, new IntRect(24,0,24,24))
            };

            pickupProbabilityTable = new Tuple<int, PickupType>[3]
            {
                new Tuple<int, PickupType>(50, PickupType.HEALTH),
                new Tuple<int, PickupType>(25, PickupType.ARMOR),
                new Tuple<int, PickupType>(25, PickupType.AMMO)
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
            player.Health += 2;
        }

        private static void AddArmor(cPlayer player)
        {
            player.Health += 2;
        }

        private static void AddAmmo(cPlayer player)
        {
            player.Health += 2;
        }
    }
}
