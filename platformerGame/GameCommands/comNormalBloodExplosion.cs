using SFML.System;

namespace platformerGame.GameCommands
{
    class comNormalBloodExplosion : cBaseGameCommand
    {
        Vector2f pos;
        public comNormalBloodExplosion(cGameScene scene, Vector2f pos) : base(scene)
        {
            this.pos = pos;
        }

        public override void Execute()
        {
            scene.ParticleManager.Explosions.NormalBlood(pos);
        }
    }
}
