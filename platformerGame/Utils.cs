using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace platformerGame
{
    class Utils
    {
        //==========================================================================================================
        public static Color GetRandomRedColor()
        {
            Color c = GetRandomColor(90, 255, 1, 80, 1, 80); // (160, 255, 20, 80, 20, 80);
            return c;
        }
        //==========================================================================================================
        public static Color GetRandomGreenColor()
        {
            Color c = GetRandomColor(20, 100, 160, 255, 20, 100);
            return c;
        }
        //==========================================================================================================
        public static Color GetRandomBlueColor()
        {
            Color c = GetRandomColor(20, 100, 20, 100, 160, 255);
            return c;
        }
        //==========================================================================================================
        public static Color GetRandomColor(byte minR, byte maxR, byte minG, byte maxG, byte minB, byte maxB)
        {
            Color c = new Color();

            c.R = cAppMath.GetRandomByte(minR, maxR);
            c.G = cAppMath.GetRandomByte(minG, maxG);
            c.B = cAppMath.GetRandomByte(minB, maxB);
            c.A = 255;
            return c;
        }
    }
}
