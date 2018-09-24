using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

using platformerGame.GameObjects;
using platformerGame.Particles;
using platformerGame.Utilities;
using platformerGame.Rendering;

namespace platformerGame
{

    /// <summary>
    /// Magát a belső játékot összefogó és megvalósító osztály
    /// </summary>
    class cGameScene : cGameState
    {
        cWorld gameWorld;

        cPlayer player;

        cEnvironment worldEnvironment;

        cLightSystem lightMap;

        cTimer levelTimer;

        // AABB viewRect;

        Camera camera;

        EntityPool entityPool;

        cParticleManager particleManager;

        RenderTexture staticTexture;

        Queue<Action> gameActions;

        public cGameScene(SfmlApp controller) : base(controller)
        {
            levelTimer = new cTimer();
        }

       
        public override void Enter()
        {
            cAnimationAssets.LoadAnimations();
            camera = new Camera(new View(new Vector2f(appController.WindowSize.X / 2.0f, appController.WindowSize.Y / 2.0f), appController.WindowSize));
            camera.Zoom = 0.6f;
            appController.MainWindow.SetView(camera.View);

            /*
            Vector2f viewSize = new Vector2f(appController.MainWindow.Size.X, appController.MainWindow.Size.Y);

            m_View.Size = new Vector2f(viewSize.X, viewSize.Y);
            m_View.Center = new Vector2f(viewSize.X / 2.0f, viewSize.Y / 2.0f);
            m_View.Viewport = new FloatRect(0.0f, 0.0f, 1.0f, 1.0f);
            m_View.Zoom(0.6f); //0.6f
            
            viewRect = new AABB();
            viewRect.SetDims(m_View.Size);
            */
            
            worldEnvironment = new cEnvironment();

            // Constants.LIGHTMAP_COLOR
            lightMap = new cLightSystem(Constants.LIGHTMAP_COLOR); //((uint)m_World.WorldBounds.dims.X, (uint)m_World.WorldBounds.dims.Y, Constants.LIGHTMAP_COLOR);
            gameWorld = new cWorld(this, appController.MainWindow.Size);

            //lightMap.Create((uint)m_World.WorldBounds.dims.X, (uint)m_World.WorldBounds.dims.Y);
            lightMap.Create(appController.MainWindow.Size.X, appController.MainWindow.Size.Y);

            lightMap.loadLightsFromTmxMap(gameWorld.GetCurrentLevel().GetTmxMap());

            this.staticTexture = new RenderTexture((uint)gameWorld.WorldBounds.dims.X, (uint)gameWorld.WorldBounds.dims.Y);
            this.staticTexture.SetActive(true);
            this.staticTexture.Clear(new Color(0,0,0,0));
            //this.staticTexture.SetView(m_View);
            

            Vector2f playerStart = new Vector2f(gameWorld.LevelStartRegion.center.X, gameWorld.LevelStartRegion.rightBottom.Y);
            playerStart.X -= Constants.CHAR_FRAME_WIDTH / 2.0f;
            playerStart.Y -= Constants.CHAR_FRAME_HEIGHT;

            player = new cPlayer(this, playerStart);

            entityPool = new EntityPool(this, gameWorld.WorldBounds.dims, player);
            entityPool.InitLevelEntites(World.GetCurrentLevel());

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

            gameActions = new Queue<Action>(50);

            Listener.GlobalVolume = 80;
            Listener.Direction = new Vector3f(1.0f, 0.0f, 0.0f);

            ShakeScreen.Init(camera.ActualPosition);
            //Pálya idő start
            levelTimer.Start();

        }


        public override void BeforeUpdate()
        {
           

          
        }

        public override void Update(float step_time)
        {
            Listener.Position = new Vector3f(player.Bounds.center.X, player.Bounds.center.Y, 5.0f);

            UpdatePlayerInput();

            while (gameActions.Count > 0)
            {
                gameActions.Dequeue().Invoke();
            }

           
            ShakeScreen.Update();

            player.Update(step_time);

            entityPool.Update(step_time);
            

            /*
            if (cCollision.IsPointInsideBox(player.Position, gameWorld.LevelEndRegion))
            {
                //vége a pályának
            }
            */

            this.particleManager.Update(step_time);
 
            Vector2f playerCenter = player.Bounds.center;
 
            this.camera.Update(playerCenter, gameWorld.WorldBounds);
            // worldEnvironment.Update(step_time);
        }

