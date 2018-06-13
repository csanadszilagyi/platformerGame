using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame.Particles
{

    /// <summary>
    /// This class attempts to handle particle effects effictively by using different controllers, which are
    /// operate on the particles stored in the particle pool.
    /// </summary>
    class cParticleManager
    {
        cGameScene scene;

        readonly Texture explosionTexture;
        readonly Texture fireworksTexture;

        cExplosionController explosions;
        cFireworksController fireworks;

        Text label;

        public cParticleManager(cGameScene scene)
        {
            this.scene = scene;

            explosionTexture = cAssetManager.GetTexture("simple_particle");
            fireworksTexture = cAssetManager.GetTexture("bullet3");

            explosions = new cExplosionController(this);
            fireworks = new cFireworksController(this);

            label = new Text();
            label.Position = new Vector2f(20, 45);

            label.Font = cAssetManager.GetFont("BGOTHL");
            label.CharacterSize = 24;
            label.FillColor = Color.White;
            label.Style = Text.Styles.Bold;

        }

        public void Update(float step_time)
        {
            fireworks.Update(step_time);
            explosions.Update(step_time);
        }

        public void PreRender(float alpha)
        {
            // if we want calculate viewPos before all renderings are started...
            fireworks.BuildVertexBuffer(alpha);
            explosions.BuildVertexBuffer(alpha);
        }

        public void Render(RenderTarget destination, float alpha)
        {
            //label.DisplayedString = "Active explosion particles: " + explosions.NumActive.ToString();
            //destination.Draw(label);
            fireworks.Render(destination, alpha);
            explosions.Render(destination, alpha);
        }

        public Texture ExplosionTexture
        {
            get { return explosionTexture; }
        }

        public Texture FireworksTexture
        {
            get { return fireworksTexture; }
        }

        public cGameScene Scene
        {
            get { return scene; }
        }

        public cExplosionController Explosions
        {
            get { return explosions; }
        }

        public cFireworksController Fireworks
        {
            get { return fireworks; }
        }
    }
}
