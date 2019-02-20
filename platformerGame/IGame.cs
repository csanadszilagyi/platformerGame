using SFML.Graphics;
using SFML.Window;

namespace OverlordEngine
{
    /*
    delegate void BeforeUpdateEvent();
    delegate void UpdateEvent(float stepTime);
    delegate void PreRenderEvent(float alpha);
    delegate void RenderEvent(RenderTarget target);
    */

    interface IGame
    {
        void Init();
        void Destroy();
        void OnSingleMouseClick(MouseButtonEventArgs e);
        void OnKeyPressed(KeyEventArgs e);
        void OnBeforeUpdate();
        void OnUpdateFixed(float stepTime);
        void OnUpdateVariable(float stepTime = 1.0f);
        void OnPreRender(float alpha);
        void OnRender(RenderTarget target);
    }

    class PlatformerGame : IGame
    {
        public PlatformerGame()
        {
        }


        public void OnBeforeUpdate()
        {
            
        }

        public void OnPreRender(float alpha)
        {
            
        }

        public void OnRender(RenderTarget target)
        {
           
        }

        public void OnUpdateFixed(float stepTime)
        {
            
        }

        public void OnUpdateVariable(float stepTime = 1.0f)
        {
            
        }
    }
}
