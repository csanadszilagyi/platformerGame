using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using platformerGame.App;

namespace platformerGame.Particles
{

    /// <summary>
    /// This class attempts to handle particle effects effictively by using different controllers, which are
    /// operate on the particles stored in the particle pool.
    /// </summary>
    class cParticleManager
    {
        GameScene sceneRef;

        readonly Texture explosionTexture;
        readonly Texture fireworksTexture;
        readonly Texture smokeTexture;

        Dictionary<string, cBaseParticleController> systems;

        Text label;

        public cParticleManager(GameScene scene)
        {
            this.sceneRef = scene;
            this.systems = new Dictionary<string, cBaseParticleController>();

            // must be called before controller additions
            explosionTexture = AssetManager.GetTexture("simple_particle");
            fireworksTexture = AssetManager.GetTexture("bullet3");
            smokeTexture = AssetManager.GetTexture("smoke_particle");

            this.systems.Add("explosions", new cExplosionController(this));
            this.systems.Add("fireworks", new cFireworksController(this));
            this.systems.Add("sprays", new cSprayController(this));

            label = new Text();
            label.Position = new Vector2f(20, 45);

            label.Font = AssetManager.GetFont("BGOTHL");
            label.CharacterSize = 24;
            label.FillColor = Color.White;
            label.Style = Text.Styles.Bold;

        }

        public void Update(float step_time)
        {
            foreach (var item in systems)
            {
                cBaseParticleController c = item.Value;
                c.Update(step_time);
            }
        }

        public void PreRender(float alpha)
        {
            // if we want calculate viewPos before all renderings are started...
            foreach (var item in systems)
            {
                cBaseParticleController c = item.Value;
                c.BuildVertexBuffer(alpha);
            }
        }

        public void Render(RenderTarget destination, float alpha)
        {
            //label.DisplayedString = "Active explosion particles: " + explosions.NumActive.ToString();
            //destination.Draw(label);
            foreach (var item in systems)
            {
                cBaseParticleController c = item.Value;
                c.Render(destination, alpha);
            }
        }

        public Texture ExplosionTexture
        {
            get { return explosionTexture; }
        }

        public Texture FireworksTexture
        {
            get { return fireworksTexture; }
        }

        public Texture SmokeTexture
        {
            get { return smokeTexture; }
        }

        public GameScene Scene
        {
            get { return sceneRef; }
        }

        public cBaseParticleController this[string key]
        {
            get 
            {
                cBaseParticleController s;
                return this.systems.TryGetValue(key, out s) ? s : null;
            }
        }
    }
}
