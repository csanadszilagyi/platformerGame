using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;

namespace platformerGame
{
    /// <summary>
    /// A játék állapotát biztosító alaposztály. Ilyen állapot lehet pl. Főmenü, játék vagy új szint.
    /// </summary>
    abstract class cGameState
    {
        protected SfmlApp appController = null;
        //protected View m_View;
        public cGameState(SfmlApp controller)
        {
            appController = controller;
            //m_View = new View();
        }
        /// <summary>
        /// Az állapot induláskor kell végre hajtani
        /// </summary>
        public abstract void Enter();

        public abstract void BeforeUpdate();
        public abstract void Update(float step_time);

        public abstract void Render(RenderTarget destination, float alpha);

        public abstract void HandleSingleMouseClick(SFML.Window.MouseButtonEventArgs e);

        public abstract void HandleKeyPress(SFML.Window.KeyEventArgs e);

        /// <summary>
        /// Az állapot kilépésekor kell meghívni
        /// </summary>
        public abstract void Exit();

        public SfmlApp AppController
        {
            get { return appController; }
        }
    }
}
