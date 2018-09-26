using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.System;

namespace platformerGame.GameCommands
{
    class comNormalFireworks : cBaseGameCommand
    {
        Vector2f pos;
        public comNormalFireworks(cGameScene scene, Vector2f pos) : base(scene)
        {
            this.pos = pos;
        }

        public override void Execute()
        {
            //scene.ParticleManager.Fireworks.NormalExplosion(new Particles.cEmissionInfo(pos));
        }
    }
}
