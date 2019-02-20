using platformerGame.Utilities;

namespace platformerGame
{
    public class GlobalClock
    {
        /*private static readonly GlobalClock instance = new GlobalClock();*/

        private static AppTimer timer;
        /*private GlobalClock() { }*/
        static GlobalClock()
        {
            timer = new AppTimer();
        }
        /*
        public static GlobalClock Instance
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
