warning: CRLF will be replaced by LF in platformerGame/cSfmlApp.cs.
The file will have its original line endings in your working directory.
[1mdiff --git a/platformerGame/cTimer.cs b/platformerGame/cTimer.cs[m
[1mindex 1442f9d..4f88e05 100644[m
[1m--- a/platformerGame/cTimer.cs[m
[1m+++ b/platformerGame/cTimer.cs[m
[36m@@ -12,8 +12,8 @@[m [mnamespace platformerGame[m
     {[m
         private Clock m_Timer; //SFML beépített clock-ja[m
 [m
[31m-        private float m_CurrentTime;[m
[31m-        private float m_LastTime;[m
[32m+[m[32m        private long m_CurrentTime;[m
[32m+[m[32m        private long m_LastTime;[m
         public cTimer()[m
         {[m
             m_Timer = new Clock();[m
[36m@@ -21,12 +21,12 @@[m [mnamespace platformerGame[m
 [m
         public void Start()[m
         {[m
[31m-            m_CurrentTime = m_Timer.Restart().AsSeconds();[m
[32m+[m[32m            m_CurrentTime = m_Timer.Restart().AsMicroseconds();[m
             m_LastTime = m_CurrentTime;[m
         }[m
[31m-        public float GetCurrentTime()[m
[32m+[m[32m        public long GetCurrentTime()[m
         {[m
[31m-            return m_Timer.ElapsedTime.AsSeconds();[m
[32m+[m[32m            return m_Timer.ElapsedTime.AsMicroseconds();[m
         }[m
 [m
         public float GetCurrentTimeAsMilliseconds()[m
warning: CRLF will be replaced by LF in platformerGame/cTimer.cs.
The file will have its original line endings in your working directory.
