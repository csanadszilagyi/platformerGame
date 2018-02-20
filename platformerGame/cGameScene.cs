using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

using platformerGame.GameObjects;
using platformerGame.Particles;
using platformerGame.Utilities;

namespace platformerGame
{

    /// <summary>
    /// Magát a belső játékot összefogó és megvalósító osztály
    /// </summary>
    class cGameScene : cGameState
    {
        cWorld m_World;

        cPlayer m_Player;

        cEnvironment worldEnvironment;

        cLightSystem lightMap;

        cTimer levelTimer;

        cAABB viewRect;

        cEntityPool entityPool;

        cParticleManager particleManager;

        RenderTexture staticTexture;

        Queue<Action> gameCommands;

        public cGameScene(cSfmlApp controller) : base(controller)
        {
            levelTimer = new cTimer();
        }

       
        public override void Enter()
        {
            Vector2f worldSize = new Vector2f(m_AppController.MainWindow.Size.X, m_AppController.MainWindow.Size.Y);
            m_View.Size = new Vector2f(worldSize.X, worldSize.Y);
            m_View.Center = new Vector2f(worldSize.X / 2.0f, worldSize.Y / 2.0f);
            m_View.Viewport = new FloatRect(0.0f, 0.0f, 1.0f, 1.0f);
            m_View.Zoom(0.6f); //0.6f
            m_AppController.MainWindow.SetView(m_View);

            viewRect = new cAABB();
            viewRect.SetDims(m_View.Size);

            worldEnvironment = new cEnvironment();

            // Constants.LIGHTMAP_COLOR
            lightMap = new cLightSystem(Constants.LIGHTMAP_COLOR); //((uint)m_World.WorldBounds.dims.X, (uint)m_World.WorldBounds.dims.Y, Constants.LIGHTMAP_COLOR);
            m_World = new cWorld(this, m_AppController.MainWindow.Size);

            //lightMap.Create((uint)m_World.WorldBounds.dims.X, (uint)m_World.WorldBounds.dims.Y);
            lightMap.Create(m_AppController.MainWindow.Size.X, m_AppController.MainWindow.Size.Y);

            this.staticTexture = new RenderTexture((uint)m_World.WorldBounds.dims.X, (uint)m_World.WorldBounds.dims.Y);
            this.staticTexture.SetActive(true);
            this.staticTexture.Clear(new Color(0,0,0,0));
            //this.staticTexture.SetView(m_View);
            

            Vector2f playerStart = new Vector2f(m_World.LevelStartRegion.center.X, m_World.LevelStartRegion.rightBottom.Y);
            playerStart.X -= Constants.CHAR_FRAME_WIDTH / 2.0f;
            playerStart.Y -= Constants.CHAR_FRAME_HEIGHT;

            m_Player = new cPlayer(this, playerStart);

            entityPool = new cEntityPool(this, m_World.WorldBounds.dims, m_Player);

            //vizekhez adunk fényt
            /*
            List<cWaterBlock> waterBlocks = m_World.GetWaterBlocks();

            foreach (cWaterBlock wb in waterBlocks)
            {
                cLight waterLight = new cLight(); //víz blokkokhoz adunk fényt, mert jól néz ki
                waterLight.Pos = new Vector2f(wb.Area.center.X, wb.Area.topLeft.Y+Constants.TILE_SIZE/2.0f);
                waterLight.Radius = (wb.Area.dims.X + wb.Area.dims.Y) * 0.8f;
                waterLight.Bleed = 0.00001f; // 0.00001f;
                waterLight.LinearizeFactor = 0.95f;
                waterLight.Color = new Color(41,174,232); // 96,156,164
                lightMap.AddStaticLight(waterLight);
            }

            //háttér, környezeti tárgyak megjelenítése
            worldEnvironment.SetWaterBlocks(waterBlocks);
            */

            this.particleManager = new cParticleManager(this);
            // lightMap.renderStaticLightsToTexture();

            gameCommands = new Queue<Action>(50);
            //Pálya idő start
            levelTimer.Start();

        }


        public override void BeforeUpdate()
        {
           

          
        }

        public override void Update(float step_time)
        {
            UpdatePlayerInput();

            while (gameCommands.Count > 0)
            {
                gameCommands.Dequeue().Invoke();
            }

            entityPool.Update(step_time);
            m_Player.Update(step_time);

            if (cCollision.IsPointInsideBox(m_Player.Position, m_World.LevelEndRegion))
            {
                //vége a pályának
                
                //AppController.PlayerInfo.Time = levelTimer.GetCurrentTime();
                //AppController.PlayerInfo.Level = AppController.LevelName;
                //AppController.CloseApp();
            }

            this.particleManager.Update(step_time);

           // worldEnvironment.Update(step_time);

            
        }

