using System;

using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Threading;

using OverlordEngine.Utilities;

namespace OverlordEngine
{
    class SfmlApp
    {
        RenderWindow    mainWindow;
        Vector2u        windowSize;
        Utilities.Timer appTimer;
        bool            appRunning;

        IGame           game = null;

        Color clearColor;

        Text timeText;

        View defaultView;

        bool vsync = false;

        public SfmlApp(IGame refGame)
        {
            this.game = refGame;
        }

        private void setupSFML()
        {
            windowSize = new Vector2u(1280, 720);
            
            //if WPF: m_MainWindow = new RenderWindow(formHandle);
            mainWindow = new RenderWindow(new VideoMode(windowSize.X, windowSize.Y, 32), "Platformer", Styles.Close);
            mainWindow.SetVisible(true);
            activateVSYNC();
            mainWindow.SetFramerateLimit(0);
            mainWindow.SetKeyRepeatEnabled(false);

            mainWindow.Closed += new EventHandler(OnClosed);
            mainWindow.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            mainWindow.Resized += new EventHandler<SizeEventArgs>(OnResized);
            mainWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);
            
            
            clearColor = new Color(0, 0, 0, 255);

            this.defaultView = mainWindow.DefaultView;
            //resource-ok betöltése (font-ok és textúrák, képek a játékhoz)
            AssetManager.LoadResources();
        }

        private void init()
        {
            setupSFML();

            appTimer = new Utilities.Timer();
            //ChangeGameState( new cGameScene(this) );


            //Idő szöveg
            timeText = new Text("", AssetManager.GetFont("pf_tempesta_seven"));
            timeText.Position = new Vector2f(this.defaultView.Size.X-500, 30);
            timeText.CharacterSize = 28; // in pixels, not points!
            timeText.FillColor = Color.White;
            timeText.Style = Text.Styles.Bold;

            GlobalClock.Start();

            game.Init();

            appRunning = true;

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

        private void beforeUpdate()
        {
            game.OnBeforeUpdate();
        }

        private void updateFixed(float step_time)
        {
            game.OnUpdateFixed(step_time);
        }

        private void updateVariable(float stepTime)
        {
            game.OnUpdateVariable(stepTime);
        }

        private void preRender(float alpha)
        {
            game.OnPreRender(alpha);
        }

        private void render(float alpha)
        {
            mainWindow.Clear(clearColor);

            game.OnRender(mainWindow);

            mainWindow.Display();
        }

        private void mainLoop()
        {
           // m_Timer.Start();

            Clock timer = new Clock();
            timer.Restart();

            const int MAX_FRAMESKIP = 5;

            float time, lastTime;
            time = timer.ElapsedTime.AsSeconds();
            lastTime = time;

            float delta = 1.0f;
            float maxDelta = 0.25f; //0.25f
            float acc = 0;
            float stepTime = Constants.STEP_TIME;
            int loops;
            float alpha = 0.0f;

            while (appRunning)
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

                beforeUpdate();

                loops = 0;
                while (acc >= stepTime)
                {
                    mainWindow.DispatchEvents();
                    updateFixed(stepTime);
                    acc -= stepTime;

                    loops++;
                    
                    if(loops >= MAX_FRAMESKIP)
                    {
                        acc = 0.0f;
                        break;
                    }
                    
                }

                updateVariable(1.0f);

                alpha = acc / stepTime;
                preRender(alpha);
                render(alpha);
            }
        }


        public void destroy()
        {
            game.Destroy();
            mainWindow.Close();
        }
        
        public void Run()
        {
            init();
            mainLoop();
            destroy();
        }

        /// <summary>
        /// Pl. kilépés menüre gombra kattintanak
        /// </summary>
        public void CloseApp()
        {
            appRunning = false;
        }

        /*
        public double FPS
        {
            get { return m_FPS; }
        }
        */

        /*
        public void ChangeGameState(cGameScene new_state)
        {
            if(game != null)
                game.Exit();

            //m_LastState = m_CurrentState;
            game = new_state;
            game.Enter();
        }
        */

        private void OnResized(object sender, SizeEventArgs e)
        {
            var view = mainWindow.GetView();
            view.Size = new Vector2f(windowSize.X, windowSize.Y); // (e.Width, e.Height);
            mainWindow.SetView(view);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            appRunning = false;
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {

            if (e.Code == Keyboard.Key.Escape)
                appRunning = false;

            if (e.Code == Keyboard.Key.V)
                toggleVSYNC();

            game.OnKeyPressed(e);
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            game.OnSingleMouseClick(e);
        }
        

        public RenderWindow MainWindow
        {
            get { return mainWindow; }
        }
    }
}
