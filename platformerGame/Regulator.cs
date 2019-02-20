using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cRegulator
    {
        double freq;
        double periodTime; //in seconds

        //in milliseconds
        double currentTime = 0.0;
        double lastTime = 0.0;

	    public cRegulator()
        {
            freq = 0.0;
            periodTime = 0.0;
        }

        public bool isReady()
        {
            currentTime = GlobalClock.GetTimeInMilliseconds();

            if ((currentTime - lastTime) >= periodTime * 1000.0)
            {
                lastTime = currentTime;
                return true;
            }

            return false;
        }

        public void resetByFrequency(double frequency)
        {
            this.freq = frequency;
            this.periodTime = (frequency > 0.0) ? (1.0 / frequency) : 0.0;
            lastTime = GlobalClock.GetTimeInMilliseconds();
        }
        public void resetByPeriodTime(float period_time_in_seconds)
        {
            this.periodTime = period_time_in_seconds;
            this.freq = (periodTime > 0.0) ? (1.0 / periodTime) : 0.0;
            lastTime = GlobalClock.GetTimeInMilliseconds();
        }
        public double getPeriodTime()
        {
            return periodTime;
        }
        public double getFrequency()
        {
            return freq;
        }
    }
}
