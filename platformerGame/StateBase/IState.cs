using SFML.Graphics;

namespace platformerGame.StateBase
{
    interface IState
    {
        void Create();
        void Enter(IState prev_state = null);
        void UpdateFixed(float step_time);
        void UpdateVariable();
        void Exit();
    }

    interface IVisualStateComponent
    {
        // rendering calculations, ordering (eg. view position calculation...etc)
        void PreRender(float alpha);
        void Render(RenderTarget destination);
    }
}
