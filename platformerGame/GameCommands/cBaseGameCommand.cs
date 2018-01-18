using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.GameCommands
{
    abstract class cBaseGameCommand
    {
        protected cGameScene scene;

        public cBaseGameCommand(cGameScene scene)
        {
            this.scene = scene;
        }
        public abstract void Execute();
    }
}
