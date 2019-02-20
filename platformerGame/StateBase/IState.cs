using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace platformerGame.StateBase
{
    interface IState<T>
    {
        void Create();
        void Enter(T prev);
        void UpdateFixed(float step_time);
        void UpdateVariable();
        void Exit();
    }

    interface IVisualStateComponent
    {
        void PreRender(float alpha);
        void Render(RenderTarget destination);
    }
}
