using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    enum HorizontalFacing
    {
        FACING_LEFT = 2,
        FACING_RIGHT
    };

    enum MotionType
    {
        STAND = 1,
        WALK,
        JUMP,
        FALL,
        LIE
    };

    class cSpriteState
    {
        private MotionType motionType;
        private HorizontalFacing horizontalFacing;

        public HorizontalFacing HorizontalFacing
        {
            get { return horizontalFacing; }
            set { horizontalFacing = value; }
        }

        public MotionType MotionType
        {
            get { return motionType; }
            set { motionType = value; }
        }
        public cSpriteState(MotionType motion, HorizontalFacing h_facing)
        {
            motionType = motion;
            horizontalFacing = h_facing;
        }

        public cSpriteState ShallowCopy()
        {
            return (cSpriteState)this.MemberwiseClone();
        }
        public static bool operator ==(cSpriteState a, cSpriteState b)
        {
            return (a.MotionType == b.MotionType && a.HorizontalFacing == b.HorizontalFacing);
        }

        public static bool operator !=(cSpriteState a, cSpriteState b)
        {
            return (a.MotionType != b.MotionType || a.HorizontalFacing != b.HorizontalFacing);
        }

        public static bool operator <(cSpriteState a, cSpriteState b)
        {
            if (a.MotionType != b.MotionType)
            {
                return a.MotionType < b.MotionType;
            }

            if (a.HorizontalFacing != b.HorizontalFacing)
            {
                return a.HorizontalFacing < b.HorizontalFacing;
            }

            return false;
        }

        public static bool operator >(cSpriteState a, cSpriteState b)
        {
            if (a.MotionType != b.MotionType)
            {
                return a.MotionType > b.MotionType;
            }

            if (a.HorizontalFacing != b.HorizontalFacing)
            {
                return a.HorizontalFacing > b.HorizontalFacing;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            cSpriteState other = (cSpriteState)obj;
            return (this.MotionType == other.MotionType && this.HorizontalFacing == other.HorizontalFacing);
        }

        public override int GetHashCode()
        {
            return (int)motionType ^ (int)horizontalFacing;
        }
    }
}
