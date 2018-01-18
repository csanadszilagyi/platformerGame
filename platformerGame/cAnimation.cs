using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cAnimation
    {
	    Texture m_pTexture;
        List<IntRect> m_Frames;

        bool m_FlipHorizontally;
        bool m_FlipVertically;

        uint m_FrameTime; //in milliseconds
        uint m_StartFrame;

        int m_CurrentFrame;
        float m_CurrentAnimTime;
        float m_LastAnimTime;

        IntRect m_ViewOffsetRect; //view rect offsets inside animation rect
	    public cAnimation(Texture sprite_sheet, IntRect view_offset_rect = new IntRect())
        {
            m_pTexture = sprite_sheet;
            m_ViewOffsetRect = view_offset_rect;
            m_Frames = new List<IntRect>();
            m_FlipHorizontally = false;
            m_FlipVertically = false;
            m_CurrentFrame = 0;

            m_CurrentAnimTime = cGlobalClock.GetTimeInMilliseconds();
            
            m_LastAnimTime = m_CurrentAnimTime;
        }

        private bool _isReadyForNextFrame()
        {
            m_CurrentAnimTime = cGlobalClock.GetTimeInMilliseconds();
            float ft = this.GetFrameTime();

            return (m_CurrentAnimTime - m_LastAnimTime >= ft);
        }

        public void Update()
        {
            if (this.GetSize() > 1)
            {
                if (_isReadyForNextFrame())
                {
                    m_LastAnimTime = cGlobalClock.GetTimeInMilliseconds();

                    m_CurrentFrame += 1;

                    if (m_CurrentFrame >= this.GetSize()) //myCurrentFrame < 0
                    {
                        m_CurrentFrame = 0; //p_myCurrentAnim->GetSize()-1;
                    }
                }
           }
            else
                m_CurrentFrame = 0;
                
        }
        public void Render(RenderTarget destination, Vector2f pos) //IntRect view_rect = new IntRect()
        {
            IntRect frame = this.GetFrame(m_CurrentFrame);

            frame.Left += m_ViewOffsetRect.Left;
            frame.Top += m_ViewOffsetRect.Top;

            frame.Width = m_ViewOffsetRect.Width;
            frame.Height = m_ViewOffsetRect.Height;

            /*
            cRenderFunctions.DrawTextureSimple( destination,
                                                pos,
                                                this.GetSpriteSheet(),
                                                frame,
                                                Color.White,
                                                BlendMode.Alpha
                );
                */
            
            cRenderFunctions.DrawTexture(destination,
                        pos,
                        this.GetSpriteSheet(),
                        frame,
                        Color.White,
                        0.0f,
                        1.0f, //SCALE_FACTOR,
                        false, //p_myCurrentAnim->IsFlippedHorizontally(),
                        false,
                        BlendMode.Alpha,
                        null);
            
        }

        public void Clear()
        {
            m_Frames.Clear();
        }
	    public bool FlipHorizontally
        {
            get { return m_FlipHorizontally; }
            set { m_FlipHorizontally = value; }
        }
        public bool FlipVertically
        {
            get { return m_FlipVertically; }
            set { m_FlipVertically = value; }
        }

        public void AddFrame(IntRect rect)
        {
            m_Frames.Add(rect);
        }

        /*
        public void SetSpriteSheet(Texture texture)
        {
            m_pTexture = texture;
        }
        */
        public Texture GetSpriteSheet()
        {
            return m_pTexture;
        }
        public int GetSize()
        {
            return m_Frames.Count;
        }

        public IntRect GetCurrentFrame()
        {
            return GetFrame(m_CurrentFrame);
        }

        public IntRect GetFrame(int n)
        {
            //n = cAppMath.Clamp<int>(n, 0, GetSize());
            //n = n < 0 ? 0 : n >= GetSize() ? GetSize() : n;
           /* if (m_Frames.Count > 0)
                return m_Frames[n];
            else
                return new IntRect(0, 0, 32, 32);*/

            return m_Frames.Count > 0 ? m_Frames[n] : new IntRect(0,0,1,1);
        }

        public void SetStartFrame(uint start_frame) { m_StartFrame = start_frame; }
        public uint GetStartFrame()                    {return m_StartFrame;}

        public void SetFrameTime(uint frame_time) { m_FrameTime = frame_time; }
        public uint GetFrameTime()                   {return m_FrameTime;}

        ~cAnimation()
        {
            
        }
    }
}
