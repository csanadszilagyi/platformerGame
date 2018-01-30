using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Threading;

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

        double m_DeltaTime;
        double m_StepTime;
        double m_MaxDeltaTime;
        double m_Accumulator;
        double m_Time;
        double m_FPS;

        Color clearColor;

        cPlayerInfo playerInfo;

        cRegulator fpsUpdater;
        Text timeText;

        View defaultView;

        bool vsync = false;

        public string LevelName { get; set; }
        public cSfmlApp()
        {
            Constants.Load();
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
            
            //if WPF: m_MainWindow = new RenderWindow(formHandle);
            m_MainWindow = new RenderWindow(new VideoMode(m_WindowSize.X, m_WindowSize.Y, 32), "Platformer", Styles.Close);
            m_MainWindow.SetVisible(true);
            activateVSYNC();
            m_MainWindow.SetFramerateLimit(0);
            m_MainWindow.SetKeyRepeatEnabled(false);

            m_MainWindow.Closed += new EventHandler(OnClosed);
            m_MainWindow.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            m_MainWindow.Resized += new EventHandler<SizeEventArgs>(OnResized);
            m_MainWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);
            
            
            clearColor = new Color(0, 0, 0, 255);

            this.defaultView = m_MainWindow.DefaultView;
            //resource-ok betöltése (font-ok és textúrák, képek a játékhoz)
            cAssetManager.LoadResources();
        }

        private void _init()
        {
            _setUpSFML();

            m_DeltaTime = 0.0;
            m_StepTime = 1.0 / 60.0;
            m_MaxDeltaTime = 0.25; //0.25f
            m_Accumulator = 0.0;
            m_Time = 0.0;
            m_FPS = 0.0;

            m_Timer = new cTimer();
            //ChangeGameState( new cGameScene(this) );

            fpsUpdater = new cRegulator();
            fpsUpdater.resetByPeriodTime(1.0f);


            //Idő szöveg
            timeText = new Text("", cAssetManager.GetFont("pf_tempesta_seven"));
            timeText.Position = new Vector2f(this.defaultView.Size.X-500, 30);
            timeText.CharacterSize = 28; // in pixels, not points!
            timeText.Color = Color.White;
            timeText.Style = Text.Styles.Bold;

            cGlobalClock.Start();

            m_CurrentState = new cGameScene(this);
            m_CurrentState.Enter();

            m_AppRunning = true;

        }

        private void toggleVSYNC()
        {
            vsync = !vsync;
            MainWindow.SetVerticalSyncEnabled(vsync);
        }
        private void activateVSYNC()
        {
            this.vsync = true;
            m_MainWindow.SetVerticalSyncEnabled(true);
        }

        private void deactivateVSYNC()
        {
            this.vsync = false;
            m_MainWindow.SetVerticalSyncEnabled(false);
        }

        private void _beforeUpdate()
        {
            m_CurrentState.BeforeUpdate();
        }

        private void _update(float step_time)
        {
            m_CurrentState.Update(step_time);
        }

        private void _render(float alpha)
        {
            m_MainWindow.Clear(clearColor);

            m_CurrentState.Render(m_MainWindow, alpha);



            this.m_MainWindow.SetView(this.defaultView);
            
            if (fpsUpdater.isReady())
                timeText.DisplayedString = /*"Delta: " + delta.ToString() + "  " + */"VSYNC: " + (this.vsync ? "ON" : "OFF"); //entityPool.getNumOfActiveBullets().ToString();
             
            this.m_MainWindow.Draw(timeText);


            m_MainWindow.Display();
        }


        
        private void _mainLoop3()
        {
            const int TICKS_PER_SECOND = 25;
            const int SKIP_TICKS = 1000 / TICKS_PER_SECOND;
            const int MAX_FRAMESKIP = 5;

            Clock gameTime = new Clock();
            gameTime.Restart();

            float alpha = 0.0f;
            int loops;

            ulong time = (ulong)gameTime.ElapsedTime.AsMicroseconds();
            ulong lastTime = time;
            const ulong STEP = 16666UL; //16666UL
            const float FSTEP_TIME = STEP / 1000000.0f;
            const ulong MAX_DELTA = 10000UL;
            ulong delta = 0;
            ulong accu = 0;

            while (m_AppRunning)
            {

                time = (ulong)gameTime.ElapsedTime.AsMicroseconds();
                delta = time - lastTime;
                lastTime = time;

                if (delta > MAX_DELTA)
                    delta = MAX_DELTA;

                m_MainWindow.DispatchEvents();

                _beforeUpdate();

               

                accu += delta;

                while (accu >= STEP)
                {
                    _update(FSTEP_TIME);
                    accu -= STEP;
                }


                /*
                 loops = 0;
                while (time - updatedTime > STEP && loops < MAX_FRAMESKIP) //m_StepTime
                {
                    _update(FSTEP_TIME);
                    updatedTime += STEP;
                    loops++;
                }
                */

                alpha = (accu /1000000.0f) / FSTEP_TIME; //accu / (float)STEP; //

                _render(alpha);
            }
        }



        float interpolation = 0.0f;
        ulong next_game_tick = 0;

        private void _mainLoop2()
        {

            const int TICKS_PER_SECOND = 25;
            const int SKIP_TICKS = 1000 / TICKS_PER_SECOND;
            const int MAX_FRAMESKIP = 5;

            Clock gameTime = new Clock();
            gameTime.Restart();

            //First tick is at 0.0 next tick is at 
            
            int loops;
            
            const float STEP_TIME = 1.0f / (SKIP_TICKS*10);

            ulong curTime = (ulong)gameTime.ElapsedTime.AsMicroseconds();
            ulong lastTime = curTime; // in micro

            float updateTime = 0.0f;

            while (m_AppRunning)
            {
                
                curTime = (ulong)gameTime.ElapsedTime.AsMicroseconds();

                m_MainWindow.DispatchEvents();
                _beforeUpdate();


                loops = 0;

                while (next_game_tick < curTime && loops < MAX_FRAMESKIP)
                {
                    _update(STEP_TIME);

                    next_game_tick += SKIP_TICKS;
                    loops++;
                }

                interpolation = (float)(curTime + SKIP_TICKS - next_game_tick)
                                / (float)(SKIP_TICKS) / 1000000.0f;

               
                _render(interpolation);
                
                
            }
        }

        float delta;
        private void _mainLoop()
        {
           // m_Timer.Start();

            Clock timer = new Clock();
            timer.Restart();

            const int MAX_FRAMESKIP = 5;

            float time, lastTime;
            time = timer.ElapsedTime.AsSeconds();
            lastTime = time;
            
            float maxDelta = 0.25f; //0.25f
            float acc = 0;
            float stepTime = 1.0f / 60.0f;
            int loops;
            float alpha = 0.0f;

            while (m_AppRunning)
            {

                time = timer.ElapsedTime.AsSeconds();
                delta = time - lastTime;
                lastTime = time;

                m_MainWindow.DispatchEvents();

                //m_DeltaTime = m_Timer.GetDeltaTime();
                //m_FPS = 1.0f / m_DeltaTime;

               if (delta > maxDelta)
                    delta = maxDelta;

                acc += delta;

                _beforeUpdate();

                loops = 0;
                while (acc >= stepTime)
                {
                    _update(stepTime);
                    acc -= stepTime;

                    loops++;
                    if(loops >= MAX_FRAMESKIP)
                    {
                        acc = 0.0f;
                        break;
                    }
                }

                alpha = acc / stepTime;
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

        public double FPS
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

            if (e.Code == Keyboard.Key.Escape)
                m_AppRunning = false;

            if (e.Code == Keyboard.Key.V)
                toggleVSYNC();
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
