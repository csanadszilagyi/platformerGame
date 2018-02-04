using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

using platformerGame.Utilities;

namespace platformerGame
{
    class cAssetManager
    {
        private const string TEX_RESOURCE_PATH = "Resources/textures/";
        private const string FONT_RESOURCE_PATH = "Resources/Fonts/";

        private static Dictionary<string, Texture> textures;
        private static Dictionary<string, Font> fonts;
        static cAssetManager()
        {
            textures = new Dictionary<string, Texture>();
            fonts = new Dictionary<string, Font>();
        }

        /// <summary>
        /// Loads game resource files.
        /// </summary>
        public static void LoadResources()
        {
            //loading fonts
            foreach (string fontName in Constants.FONT_NAMES)
            {
                LoadFont(FONT_RESOURCE_PATH + fontName);
            }

            //loadin textures
            foreach (string texName in Constants.TEXTURES_NAMES)
            {
                LoadTexture(TEX_RESOURCE_PATH + texName);
            }
        }

        public static string GenerateIDFromFilePath(string file_path)
        {
            file_path = file_path.Replace('\\', '/');
            int start = file_path.LastIndexOf('/')+1;
            int len = Math.Abs(file_path.LastIndexOf('.') - start);
            return file_path.Substring(start, len);
        }

        private static void LoadFont(string file_name)
        {
            Font f = new Font(file_name);

            string id_name = GenerateIDFromFilePath(file_name);

            fonts.Add(id_name, f);
        }

        private static void LoadTexture(string file_name)
        {
            Texture t = new Texture(file_name);
            t.Smooth = true;
            string id_name = GenerateIDFromFilePath(file_name);
 
            textures.Add(id_name, t);
        }

        public static Font GetFont(string id)
        {
            return fonts[id];
        }
        public static Texture GetTexture(string id)
        {
            return textures[id];
        }

        public static void Destroy()
        {
            textures.Clear();
            fonts.Clear();
        }

    }
}
