using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using SFML.System;

namespace platformerGame.Utilities
{
    class AppTimer
    {
        private Stopwatch m_Timer; //SFML beépített clock-ja

        private double m_CurrentTime;
        private double m_LastTime;

        public AppTimer()
        {
            m_Timer = new Stopwatch();
        }

        public void Start()
        {
            m_Timer.Start();
            m_CurrentTime = m_Timer.Elapsed.TotalSeconds;

            m_LastTime = m_CurrentTime;
        }
        public double GetCurrentTime()
        {
            return m_Timer.Elapsed.TotalSeconds;
        }

        public double GetCurrentTimeAsMilliseconds()
        {
            return m_Timer.Elapsed.TotalMilliseconds;
        }
        /// <summary>
        /// Megmondja az eltelt időt az előző hívás óta (másodpercben). Frame-nként hívva megmondja a framek között eltelt (delta) időt, amire szükségünk van.
        /// </summary>
        /// <returns></returns>
        public double GetDeltaTime()
        {
            m_CurrentTime = GetCurrentTime();
            double dt = m_CurrentTime - m_LastTime;
            m_LastTime = m_CurrentTime;
            return dt;
        }

        public override string ToString()
        {
            cTimeInfo tinfo = new cTimeInfo(this.GetCurrentTime());
            return tinfo.ToString();
        }

        /*
        private Clock m_Timer; //SFML beépített clock-ja

        private float m_CurrentTime;
        private float m_LastTime;
        public cTimer()
        {
            m_Timer = new Clock();
        }

        public void Start()
        {
            m_CurrentTime = m_Timer.Restart().AsSeconds();
            m_LastTime = m_CurrentTime;
        }
        public float GetCurrentTime()
        {
            return m_Timer.ElapsedTime.AsSeconds();
        }

        public float GetCurrentTimeAsMilliseconds()
        {
            return m_Timer.ElapsedTime.AsMilliseconds();
        }
        /// <summary>
        /// Megmondja az eltelt időt az előző hívás óta (másodpercben). Frame-nként hívva megmondja a framek között eltelt (delta) időt, amire szükségünk van.
        /// </summary>
        /// <returns></returns>
        public float GetDeltaTime()
        {
            m_CurrentTime = GetCurrentTime();
            float dt = m_CurrentTime - m_LastTime;
            m_LastTime = m_CurrentTime;
            return dt;
        }

        public override string ToString()
        {
            cTimeInfo tinfo = new cTimeInfo(this.GetCurrentTime());
            return tinfo.ToString();
        }
        */
    }
}
