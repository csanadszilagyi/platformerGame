using SFML.System;
using platformerGame.Particles;

namespace platformerGame.GameCommands
{
    class comNormalBloodExplosion : cBaseGameCommand
    {
        cEmissionInfo emission;
        public comNormalBloodExplosion(cGameScene scene, cEmissionInfo emission) : base(scene)
        {
            this.emission = emission;
        }

        public override void Execute()
        {
            scene.ParticleManager.Explosions.NormalBlood(emission);
        }
    }
}
