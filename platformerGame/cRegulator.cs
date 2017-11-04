using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cRegulator
    {
        float freq;
        float periodTime; //in seconds

        //in milliseconds
        float currentTime = 0.0f;
        float lastTime = 0.0f;
	    public cRegulator()
        {
            freq = 0.0f;
            periodTime = 0.0f;
        }

        public bool isReady()
        {
            currentTime = cGlobalClock.GetTimeInMilliseconds();

            if ((currentTime - lastTime) >= periodTime * 1000.0f)
            {
                lastTime = currentTime;
                return true;
            }

            return false;
        }

        public void resetByFrequency(float frequency)
        {
            this.freq = frequency;
            this.periodTime = (frequency > 0.0f) ? (1.0f / frequency) : 0.0f;
            lastTime = cGlobalClock.GetTimeInMilliseconds();
        }
        public void resetByPeriodTime(float period_time_in_seconds)
        {
            this.periodTime = period_time_in_seconds;
            this.freq = (periodTime > 0.0f) ? (1.0f / periodTime) : 0.0f;
            lastTime = cGlobalClock.GetTimeInMilliseconds();
        }
        public float getPeriodTime()
        {
            return periodTime;
        }
        public float getFrequency()
        {
            return freq;
        }
    }
}
