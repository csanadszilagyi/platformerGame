using SFML.System;

namespace platformerGame.GameCommands
{
    class comLittleBloodExplosion : cBaseGameCommand
    {
        Vector2f pos;
        public comLittleBloodExplosion(cGameScene scene, Vector2f pos) : base(scene)
        {
            this.pos = pos;
        }

        public override void Execute()
        {
            scene.ParticleManager.Explosions.LittleBlood(new Particles.cEmissionInfo(pos));
        }
    }
}
