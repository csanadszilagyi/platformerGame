using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Threading;

using platformerGame.Utilities;

namespace platformerGame.App
{
    class SfmlApp
    {
        RenderWindow    mainWindow;
        Vector2f        windowSize;
        AppTimer          m_Timer;
        bool            m_AppRunning;

        Dictionary<string, Action> definiedStates = new Dictionary<string, Action>();
        
        GameState      currentState = null;
        // GameState   lastState = null;

        double m_DeltaTime;
        double m_StepTime;
        double m_MaxDeltaTime;
        double m_Accumulator;
        double m_Time;
        double m_FPS;

        Color clearColor;

        cRegulator fpsUpdater;
        Text timeText;

        View defaultView;

        bool vsync = false;

        AssetContext appAssets;

        public SfmlApp()
        {
            Constants.Load();
            appAssets = new AssetContext();
            
        }

        private void _setUpSFML()
        {
            windowSize = new Vector2f(1600.0f, 900.0f);
            
            //if WPF: m_MainWindow = new RenderWindow(formHandle);
            mainWindow = new RenderWindow(new VideoMode((uint)windowSize.X, (uint)windowSize.Y, 32), "Platformer", Styles.Close);
            mainWindow.SetVisible(true);
            activateVSYNC();
            mainWindow.SetFramerateLimit(0);
            mainWindow.SetKeyRepeatEnabled(false);

            mainWindow.Closed += new EventHandler(OnClosed);
            mainWindow.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            mainWindow.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyReleased);
            mainWindow.TextEntered += new EventHandler<TextEventArgs>(OnTextEntered);

            mainWindow.Resized += new EventHandler<SizeEventArgs>(OnResized);

            mainWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseButtonReleased);
            mainWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);

            mainWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
            
            
            clearColor = new Color(0, 0, 0, 255);

            this.defaultView = mainWindow.DefaultView;
            
        }

        private void _init()
        {
            _setUpSFML();

            //resource-ok betöltése (font-ok és textúrák, képek a játékhoz)
            appAssets.LoadResources(Constants.FONT_NAMES);

            m_DeltaTime = 0.0;
            m_StepTime = 1.0 / 60.0;
            m_MaxDeltaTime = 0.25; //0.25f
            m_Accumulator = 0.0;
            m_Time = 0.0;
            m_FPS = 0.0;

            m_Timer = new AppTimer();
            //ChangeGameState( new GameScene(this) );

            fpsUpdater = new cRegulator();
            fpsUpdater.resetByPeriodTime(1.0f);


            //Idő szöveg
            timeText = new Text("", appAssets.GetFont("pf_tempesta_seven"));
            timeText.Position = new Vector2f(this.defaultView.Size.X-500, 30);
            timeText.CharacterSize = 28; // in pixels, not points!
            timeText.FillColor = Color.White;
            timeText.Style = Text.Styles.Bold;

            GlobalClock.Start();

            // currentState = new GameScene(this);
            // currentState.Enter();

            m_AppRunning = true;

            definiedStates.Add("main-menu", () => { this.SetGameState(new MainMenu(this)); });
            definiedStates.Add("game-scene", () => { this.SetGameState(new GameScene(this)); });

            this.ChangeGameState("main-menu");

        }

        private void toggleVSYNC()
        {
            vsync = !vsync;
            MainWindow.SetVerticalSyncEnabled(vsync);
        }
        private void activateVSYNC()
        {
            this.vsync = true;
            mainWindow.SetVerticalSyncEnabled(true);
        }

        private void deactivateVSYNC()
        {
            this.vsync = false;
            mainWindow.SetVerticalSyncEnabled(false);
        }

        private void _beforeUpdate()
        {
            currentState.BeforeUpdate();
        }

        private void _updateFixed(float step_time)
        {
            currentState.UpdateFixed(step_time);
        }

        private void _updateVariable(float step_time  = 1.0f)
        {
            currentState.UpdateVariable(step_time);
        }

        private void _render(float alpha)
        {
            mainWindow.Clear(clearColor);

            currentState.Render(mainWindow, alpha);



            this.mainWindow.SetView(this.defaultView);
            
            //if (fpsUpdater.isReady())
             //   timeText.DisplayedString = /*"Delta: " + delta.ToString() + "  " + */"VSYNC: " + (this.vsync ? "ON" : "OFF"); //entityPool.getNumOfActiveBullets().ToString();
            
            this.mainWindow.Draw(timeText);


            mainWindow.Display();
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

                mainWindow.DispatchEvents();

                _beforeUpdate();

               

                accu += delta;

                while (accu >= STEP)
                {
                    _updateFixed(FSTEP_TIME);
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

                mainWindow.DispatchEvents();
                _beforeUpdate();


                loops = 0;

                while (next_game_tick < curTime && loops < MAX_FRAMESKIP)
                {
                    _updateFixed(STEP_TIME);

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
            float stepTime = Constants.STEP_TIME;
            int loops;
            float alpha = 0.0f;

            while (m_AppRunning)
            {

                time = timer.ElapsedTime.AsSeconds();
                delta = time - lastTime;
                lastTime = time;

                //m_MainWindow.DispatchEvents();

                //m_DeltaTime = m_Timer.GetDeltaTime();
                //m_FPS = 1.0f / m_DeltaTime;
                
                if (delta > maxDelta)
                    delta = maxDelta;
                

                acc += delta;

                /*
                float lagTreshold = MAX_FRAMESKIP * stepTime;
                if (acc > lagTreshold)
                {
                    acc = lagTreshold;
                }
                */

                _beforeUpdate();

                loops = 0;
                while (acc >= stepTime)
                {
                    mainWindow.DispatchEvents();
                    _updateFixed(stepTime);
                    acc -= stepTime;

                    loops++;
                    
                    if(loops >= MAX_FRAMESKIP)
                    {
                        acc = 0.0f;
                        break;
                    }
                    
                }

                _updateVariable();

                alpha = acc / stepTime;
                _render(alpha);
            }
        }


        public void _destroy()
        {
            currentState.Exit();
            mainWindow.Close();
        }
        
        public void Run()
        {
            _init();
            _mainLoop();
            _destroy();
        }

        /// <summary>
        /// Pl. kilépés menüre gombra kattintanak
        /// </summary>
        public void CloseApp()
        {
            currentState.Exit();
            m_AppRunning = false;
        }

        public double FPS
        {
            get { return m_FPS; }
        }

        private void SetGameState(GameState new_state)
        {
            // ? is neccessary
            currentState?.Exit();
            currentState = new_state;
            currentState.Enter();
        }

        public void ChangeGameState(string state_id)
        {
            Action desiredAction = null;
            if(definiedStates.TryGetValue(state_id, out desiredAction))
            {
                desiredAction?.Invoke();
            }

        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            var view = mainWindow.GetView();
            view.Size = new Vector2f(windowSize.X, windowSize.Y); // (e.Width, e.Height);
            mainWindow.SetView(view);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.CloseApp();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            /*
            if (e.Code == Keyboard.Key.Escape)
                m_AppRunning = false;
            */

            if (e.Code == Keyboard.Key.V)
                toggleVSYNC();

            currentState.HandleKeyPressed(e);

        }

        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            currentState.HandleKeyReleased(e);
        }

        private void OnTextEntered(object sender, TextEventArgs e)
        {
            currentState.HandleTextEntered(e);
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            currentState.HandleMouseMoved(e);
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            currentState.HandleMouseButtonPressed(e);
        }

        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            currentState.HandleMouseButtonReleased(e);
        }

        public RenderWindow MainWindow
        {
            get { return mainWindow; }
        }

        public Vector2f WindowSize
        {
            get { return windowSize; }
        }

        public AABB WindowArea
        {
            get { return new AABB(0.0f, 0.0f, windowSize.X, windowSize.Y); }
        }

        public void StartGame()
        {
            this.ChangeGameState("game-scene");
        }
    }
}
