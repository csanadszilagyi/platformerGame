using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace platformerGame.Utilities
{
    public static class Utils
    {
        public static readonly Color RED = new Color(255, 82, 82);
        public static readonly Color BLUE = new Color(99, 196, 236);

        //==========================================================================================================
        public static Color GetRandomRedColor()
        {
            Color c = GetRandomColor(200, 255, 5, 90, 5, 90); // (160, 255, 20, 80, 20, 80);
            return c;
        }
        //==========================================================================================================
        public static Color GetRandomOrangeColor()
        {
            Color c = GetRandomColor(220, 255, 130, 160, 50, 70); // (160, 255, 20, 80, 20, 80);
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
            Color c = GetRandomColor(37, 99, 150, 196, 220, 236);
            return c;
        }
        //==========================================================================================================
        public static Color GetRandomColor(byte minR, byte maxR, byte minG, byte maxG, byte minB, byte maxB)
        {
            Color c = new Color();

            c.R = AppMath.GetRandomByte(minR, maxR);
            c.G = AppMath.GetRandomByte(minG, maxG);
            c.B = AppMath.GetRandomByte(minB, maxB);
            c.A = 255;
            return c;
        }

        //==========================================================================================================
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
        //==========================================================================================================
        public static Color GetSfmlColorFromHex(string hex_color)
        {
            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(hex_color);
            return new Color(c.R, c.G, c.B, c.A);
        }
        //==========================================================================================================
        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> match)
        {
            foreach (var cur in dict.Where(match).ToList())
            {
                dict.Remove(cur.Key);
            }
        }
    }
}
