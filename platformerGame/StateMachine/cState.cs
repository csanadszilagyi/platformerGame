using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.StateMachine
{
    abstract class cState
    {
        public abstract void Enter();
        public abstract void Exit();
    }
}
