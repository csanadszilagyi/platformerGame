using SFML.Graphics;

namespace platformerGame
{
    public interface IDrawable
    {
        //bool IsVisibleOnScreen();
        void CalculateViewPos(float alpha);
        void Render(RenderTarget destination);
    }
}
