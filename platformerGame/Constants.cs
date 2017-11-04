using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace platformerGame
{
    class Constants
    {
        public const int       TILE_SIZE                    = 16;
        public const int       TILE_SIZE_HALF               = TILE_SIZE / 2;
        public const float     WALK_SPEED                   = 250.0f; //300.0f;
        public const float     MAX_WALK_SPEED               = 150.0f; //200.0f;
        public const float     GROUND_SLOW_DOWN_FACTOR      = 0.6f; //0.745f;
        public const float     AIR_SLOW_DOWN_FACTOR         = 0.95f;
        public const float     JUMP_SPEED                   = 300.0f; //210 250 
        public const float     MAX_Y_SPEED                  = 400.0f; //290.0f; //400
        public const float     GRAVITY                      = 881.25f; //680  781.25f
        public const float     JUMP_GRAVITY                 = 460; //212.5f; 260

        public const float     BULLET_HIT_FORCE             = 1000;

        public const float FRICTION = 0.1f; //0.02f;
        public const float RESTITUTION = 0.5f; //0.1f;
        public const float GLUE = 0.001f; //0.01f;

        public const int CHAR_FRAME_WIDTH =  32;
        public const int CHAR_FRAME_HEIGHT = 32; //48

        public const string PLAYER_TEXTURE_NAME = "player1_char_set";
        public const string MONSTER_TEXTURE_NAME = "monster1_char_set";

        public static readonly IntRect CHAR_COLLISON_RECT; // = new IntRect(10, 0, 22, 32); //32-10 = 22 | 10:left, 32:width
        public static readonly IntRect CHAR_VIEW_RECT; // = new IntRect(5, 0, 27, 32);

        public static readonly Color LIGHTMAP_COLOR = new Color(50, 50, 50, 255);
        public static readonly Color BACKGROUND_COLOR = Color.White;
        static Constants()
        {
            //32*32
            
            CHAR_COLLISON_RECT = new IntRect(10, 0, 22, 32); //32-10 = 22 | 10:left, 32:width
            CHAR_VIEW_RECT = new IntRect(5, 0, 27, 32);
            

            //32*48
            /*
            CHAR_COLLISON_RECT = new IntRect(10, 0, 22, 48); //32-10 = 22 | 10:left, 32:width
            CHAR_VIEW_RECT = new IntRect(5, 0, 27, 48);
            */
        }

    }
}
