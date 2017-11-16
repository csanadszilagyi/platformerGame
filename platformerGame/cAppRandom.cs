using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cAppRandom
    {
        private static Random random;

        static cAppRandom()
        {
            random = new Random();
        }
        /// <summary>
        /// Chooses a random item from the given array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T Chooose<T>(T[] array)
        {
            return array[ random.Next(0, array.Length) ];
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
    }
}
