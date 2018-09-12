using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;
using platformerGame.Utilities;

namespace platformerGame
{
    static class ShakeScreen
    {
        static Vector2f m_DefaultCenter;
        static Vector2f m_Offset;

        static Vector2f m_CurrentCenterPos;
        static float m_CurrentRadius;
        static float m_CurrentAngle;

        static float m_StartRadius;
        static float m_MinShakeRadius;
        static float m_DiminishFactor;

        static bool shaking;

        static ShakeScreen()
        {
            shaking = false;
        }

        public static void Init(Vector2f default_center)
        {
            m_DefaultCenter = default_center;
        }

        public static void StopShake()
        {
            shaking = false;
            m_Offset = new Vector2f(0.0f, 0.0f);
            m_CurrentCenterPos = new Vector2f(0.0f, 0.0f); //m_DefaultCenter;
        }
        //=====================================================================================================
        public static void StartShake(float start_radius = 5.0f, float min_shake_radius = 1.0f, float dimin_factor = 0.925f)
        {
            shaking = true;
            m_CurrentCenterPos = m_DefaultCenter;
            m_Offset = new Vector2f(0.0f, 0.0f);
            m_CurrentAngle = (float)cAppRandom.GetRandomNumber(0, 360);
            m_CurrentRadius = start_radius;
            m_MinShakeRadius = min_shake_radius;
            m_DiminishFactor = dimin_factor;
        }


        public static void Update()
        {
            if(shaking)
            {
                m_CurrentRadius *= m_DiminishFactor; //diminish radius each frame
                m_CurrentAngle += (150.0f + cAppRandom.GetRandomNumber(0, 60)); //pick new angle 
                m_Offset = new Vector2f((float)Math.Cos(m_CurrentAngle) * m_CurrentRadius, (float)Math.Sin(m_CurrentAngle) * m_CurrentRadius); //create offset 2d vector

                m_CurrentCenterPos = m_Offset; // m_DefaultCenter + m_Offset;
                //set centre of viewport

                if (m_CurrentRadius < m_MinShakeRadius) //2.0f
                    StopShake();
               
            }


        }

        public static Vector2f Offset
        {
            get { return m_CurrentCenterPos; }
        }


    }
}
