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
    class AssetContext
    {
        public string[] FontNames { get; private set; }
        public string[] TextureNames { get; private set; }
        public string[] SoundNames { get; private set; }

        public Dictionary<string, Texture> Textures { get; private set; }
        public Dictionary<string, Font> Fonts { get; private set; }
        public Dictionary<string, SoundBuffer> SoundBuffers { get; private set; }
        public List<Sound> Sounds { get; private set; }

        public AssetContext()
        {
            Textures = new Dictionary<string, Texture>();
            Fonts = new Dictionary<string, Font>();
            SoundBuffers = new Dictionary<string, SoundBuffer>();
            Sounds = new List<Sound>();
        }

        public void LoadResources(string[] font_names = null, string[] textures_names = null, string[] sound_names = null)
        {
            // return Task.Factory.StartNew(() => { });
            this.ClearResources();

            this.FontNames = font_names;
            this.TextureNames = textures_names;
            this.SoundNames = sound_names;

            //loading fonts
            if (this.FontNames != null)
            {
                foreach (string fontName in this.FontNames)
                {
                    Tuple<string, Font> data = AssetLoader.LoadFont(fontName);
                    this.TryAddFont(data);
                }
            }

            //loading textures
            if (this.TextureNames != null)
            {
                foreach (string texName in this.TextureNames)
                {
                    Tuple<string, Texture> data = AssetLoader.LoadTexture(texName);
                    this.TryAddTexture(data);
                }
            }


            //loading sounds
            if (this.SoundNames != null)
            {
                foreach (string soundName in this.SoundNames)
                {
                    Tuple<string, SoundBuffer> data = AssetLoader.LoadSound(soundName);
                    this.TryAddSound(data);
                }
            }
        }

        public void TryAddFont(Tuple<string, Font> font_data)
        {
            if (false == Fonts.ContainsKey(font_data.Item1))
            {
                Fonts.Add(font_data.Item1, font_data.Item2);
            }
        }

        /*
        public string TryAddTexture(string path)
        {
            string id_name = AssetManager.GenerateIDFromFilePath(path);
            Texture t = AssetManager.LoadAndReturnTexture(path);
            this.TryAddTexture(new Tuple<string, Texture>(id_name, t));
            return id_name;
        }
        */

        public void TryAddTexture(Tuple<string, Texture> texture_data)
        {
            if (false == Textures.ContainsKey(texture_data.Item1))
            {
                Textures.Add(texture_data.Item1, texture_data.Item2);
            }
        }

        public void TryAddSound(Tuple<string, SoundBuffer> sound_data)
        {
            if (false == SoundBuffers.ContainsKey(sound_data.Item1))
            {
                SoundBuffers.Add(sound_data.Item1, sound_data.Item2);
            }
        }

        public Font GetFont(string id)
        {
            Font f;
            if (Fonts.TryGetValue(id, out f))
            {
                return f;
            }

            return null;
        }

        public Texture GetTexture(string id)
        {
            Texture t;
            if (Textures.TryGetValue(id, out t))
            {
                return t;
            }

            return null;
        }

        public void PlaySound(string id, int volume = 50, Vector2f pos = default(Vector2f))
        {
            Sound s = new Sound(SoundBuffers[id]);

            s.Volume = volume;
            s.RelativeToListener = true;
            s.Position = new Vector3f(pos.X, 0.0f, pos.Y);
            //s.MinDistance = 5.0f;
            //s.Attenuation = 10.0f;
            s.Play();

            //sounds.Find(snd => snd.SoundBuffer == soundBuffers[id]);

            Sounds.Add(s);
            Sounds.RemoveAll(snd => snd.Status != SoundStatus.Playing);
        }

        public void ClearResources()
        {
            Textures.Clear();
            Fonts.Clear();
            SoundBuffers.Clear();
            Sounds.Clear();
        }

        public void Mix(AssetContext other_context)
        {

        }
    }

    class AssetLoader
    {
        public const string ROOT_PATH = "Resources/";
        public const string TEX_RESOURCE_PATH = "textures/";
        public const string TILESET_PATH = TEX_RESOURCE_PATH + "tilesets/";
        public const string FONT_RESOURCE_PATH = "fonts/";
        public const string SOUND_RESOURCE_PATH = "sounds/";


        static AssetLoader()
        {
        }

        public static string GenerateIDFromFilePath(string file_path)
        {
            file_path = file_path.Replace('\\', '/');
            int start = file_path.LastIndexOf('/')+1;
            int len = Math.Abs(file_path.LastIndexOf('.') - start);
            return file_path.Substring(start, len);
        }

        public static Tuple<string, Font> LoadFont(string file_name)
        {
            string path = Path.Combine(ROOT_PATH, FONT_RESOURCE_PATH, file_name);
            Font f = new Font(path);

            string id_name = GenerateIDFromFilePath(file_name);

            return new Tuple<string, Font>(id_name, f);
        }

        public static Tuple<string, SoundBuffer> LoadSound(string file_name)
        {
            string path = Path.Combine(ROOT_PATH, SOUND_RESOURCE_PATH, file_name);
            SoundBuffer buffer; // = new SoundBuffer(path);
            string id_name = GenerateIDFromFilePath(file_name);
            buffer = new SoundBuffer(path);
            return new Tuple<string, SoundBuffer>(id_name, buffer);
        }

        public static Tuple<string, Texture> LoadTexture(string file_name)
        {
            string path = Path.Combine(ROOT_PATH, TEX_RESOURCE_PATH, file_name);
            Texture t = new Texture(path);
            t.Smooth = true;
            string id_name = GenerateIDFromFilePath(file_name);

            return new Tuple<string, Texture>(id_name, t);
        }

        /// <summary>
        /// Just in case inline loader...
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public static Texture LoadAndReturnTexture(string file_name)
        {
            Texture t = new Texture(file_name);
            t.Smooth = true;
            return t;
        }

    }
}
