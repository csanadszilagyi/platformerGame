using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame
{
    class cTimeInfo
    {
        float seconds;

        public cTimeInfo()
        {
            seconds = 0.0f;
        }

        public cTimeInfo(float sec)
        {
            seconds = sec;
        }
        public float Seconds
        {
            get { return seconds; }
            set { seconds = value; }
        }

        public override string ToString()
        {
            int min = (int)(seconds / 60);
            int sec = ((int)seconds % 60);
            int sec2 = sec / 10;
            int sec3 = sec % 10;

            return String.Format("{0}:{1}{2}", min, sec2, sec3);
        }
    }
}
