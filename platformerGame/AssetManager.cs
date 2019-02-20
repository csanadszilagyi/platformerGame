using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SFML.Graphics;
using SFML.Audio;
using SFML.System;

using platformerGame.Utilities;

namespace platformerGame
{
    class Assets
    {
        List<string> textureNames;
        List<string> fontNames;
        List<string> soundNames;
    }

    class AssetManager
    {
        public const string ROOT_PATH = "Resources/";
        public const string TEX_RESOURCE_PATH = "textures/";
        public const string TILESET_PATH = TEX_RESOURCE_PATH + "tilesets/";
        public const string FONT_RESOURCE_PATH = "fonts/";
        public const string SOUND_RESOURCE_PATH = "sounds/";

        private static Dictionary<string, Texture> textures;
        private static Dictionary<string, Font> fonts;
        private static Dictionary<string, SoundBuffer> soundBuffers;
        private static List<Sound> sounds;

        static AssetManager()
        {
            textures = new Dictionary<string, Texture>();
            fonts = new Dictionary<string, Font>();
            soundBuffers = new Dictionary<string, SoundBuffer>();
            sounds = new List<Sound>();
        }

        /// <summary>
        /// Loads game resource files.
        /// </summary>
        public static void LoadResources()
        {
            //loading fonts
            foreach (string fontName in Constants.FONT_NAMES)
            {
                LoadFont(fontName);
            }

            //loadin textures
            foreach (string texName in Constants.TEXTURES_NAMES)
            {
                LoadTexture(texName);
            }

            LoadSound("shotgun.wav");
            LoadSound("rifle.wav");
            LoadSound("cg1.wav");
            LoadSound("pistol.wav");

            LoadSound("blood_hit1.wav");
            LoadSound("blood_hit2.wav");
            LoadSound("coin40.wav");
            LoadSound("coin41.wav");
            LoadSound("coin_pickup1.wav");
            LoadSound("coin_drop1.wav");

            LoadSound("weapon_blow2.wav");
            LoadSound("wallhit.wav");
            LoadSound("body_hit1.wav");
        }

        public static string GenerateIDFromFilePath(string file_path)
        {
            file_path = file_path.Replace('\\', '/');
            int start = file_path.LastIndexOf('/')+1;
            int len = Math.Abs(file_path.LastIndexOf('.') - start);
            return file_path.Substring(start, len);
        }

        public static void LoadFont(string file_name)
        {
            string path = Path.Combine(ROOT_PATH, FONT_RESOURCE_PATH, file_name);
            Font f = new Font(path);

            string id_name = GenerateIDFromFilePath(file_name);

            fonts.Add(id_name, f);
        }
 
        public static void LoadTexture(string file_name)
        {
            string path = Path.Combine(ROOT_PATH, TEX_RESOURCE_PATH, file_name);
            Texture t = new Texture(path);
            t.Smooth = true;
            string id_name = GenerateIDFromFilePath(file_name);
            if(false == textures.ContainsKey(id_name))
            {
                textures.Add(id_name, t);
            }
        }

        public static void LoadSound(string file_name)
        {
            string path = Path.Combine(ROOT_PATH, SOUND_RESOURCE_PATH, file_name);
            SoundBuffer buffer; // = new SoundBuffer(path);
            string id_name = GenerateIDFromFilePath(file_name);

            if( !soundBuffers.TryGetValue(id_name, out buffer))
            {
                buffer = new SoundBuffer(path);
                soundBuffers.Add(id_name, buffer);
            }
        }

        public static Texture LoadAndReturnTexture(string file_name)
        {
            Texture t = new Texture(file_name);
            t.Smooth = true;
            string id_name = GenerateIDFromFilePath(file_name);
            textures.Add(id_name, t);
            return t;
        }

        public static Font GetFont(string id)
        {
            return fonts[id];
        }
        public static Texture GetTexture(string id)
        {
            return textures[id];
        }

        public static void playSound(string id, int volume = 50, Vector2f pos = default(Vector2f))
        {
            Sound s = new Sound(soundBuffers[id]);
            
            s.Volume = volume;
            s.RelativeToListener = true;
            s.Position = new Vector3f(pos.X, 0.0f, pos.Y);
            //s.MinDistance = 5.0f;
            //s.Attenuation = 10.0f;
            s.Play();

            //sounds.Find(snd => snd.SoundBuffer == soundBuffers[id]);

            sounds.Add(s);
            sounds.RemoveAll(snd => snd.Status != SoundStatus.Playing);
        }

        public static void ClearAll()
        {
            textures.Clear();
            fonts.Clear();
            soundBuffers.Clear();
            sounds.Clear();
        }

    }
}
