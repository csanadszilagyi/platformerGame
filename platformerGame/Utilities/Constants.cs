using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.IO;

namespace platformerGame.Utilities
{
    class Constants
    {
        public const float STEP_TIME = 1.0f / 60.0f;
        public const uint ANIM_FRAME_TIME = 30; //in ms

        public static int TILE_SIZE = 16;
        public static int TILE_SIZE_HALF = TILE_SIZE / 2;

        [JsonProperty("monster-walk-speed")]
        public static float     MONSTER_WALK_SPEED { get; private set; } //300.0f;

        [JsonProperty("player-walk-speed")]
        public static float PLAYER_WALK_SPEED { get; private set; } //300.0f;

        [JsonProperty("monster-max-walk-speed")]
        public static float     MONSTER_MAX_WALK_SPEED { get; private set; } //200.0f;

        [JsonProperty("player-max-walk-speed")]
        public static float PLAYER_MAX_WALK_SPEED { get; private set; } //200.0f;

        [JsonProperty("ground-slowdown-factor")]
        public static float     GROUND_SLOW_DOWN_FACTOR { get; private set; } //0.745f;

        [JsonProperty("air-slowdown-factor")]
        public static float     AIR_SLOW_DOWN_FACTOR { get; private set; }

        [JsonProperty("jump-speed")]
        public static float     JUMP_SPEED { get; private set; } //210 250 

        [JsonProperty("max-Y-speed")]
        public static float     MAX_Y_SPEED { get; private set; } //290.0f; //400

        [JsonProperty("gravity")]
        public static float     GRAVITY { get; private set; } //680  781.25f

        [JsonProperty("jump-gravity")]
        public static float     JUMP_GRAVITY { get; private set; } //212.5f; 260

        [JsonProperty("bullet-hit-force")]
        public static float     BULLET_HIT_FORCE { get; private set; }

        [JsonProperty("bullet-start-speed")]
        public static float BULLET_START_SPEED { get; private set; }

        [JsonProperty("bullet-texture-name")]
        public static string BULLET_TEXTURE_NAME { get; private set; }

        [JsonProperty("default-weapon-firing-frequency")]
        public static int DEFAULT_WEAPON_FIRING_FREQUENCY { get; private set; }

        [JsonProperty("friction")]
        public static float FRICTION { get; private set; } //0.02f;

        [JsonProperty("restitution")]
        public static float RESTITUTION { get; private set; } //0.1f;

        [JsonProperty("glue")]
        public static float GLUE { get; private set; } //0.01f;

        [JsonProperty("character-frame-width")]
        public static int CHAR_FRAME_WIDTH { get; private set; }

        [JsonProperty("character-frame-height")]
        public static int CHAR_FRAME_HEIGHT { get; private set; } //48

        [JsonProperty("player-texture-name")]
        public static string PLAYER_TEXTURE_NAME { get; private set; }

        [JsonProperty("monster-texture-name")]
        public static string MONSTER_TEXTURE_NAME { get; private set; }

        [JsonProperty("monster-max-health")]
        public static int MONSTER_MAX_HEALTH { get; private set; }

        [JsonProperty("character-collision-rect")]
        public static MyIntRect CHAR_COLLISON_RECT { get; private set; } // = new IntRect(10, 0, 22, 32); //32-10 = 22 | 10:left, 32:width

        [JsonProperty("character-view-rect")]
        public  static MyIntRect CHAR_VIEW_RECT { get; private set; } // = new IntRect(5, 0, 27, 32);

        [JsonProperty("lightmap-color")]
        public  static Color LIGHTMAP_COLOR { get; private set; }

        [JsonProperty("background-color")]
        public  static Color BACKGROUND_COLOR { get; private set; }

        [JsonProperty("font-names")]
        public static string[] FONT_NAMES { get; private set; }

        [JsonProperty("texture-names")]
        public static string[] TEXTURES_NAMES { get; private set; }

        [JsonProperty("sound-names")]
        public static string[] SOUND_NAMES { get; private set; }

        [JsonProperty("bg-texture")]
        public static string BG_TEXTURE { get; private set; }

        static Constants()
        {
        }

        public static void Load()
        {
            if (File.Exists("constants.json"))
            {
                string json = File.ReadAllText(@"constants.json");
                FromJson(json);
            }
            else
                throw new FileNotFoundException("The constants.json file is missing.");
        }

        private static Constants FromJson(string json) => JsonConvert.DeserializeObject<Constants>(json, new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
            }
        );
    }
}
