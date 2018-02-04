using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using platformerGame.Utilities;

namespace platformerGame
{
    public class cGlobalClock
    {
        /*private static readonly cGlobalClock instance = new cGlobalClock();*/

        private static cTimer timer;
        /*private cGlobalClock() { }*/
        static cGlobalClock()
        {
            timer = new cTimer();
        }
        /*
        public static cGlobalClock Instance
        {
            get
            {
                return instance;
            }
        }
        */
        public static void Start()
        {
            
            timer.Start();
        }

        public static double GetCurrentTime()
        {
            return timer.GetCurrentTime();
        }

        public static double GetTimeInMilliseconds()
        {
            return timer.GetCurrentTimeAsMilliseconds();
        }
    }
}
