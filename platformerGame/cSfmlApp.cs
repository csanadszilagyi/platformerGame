using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cSfmlApp
    {
        RenderWindow    m_MainWindow;
        Vector2u        m_WindowSize;
        cTimer          m_Timer;
        bool            m_AppRunning;

        cGameState      m_CurrentState = null;
        //cGameState      m_LastState = null;

        float m_DeltaTime;
        float m_StepTime;
        float m_MaxDeltaTime;
        float m_Accumulator;
        float m_Time;
        float m_FPS;

        Color clearColor;

        cPlayerInfo playerInfo;

        public string LevelName { get; set; }
        public cSfmlApp()
        {
            playerInfo = new cPlayerInfo();
        }

        public cSfmlApp(string level_name)
        {
            playerInfo = new cPlayerInfo();
            LevelName = level_name;
        }

        private void _setUpSFML()
        {
            m_WindowSize = new Vector2u(1280, 720);
            
            //m_MainWindow = new RenderWindow(formHandle);
            m_MainWindow =  new RenderWindow(new VideoMode(m_WindowSize.X, m_WindowSize.Y, 32), "Platformer", Styles.Close);
            m_MainWindow.SetVisible(true);
            m_MainWindow.SetVerticalSyncEnabled(true); // false
            m_MainWindow.SetFramerateLimit(70);
            m_MainWindow.SetKeyRepeatEnabled(false);

            m_MainWindow.Closed += new EventHandler(OnClosed);
            m_MainWindow.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            m_MainWindow.Resized += new EventHandler<SizeEventArgs>(OnResized);
            m_MainWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);
            m_AppRunning = true;
            
            clearColor = new Color(0, 0, 0, 255);

            //resource-ok betöltése (font-ok és textúrák, képek a játékhoz)
            cAssetManager.LoadResources();
        }

        private void _init()
        {
            _setUpSFML();

            m_DeltaTime = 0.0f;
            m_StepTime = 0.0166f;
            m_MaxDeltaTime = 0.25f;
            m_Accumulator = 0.0f;
            m_Time = 0.0f;
            m_FPS = 0.0f;

            m_Timer = new cTimer();
            //ChangeGameState( new cGameScene(this) );

            cGlobalClock.Start();

            m_CurrentState = new cGameScene(this);
            m_CurrentState.Enter();

            
        }
        private void _update(float step_time)
        {
            m_CurrentState.Update(step_time);
        }

        private void _render(float alpha)
        {
            m_MainWindow.Clear(clearColor);

            m_CurrentState.Render(m_MainWindow, alpha);

            m_MainWindow.Display();
        }
        private void _mainLoop()
        {
            m_Timer.Start();

            float alpha = 0.0f;

            while(m_AppRunning)
            {
                m_MainWindow.DispatchEvents();

                m_DeltaTime = m_Timer.GetDeltaTime();
                m_FPS = 1.0f / m_DeltaTime;

                if (m_DeltaTime > m_MaxDeltaTime)
                    m_DeltaTime = m_MaxDeltaTime;

                m_Accumulator += m_DeltaTime;

                while (m_Accumulator > m_StepTime) //m_StepTime
                {
                    _update(m_StepTime);
                    m_Accumulator -= m_StepTime;

                    m_Time += m_StepTime;
                }

                alpha = m_Accumulator / m_StepTime;

                _render(alpha);
            }
        }

        public void _destroy()
        {
            cAssetManager.Destroy();
            m_CurrentState.Exit();
            m_MainWindow.Close();
        }
        
        public void Run()
        {
            _init();
            _mainLoop();
            _destroy();
        }

        public cPlayerInfo PlayerInfo
        {
            get { return playerInfo; }
            set { playerInfo = value; }
        }
        /// <summary>
        /// Pl. kilépés menüre gombra kattintanak
        /// </summary>
        public void CloseApp()
        {
            m_AppRunning = false;
        }

        public float FPS
        {
            get { return m_FPS; }
        }

        public void ChangeGameState(cGameScene new_state)
        {
            if(m_CurrentState != null)
                m_CurrentState.Exit();

            //m_LastState = m_CurrentState;
            m_CurrentState = new_state;
            m_CurrentState.Enter();
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            var view = m_MainWindow.GetView();
            view.Size = new Vector2f(m_WindowSize.X, m_WindowSize.Y); // (e.Width, e.Height);
            m_MainWindow.SetView(view);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            m_AppRunning = false;
        }
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {

            if (e.Code == SFML.Window.Keyboard.Key.Escape)
                m_AppRunning = false;
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            m_CurrentState.HandleSingleMouseClick(e);
        }
        

        public RenderWindow MainWindow
        {
            get { return m_MainWindow; }
        }
    }
}
