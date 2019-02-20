using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace platformerGame.App
{
    /// <summary>
    /// A játék állapotát biztosító alaposztály. Ilyen állapot lehet pl. Főmenü, játék vagy új szint.
    /// </summary>
    abstract class GameState
    {
        protected SfmlApp appControllerRef = null;
        protected Camera camera = null;

        //protected View m_View;
        public GameState(SfmlApp app_ref)
        {
            appControllerRef = app_ref;
            //m_View = new View();
        }
        /// <summary>
        /// Az állapot induláskor kell végre hajtani
        /// </summary>
        public abstract void Enter();

        public virtual void BeforeUpdate() { }
        public abstract void UpdateFixed(float step_time);
        public abstract void UpdateVariable(float step_time = 1.0f);

        public abstract void Render(RenderTarget destination, float alpha);

        public abstract void HandleSingleMouseClick(SFML.Window.MouseButtonEventArgs e);

        public abstract void HandleKeyPress(SFML.Window.KeyEventArgs e);

        /// <summary>
        /// Az állapot kilépésekor kell meghívni
        /// </summary>
        public abstract void Exit();

        public Vector2f GetMousePosInWindow()
        {
            Vector2i m = SFML.Window.Mouse.GetPosition(appControllerRef.MainWindow);
            Vector2f mf = appControllerRef.MainWindow.MapPixelToCoords(m);

            return mf; // new Vector2f(m.X, m.Y);
        }

        public Vector2f GetMousePos()
        {
            Vector2i iMp = SFML.Window.Mouse.GetPosition(appControllerRef.MainWindow);

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
    }
}
