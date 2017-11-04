using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cLightSystem
    {
        private RenderTexture staticLightTexture;
        private RenderTexture dynamicLightTexture;

        private Shader m_AutenuationShader;

        private List<cLight> staticLights;

        private List<cLight> dynamicLights; // not used

        private Color m_ClearColor;

        private List<cLight> visibleLights = new List<cLight>();

        private Texture lightTexture;
        private Sprite lightSprite;

        private RectangleShape lightMapDarkShape;

        public cLightSystem(uint width, uint height, Color clear_color)
        {
            staticLights = new List<cLight>();
            dynamicLights = new List<cLight>();
            m_ClearColor = clear_color;
            lightMapDarkShape = new RectangleShape();
            lightMapDarkShape.Position = new Vector2f(0.0f, 0.0f);
            this.Create(width, height);

            loadShader();
        }
        public cLightSystem(Color clear_color)
        {
            lightMapDarkShape = new RectangleShape();
            lightMapDarkShape.Position = new Vector2f(0.0f, 0.0f);

            staticLights = new List<cLight>();
            dynamicLights = new List<cLight>();
            m_ClearColor = clear_color;

            lightTexture = new Texture(cAssetManager.GetTexture("light1"));
            lightTexture.Smooth = true;
            lightSprite = new Sprite(this.lightTexture);
            lightSprite.Origin = new Vector2f(lightTexture.Size.X / 2.0f, lightTexture.Size.Y / 2.0f);
            //lightSprite.Scale = new Vector2f(0.45f, 0.45f);
            loadShader();
        }

        public static cLight GetEnvironLight(Vector2f centre, float radius, Color color)
        {
            cLight light = new cLight(centre);
            light.LinearizeFactor = 0.99f;
            light.Radius = radius;
            light.Color = color;
            light.Bleed = 0.01f;
            return light;
        }
        public void Create(uint width, uint height)
        {
            lightMapDarkShape.Size = new Vector2f(width, height);
            lightMapDarkShape.FillColor = new Color(0, 0, 0, 50);

            staticLightTexture = new RenderTexture(width, height);
            staticLightTexture.SetActive(true);
            staticLightTexture.Clear(m_ClearColor);

            dynamicLightTexture = new RenderTexture(width, height);
            dynamicLightTexture.SetActive(true);
            dynamicLightTexture.Clear(m_ClearColor);
        }
        public void RemoveAll()
        {
            staticLights.Clear();
            staticLightTexture.Clear(m_ClearColor);
            dynamicLights.Clear();
            dynamicLightTexture.Clear(m_ClearColor);

        }
        public void AddStaticLight(cLight light)
        {
            staticLights.Add(light);
        }

        public void remove(cLight light)
        {
            staticLights.Remove(light);
        }
        public void AddDynamicLight(cLight light)
        {
            dynamicLights.Add(light);
        }
        private void loadShader()
        {
            
            m_AutenuationShader = new Shader(null, "Resources/lightAttenuationShader.frag");
            m_AutenuationShader.SetParameter("texture", Shader.CurrentTexture);
        }

        private void renderALight(cLight light, RenderTarget target, cAABB viewRect)
        {
            /*
            Vector3f glColor = new Vector3f(0, 0, 0);
            glColor.X = ((float)light.Color.R / 255.0f);
            glColor.Y = ((float)light.Color.G / 255.0f);
            glColor.Z = ((float)light.Color.B / 255.0f);
            */

            Vector2f newPos = new Vector2f(light.Pos.X - viewRect.topLeft.X, light.Pos.Y - viewRect.topLeft.Y);

            m_AutenuationShader.SetParameter("lightPos", newPos.X, target.Size.Y - newPos.Y);
            m_AutenuationShader.SetParameter("lightColor", light.GLcolor.X, light.GLcolor.Y, light.GLcolor.Z);
            m_AutenuationShader.SetParameter("radius", light.Radius);
            m_AutenuationShader.SetParameter("bleed", light.Bleed);
            m_AutenuationShader.SetParameter("linearizeFactor", light.LinearizeFactor);

            cRenderFunctions.DrawDirLightByDVec(target,
                                            newPos,
                                            light.Radius,
                                            light.Dir,
                                            light.SpreadAngle,
                                            light.Color,
                                            BlendMode.Add,
                                            m_AutenuationShader);
            
            /*
            Vector2f newPos = new Vector2f();
            newPos.X = light.Pos.X - viewRect.topLeft.X; //(light.Pos.X / defaultTarget.Size.X) * target.Size.X;
            newPos.Y = light.Pos.Y - viewRect.topLeft.Y;//(light.Pos.Y / defaultTarget.Size.Y) * target.Size.Y;

            lightSprite.Position = new Vector2f(light.Pos.X - viewRect.topLeft.X, light.Pos.Y - viewRect.topLeft.Y);
            lightSprite.Color = light.Color;

            float scale = 1.0f + (light.Radius / (lightTexture.Size.X /2.0f));
            lightSprite.Scale = new Vector2f(scale, scale);

            
            // BlendMode bm = new BlendMode(BlendMode.Factor.Zero, BlendMode.Factor.DstColor, BlendMode.Equation.Add, 
            //                            BlendMode.Factor.Zero, BlendMode.Factor.OneMinusSrcAlpha, BlendMode.Equation.Add);
            
            target.Draw(lightSprite, new RenderStates(BlendMode.Add));
            */

        }

        public void separateVisibleLights(cAABB viewRect)
        {
            visibleLights.Clear();

            float viewRadius = (float) Math.Sqrt( (viewRect.dims.X * viewRect.dims.X) + (viewRect.dims.Y * viewRect.dims.Y) );

            foreach (cLight light in staticLights)
            {
                if(cCollision.testCircleVsCirlceOverlap(light.Pos, light.Radius, viewRect.center, viewRadius))
                {
                    visibleLights.Add(light);
                }
            }
        }
        public void renderStaticLightsToTexture(cAABB viewRect)
        {
           staticLightTexture.Clear(m_ClearColor);

           foreach(cLight light in this.visibleLights)
           {
               renderALight(light, staticLightTexture, viewRect);
           }

           staticLightTexture.Display();
       
        }
        /*
        public void renderDynamicLightsToTexture()
        {
           dynamicLightTexture.Clear(m_ClearColor);

            for (int i = 0; i < dynamicLights.Count; ++i)
            {
                renderALight(dynamicLights[i], dynamicLightTexture);
            }

           dynamicLightTexture.Display();

        }
        */
        public void Render(RenderTarget destination, cAABB viewRect)
        {

            //destination.Draw(this.lightMapDarkShape, new RenderStates(BlendMode.Multiply));

            renderStaticLightsToTexture(viewRect);





            //cRenderFunctions.DrawTextureSimple(destination, new Vector2f(), m_LightTexture.Texture, new IntRect(0,0, (int)m_LightTexture.Size.X, (int)m_LightTexture.Size.Y),Color.White, BlendMode.Multiply);

            cRenderFunctions.DrawTextureSimple( destination, 
                                                viewRect.topLeft, 
                                                staticLightTexture.Texture, 
                                                new IntRect(0, 0, (int)this.staticLightTexture.Size.X, (int)this.staticLightTexture.Size.Y),
                                               //new IntRect((int)view_rect.topLeft.X, (int)view_rect.topLeft.Y,
                                                //            (int)view_rect.dims.X, (int)view_rect.dims.Y),
                                                Color.White,
                                                BlendMode.Multiply );

            
            /* renderDynamicLightsToTexture();

             cRenderFunctions.DrawTextureSimple(destination,
                                                   view_rect.topLeft,
                                                   dynamicLightTexture.Texture,
                                                   new IntRect((int)view_rect.topLeft.X, (int)view_rect.topLeft.Y,
                                                               (int)view_rect.dims.X, (int)view_rect.dims.Y),
                                                   Color.White,
                                                   BlendMode.Multiply);
            */

        }
    }
}
