using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SFML.Graphics;

namespace platformerGame.Rendering
{
    class AnimationInfo
    {
        public string TextureName { get; set; }
        public List<IntRect> Frames { get; set; }

        public AnimationInfo(string texture_name, List<IntRect> frames)
        {
            this.TextureName = texture_name;
            this.Frames = frames;
        }
    }

    class cAnimationAssets
    {
        static Dictionary<string, AnimationInfo> framesContainer;

        static cAnimationAssets()
        {
            framesContainer = new Dictionary<string, AnimationInfo>();
        }

        static List<IntRect> getLoopedFull(int rows, int columns, int frame_width, int frame_height)
        {
            List<IntRect> rects = new List<IntRect>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; i++)
                {

                    rects.Add(new IntRect(j * frame_width, i * frame_height, frame_width, frame_height));
                }
            }
            return rects;
        }

        static List<IntRect> getLooped(int start_row, int start_column, int num_frames, int frame_width, int frame_height)
        {
            List<IntRect> frames = new List<IntRect>();

            for (int f = 0; f < num_frames; f++)
            {
                frames.Add(new IntRect((start_column + f) * frame_width, start_row * frame_height, frame_width, frame_height));
            }

            return frames;
        }

        public static void LoadAnimations()
        {

            framesContainer.Add("coins-gold", new AnimationInfo("coins", getLooped(0, 0, 8, 16, 16)));
            framesContainer.Add("coins-silver", new AnimationInfo("coins", getLooped(1, 0, 8, 16, 16)));
            framesContainer.Add("coins-copper", new AnimationInfo("coins", getLooped(2, 0, 8, 16, 16)));
        }

        public static void ClearAll()
        {
            framesContainer.Clear();
        }

        public static AnimationInfo Get(string id)
        {
            // @TODO: tryGetValue
            return framesContainer[id];
        }
    }
}