        private void PreRender(RenderTarget destination, float alpha)
        {
            m_Player.CalculateViewPos(alpha);

            Vector2f pcenter = m_Player.GetCenterViewPos();
            m_View.Center = pcenter;

            //játékoshoz viszonyított view rect
            
            //viewRect.SetDims(m_View.Size);
            viewRect.SetPosByCenter(m_View.Center);

            if (viewRect.topLeft.X < 0.0f)
                m_View.Center = new Vector2f(m_World.WorldBounds.topLeft.X + m_View.Size.X / 2.0f, m_View.Center.Y);
            else
            if (viewRect.rightBottom.X > m_World.WorldBounds.rightBottom.X)
                m_View.Center = new Vector2f(m_World.WorldBounds.rightBottom.X - m_View.Size.X / 2.0f, m_View.Center.Y);


            if (viewRect.topLeft.Y < 0.0f)
                m_View.Center = new Vector2f(m_View.Center.X, m_World.WorldBounds.topLeft.Y + m_View.Size.Y / 2.0f);
            else
             if (viewRect.rightBottom.Y > m_World.WorldBounds.rightBottom.Y)
                m_View.Center = new Vector2f(m_View.Center.X, m_World.WorldBounds.rightBottom.Y - m_View.Size.Y / 2.0f);

            viewRect.SetPosByCenter(m_View.Center);

            destination.SetView(m_View);

            this.lightMap.separateVisibleLights(viewRect);

            this.particleManager.PreRender(alpha);
            //TODO: Entity pool PreRender, filter visible objects
        }
        public override void Render(RenderTarget destination, float alpha)
        {

            this.PreRender(destination, alpha);

            this.staticTexture.Display();

            m_World.DrawBackground(destination);

            //worldEnvironment.RenderEnvironment(destination);

            m_World.Render(destination, viewRect);

            m_Player.Render(destination);

            //worldEnvironment.RenderWaterBlocks(destination);

            this.entityPool.RenderEntities(destination, alpha);
            
            

            cRenderFunctions.DrawTextureSimple( destination,
                                                viewRect.topLeft,
                                                this.staticTexture.Texture,
                                                new IntRect((int)viewRect.topLeft.X, (int)viewRect.topLeft.Y,
                                                            (int)viewRect.dims.X, (int)viewRect.dims.Y),
                                                Color.White,
                                                BlendMode.Add);


            this.lightMap.Render(destination, viewRect);

            this.entityPool.RenderPickups(destination, alpha);

            this.entityPool.RenderBullets(destination, alpha);

            this.particleManager.Render(destination, alpha);

            /*
#if DEBUG
            this.entityPool.RenderQuadtree(destination);
#endif
*/

            // cRenderFunctions.DrawLine(destination, new Vector2f(0, 400), new Vector2f(720, 400), Color.White, BlendMode.None);
        }

        public RenderTexture StaticTexture
        {
            get { return staticTexture; }
        }

        public override void Exit()
        {
            m_World.ClearAll();
            lightMap.RemoveAll();
        }

        private void UpdatePlayerInput()
        {
            Vector2f mouse = this.GetMousePos(); // this.GetMousePos();

            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                m_Player.CurrentWeapon.fire(mouse);
                // m_Player.Position = this.GetMousePos();
            }

            //player movement
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                m_Player.StartJumping();
            }
            else
                m_Player.StopJumping();

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                if(m_Player.IsOnOneWayPlatform)
                    m_Player.Move(0.0f, 1.0f);
                //m_Player.IsOnOneWayPlatform = false;
            }


            if (Keyboard.IsKeyPressed(Keyboard.Key.A) && Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                m_Player.StopMoving();//stop m_Player moving
            }
            else
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                if (m_Player.Velocity.X > 0.0f)
                    m_Player.StopMovingX();

                m_Player.StartMovingLeft();
            }
            else
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                if (m_Player.Velocity.X < 0.0f)
                    m_Player.StopMovingX();

                m_Player.StartMovingRight();
            }
            else
                m_Player.StopMoving();//stop m_Player moving
        }

        public Vector2f GetMousePosInWindow()
        {
            Vector2i m = SFML.Window.Mouse.GetPosition(m_AppController.MainWindow);
            Vector2f mf = m_AppController.MainWindow.MapPixelToCoords(m);

            return mf; // new Vector2f(m.X, m.Y);
        }
        public Vector2f GetMousePos()
        {
            Vector2i iMp = SFML.Window.Mouse.GetPosition(m_AppController.MainWindow);

            Vector2f mousePos = m_AppController.MainWindow.MapPixelToCoords(iMp, m_View);
            
            return mousePos;
        }

        public override void HandleSingleMouseClick(MouseButtonEventArgs e)
        {
            Vector2f mousePos = this.GetMousePos();
            if (e.Button == Mouse.Button.Right)
            {
                 this.entityPool.AddMonster(new cMonster(this, mousePos));
               // this.particleManager.AddExplosion(this.GetMousePos());
            }
        }

        public cEntityPool EntityPool
        {
            get { return entityPool; }
        }
        public cWorld World
        {
            get { return m_World; }
        }

        public cEnvironment WolrdEnv
        {
            get { return worldEnvironment; }
        }

        public cPlayer Player
        {
            get { return m_Player; }
        }

        public cLightSystem LightMap
        {
            get { return lightMap; }
        }

        public cParticleManager ParticleManager
        {
            get { return this.particleManager; }
        }

        public void QueueCommand(Action action)
        {
            gameCommands.Enqueue(action);
        }

        public bool onScreen(cAABB box)
        {
            return cCollision.OverlapAABB(this.viewRect, box);
        }

    }
}