        private void PreRender(RenderTarget destination, float alpha)
        {
            this.player.CalculateViewPos(alpha);

            //camera.Update(player.ViewPosition);
            // Vector2f playerCenter = player.GetCenterViewPos();
            // camera.Position = playerCenter;

            // m_View.Center = playerCenter;
            //játékoshoz viszonyított view rect
            //viewRect.SetDims(m_View.Size);
            //viewRect.SetPosByCenter(m_View.Center);


            // camera.Update();
            this.camera.DeployOn(destination);
            var cameraBounds = camera.Bounds;
            //viewRect.SetPosByCenter(m_View.Center);

            
            //destination.GetView().Move(ShakeScreen.Offset);

            // destination.SetView(m_View);

            this.gameWorld.PreRender(cameraBounds);

            this.lightMap.separateVisibleLights(cameraBounds);

            this.particleManager.PreRender(alpha);

            this.entityPool.PreRender(alpha, cameraBounds);
            //TODO: Entity pool PreRender, filter visible objects
        }

        public override void Render(RenderTarget destination, float alpha)
        {

            this.PreRender(destination, alpha);

           
            AABB cameraBounds = camera.Bounds;
            //this.staticTexture.Display();

            gameWorld.DrawBackground(destination);


            //worldEnvironment.RenderEnvironment(destination);

            gameWorld.Render(destination, cameraBounds);

            player.Render(destination);

            //worldEnvironment.RenderWaterBlocks(destination);

            this.entityPool.Render(destination);
            
            

            DrawingBase.DrawTextureSimple( destination,
                                                cameraBounds.topLeft,
                                                this.staticTexture.Texture,
                                                new IntRect((int)cameraBounds.topLeft.X, (int)cameraBounds.topLeft.Y,
                                                            (int)cameraBounds.dims.X, (int)cameraBounds.dims.Y),
                                                Color.White,
                                                BlendMode.Add);


            this.lightMap.Render(destination, cameraBounds);

            this.entityPool.RenderPickups(destination);

            this.entityPool.RenderBullets(destination);

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
            gameWorld.ClearAll();
            lightMap.RemoveAll();
        }

        private void UpdatePlayerInput()
        {
            Vector2f mouse = this.GetMousePos(); // this.GetMousePos();

            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                player.CurrentWeapon.fire(mouse);
                // m_Player.Position = this.GetMousePos();
            }

            //player movement
            if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                player.StartJumping();
            }
            else
                player.StopJumping();

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                if(player.IsOnOneWayPlatform)
                    player.Move(0.0f, 1.0f);
                //m_Player.IsOnOneWayPlatform = false;
            }


            if (Keyboard.IsKeyPressed(Keyboard.Key.A) && Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                player.StopMoving();//stop m_Player moving
            }
            else
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                if (player.Velocity.X > 0.0f)
                    player.StopMovingX();

                player.StartMovingLeft();
            }
            else
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                if (player.Velocity.X < 0.0f)
                    player.StopMovingX();

                player.StartMovingRight();
            }
            else
                player.StopMoving();//stop m_Player moving
        }

        public Vector2f GetMousePosInWindow()
        {
            Vector2i m = SFML.Window.Mouse.GetPosition(appController.MainWindow);
            Vector2f mf = appController.MainWindow.MapPixelToCoords(m);

            return mf; // new Vector2f(m.X, m.Y);
        }
        public Vector2f GetMousePos()
        {
            Vector2i iMp = SFML.Window.Mouse.GetPosition(appController.MainWindow);

            Vector2f mousePos = appController.MainWindow.MapPixelToCoords(iMp, camera.View);
            
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

        public EntityPool EntityPool
        {
            get { return entityPool; }
        }
        public cWorld World
        {
            get { return gameWorld; }
        }

        public cEnvironment WolrdEnv
        {
            get { return worldEnvironment; }
        }

        public cPlayer Player
        {
            get { return player; }
        }

        public cLightSystem LightMap
        {
            get { return lightMap; }
        }

        public cParticleManager ParticleManager
        {
            get { return this.particleManager; }
        }

        public void QueueAction(Action action)
        {
            gameActions.Enqueue(action);
        }

        public bool onScreen(AABB box)
        {
            return cCollision.OverlapAABB(this.camera.Bounds, box);
        }

        public Camera Camera
        {
            get { return this.camera; }
        }
    }
}
