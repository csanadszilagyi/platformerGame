using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace platformerGame
{
    class cAssetManager
    {
        private static Dictionary<string, Texture> textures;
        private static Dictionary<string, Font> fonts;
        static cAssetManager()
        {
            textures = new Dictionary<string, Texture>();
            fonts = new Dictionary<string, Font>();
        }

        public static void LoadResources()
        {
            LoadFont("Resources/Fonts/BGOTHL.TTF");
            LoadFont("Resources/Fonts/pf_tempesta_seven.ttf");
            
            /*
            LoadTexture("Resources/textures/Bullet2.tga");

            LoadTexture("Resources/textures/bkBlue.bmp");
            LoadTexture("Resources/textures/TileSet2.tga");

            LoadTexture("Resources/textures/fire1.png");
            */

            LoadTexture("Resources/textures/player1_char_set.png");
            LoadTexture("Resources/textures/player1_char_set_48.png");
            LoadTexture("Resources/textures/monster1_char_set.png");
            LoadTexture("Resources/textures/bkBlue.bmp");
            LoadTexture("Resources/textures/wall.png");
            LoadTexture("Resources/textures/wall_smaller.bmp");
            LoadTexture("Resources/textures/door1_sm.png");
            LoadTexture("Resources/textures/tileSet_32.png");
            LoadTexture("Resources/textures/tileSet_16.png");
            LoadTexture("Resources/textures/bullet2.png");
            LoadTexture("Resources/textures/bullet3.png");
            LoadTexture("Resources/textures/bullet4.png");
            LoadTexture("Resources/textures/bullet4_B.png");
            LoadTexture("Resources/textures/bullet_yellow.png");
            LoadTexture("Resources/textures/bullet_yellow_sm.png");
            LoadTexture("Resources/textures/bullet_light_green.png");
            LoadTexture("Resources/textures/light1.png");
            LoadTexture("Resources/textures/simple_particle.tga");
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
