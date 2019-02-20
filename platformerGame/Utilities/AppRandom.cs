using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.Utilities
{
    class AppRandom
    {
        private static Random random;

        static AppRandom()
        {
            random = new Random();
        }
        /// <summary>
        /// Chooses a random item from the given array
        /// </summary>
        /// <typeparam name="T">type of the array</typeparam>
        /// <param name="array"></param>
        /// <param name="len">0 to len to choose, if -1, the length of the array used</param>
        /// <returns></returns>
        public static T Choose<T>(T[] array, int len = -1)
        {
            return array[ random.Next(0, (len < 0) ? array.Length : len) ];
        }

        /// <summary>
        /// Chooses a random item from the given list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Chooose<T>(List<T> list)
        {
            return list[random.Next(0, list.Count)];
        }

        /// <summary>
        /// Gets a random number in the specified range (max param excluded).
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// Gets a random float between 0 and 1.
        /// </summary>
        /// <returns></returns>
        public static float GetRandomFloat()
        {
            return (float)random.NextDouble();
        }

        public static double GetRandomDouble()
        {
            return random.NextDouble();
        }

        public static double GetRandomClamped()
        {
            return GetRandomDouble() - GetRandomDouble();
        }
    }
}
