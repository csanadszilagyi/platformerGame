using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using platformerGame.Utilities;

namespace platformerGame.Rendering
{
    class AnimationInfo
    {
        public Texture refTexture { get; set; }
        public List<MyIntRect> Frames { get; set; }
        public MyIntRect ViewOffsetRect { get; set; }
        public bool Repeat { get; set; }

        public AnimationInfo(string texture_name, IEnumerable<MyIntRect> frames, MyIntRect view_offset, bool repeat = false)
        {
            this.refTexture = AssetManager.GetTexture(texture_name);
            this.Frames = new List<MyIntRect>(frames);
            this.ViewOffsetRect = view_offset;
            this.Repeat = repeat;
        }

        public AnimationInfo(Texture texture, IEnumerable<MyIntRect> frames, MyIntRect view_offset, bool repeat = false)
        {
            this.refTexture = texture;
            this.Frames = new List<MyIntRect>(frames);
            this.ViewOffsetRect = view_offset;
            this.Repeat = repeat;
        }

        public AnimationInfo(AnimationInfo other)
        {
            this.refTexture = other.refTexture;
            this.Frames = new List<MyIntRect>(other.Frames.ToArray());
            this.ViewOffsetRect = other.ViewOffsetRect;
            this.Repeat = other.Repeat;
        }

        public void setFramesByRef(List<MyIntRect> ref_frames)
        {
            this.Frames = ref_frames;
        }

        public AnimationInfo DeepCopy()
        {
            return new AnimationInfo(this);
        }
    }

    class cAnimationAssets
    {
        static Dictionary<string, AnimationInfo> framesContainer = new Dictionary<string, AnimationInfo>();

        static cAnimationAssets()
        {
            // framesContainer 
        }

        public static IEnumerable<MyIntRect> getLoopedFull(int rows, int columns, int frame_width, int frame_height)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    yield return new MyIntRect(j * frame_width, i * frame_height, frame_width, frame_height);
                }
            }
        }

        public static IEnumerable<MyIntRect> getLoopedRow(int start_row, int start_column, int num_frames, int frame_width, int frame_height)
        {
            for (int f = 0; f < num_frames; f++)
            {
                yield return new MyIntRect((start_column + f) * frame_width, start_row * frame_height, frame_width, frame_height);
            }
        }

        public static void LoadAnimations()
        {
            framesContainer.Add("coins-gold", new AnimationInfo("coins", getLoopedRow(0, 0, 8, 16, 16), new MyIntRect(0,0,16,16)));
            framesContainer.Add("coins-silver", new AnimationInfo("coins", getLoopedRow(1, 0, 8, 16, 16), new MyIntRect(0, 0, 16, 16)));
            framesContainer.Add("coins-copper", new AnimationInfo("coins", getLoopedRow(2, 0, 8, 16, 16), new MyIntRect(0, 0, 16, 16)));
            framesContainer.Add("side-explosion1", new AnimationInfo("side_explosion1", getLoopedFull(3, 5, 96, 96), new MyIntRect(0, 0, 96, 96)));
            framesContainer.Add("simple-explosion1", new AnimationInfo("explosion32_B", getLoopedFull(4, 4, 32, 32), new MyIntRect(0, 0, 32, 32)));
            framesContainer.Add("simple-explosion2", new AnimationInfo("explosion32", getLoopedFull(4, 4, 32, 32), new MyIntRect(0, 0, 32, 32)));
            framesContainer.Add("simple-explosion3", new AnimationInfo("explosion196_C", getLoopedFull(4, 4, 49, 49), new MyIntRect(0, 0, 49, 49)));
        }

        public static void ClearAll()
        {
            framesContainer.Clear();
        }

        public static AnimationInfo Get(string id)
        {
            AnimationInfo inf;
            if(framesContainer.TryGetValue(id, out inf))
            {
                return inf;
            }

            return null;
            
        }
    }
}
