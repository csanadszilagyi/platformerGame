using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace platformerGame.App
{
    /// <summary>
    /// A játék állapotát biztosító alaposztály. Ilyen állapot lehet pl. Főmenü, játék vagy új szint.
    /// </summary>
    abstract class GameState
    {
        protected SfmlApp appControllerRef = null;
        protected Camera camera = null;
        protected AssetContext resourceAssets = null;

        //protected View m_View;
        public GameState(SfmlApp app_ref)
        {
            appControllerRef = app_ref;
            resourceAssets = new AssetContext();
            //m_View = new View();
        }
        /// <summary>
        /// Az állapot induláskor kell végre hajtani
        /// </summary>
        public abstract void Enter();

        public virtual void     BeforeUpdate() { }
        public abstract void    UpdateFixed(float step_time);
        public abstract void    UpdateVariable(float step_time = 1.0f);

        public abstract void    Render(RenderTarget destination, float alpha);

        // Events
        public abstract void    HandleMouseButtonPressed       (MouseButtonEventArgs e);
        public abstract void    HandleMouseButtonReleased      (MouseButtonEventArgs e);
        public abstract void    HandleMouseMoved               (MouseMoveEventArgs e);

        public abstract void    HandleKeyPressed               (KeyEventArgs e);
        public abstract void    HandleKeyReleased              (KeyEventArgs e);
        public abstract void    HandleTextEntered              (TextEventArgs e);

        /// <summary>
        /// Must be called when exiting from a state
        /// </summary>
        public abstract void Exit();

        public Vector2f GetMousePosInWindow()
        {
            Vector2i m = Mouse.GetPosition(appControllerRef.MainWindow);
            Vector2f mf = appControllerRef.MainWindow.MapPixelToCoords(m);

            return mf; // new Vector2f(m.X, m.Y);
        }

        public Vector2f GetMousePos()
        {
            Vector2i iMp = Mouse.GetPosition(appControllerRef.MainWindow);

            Vector2f mousePos = appControllerRef.MainWindow.MapPixelToCoords(iMp, camera.View);

            return mousePos;
        }

        public SfmlApp AppController
        {
            get { return this.appControllerRef; }
        }

        public Camera Camera
        {
            get { return this.camera; }
        }

        public AssetContext Assets
        {
            get { return this.resourceAssets; }
        }
    }
}
