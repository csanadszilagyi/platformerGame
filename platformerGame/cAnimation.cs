using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using platformerGame.Rendering;

namespace platformerGame
{
    class cAnimation
    {
	    Texture pTexture;
        List<IntRect> mFrames;

        bool m_FlipHorizontally;
        bool m_FlipVertically;

        uint m_FrameTime; //in milliseconds
        uint m_StartFrame;

        int m_CurrentFrame;
        double m_CurrentAnimTime;
        double m_LastAnimTime;

        IntRect viewOffsetRect; //view rect offsets inside animation rect

        AnimationInfo animInfo;

	    public cAnimation(Texture sprite_sheet, IntRect view_offset_rect = new IntRect())
        {
            pTexture = sprite_sheet;
            viewOffsetRect = view_offset_rect;
            this.mFrames = new List<IntRect>();
            init();
        }

        public cAnimation(AnimationInfo info, IntRect view_offset_rect = new IntRect())
        {
            this.animInfo = info;
            pTexture = AssetManager.GetTexture(info.TextureName);
            viewOffsetRect = view_offset_rect;
            this.SetFrames(info.Frames);
            init();
        }

        public void init()
        {
            m_FlipHorizontally = false;
            m_FlipVertically = false;
            m_CurrentFrame = 0;

            m_CurrentAnimTime = cGlobalClock.GetTimeInMilliseconds();
            m_FrameTime = platformerGame.Utilities.Constants.ANIM_FRAME_TIME;
            m_LastAnimTime = m_CurrentAnimTime;
        }

        private bool _isReadyForNextFrame()
        {
            m_CurrentAnimTime = cGlobalClock.GetTimeInMilliseconds();
            return (m_CurrentAnimTime - m_LastAnimTime >= m_FrameTime);
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

        public void RenderCentered(RenderTarget destination, Vector2f pos)
        {
            IntRect frame = this.GetFrame(m_CurrentFrame);

            frame.Left += viewOffsetRect.Left;
            frame.Top += viewOffsetRect.Top;

            frame.Width = viewOffsetRect.Width;
            frame.Height = viewOffsetRect.Height;

            DrawingBase.DrawTextureUseCenter(destination,
                       pos,
                       this.GetTexture(),
                       frame,
                       Color.White,
                       0.0f,
                       1.0f, //SCALE_FACTOR,
                       false, //p_myCurrentAnim->IsFlippedHorizontally(),
                       false,
                       BlendMode.Alpha,
                       null);
        }

        public void Render(RenderTarget destination, Vector2f pos) //IntRect view_rect = new IntRect()
        {
            IntRect frame = this.GetFrame(m_CurrentFrame);

            frame.Left += viewOffsetRect.Left;
            frame.Top += viewOffsetRect.Top;

            frame.Width = viewOffsetRect.Width;
            frame.Height = viewOffsetRect.Height;

            /*
            cRenderFunctions.DrawTextureSimple( destination,
                                                pos,
                                                this.GetSpriteSheet(),
                                                frame,
                                                Color.White,
                                                BlendMode.Alpha
                );
                */
            
  
             DrawingBase.DrawTexture(destination,
                            pos,
                            this.GetTexture(),
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
            this.mFrames.Clear();
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
            mFrames.Add(rect);
        }

        public void SetFrames(List<IntRect> frames)
        {
            //this.Clear();
            //this.frames.AddRange(frames);
            this.mFrames = frames;
        }

        /*
        public void SetSpriteSheet(Texture texture)
        {
            m_pTexture = texture;
        }
        */
        public Texture GetTexture()
        {
            return pTexture;
        }

        public IntRect GetViewOffsetRect()
        {
            return this.viewOffsetRect;
        }

        public AnimationInfo GetAnimInfo()
        {
            return this.animInfo;
        }

        public int GetSize()
        {
            return mFrames.Count;
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

            return mFrames.Count > 0 ? mFrames[n] : new IntRect(0,0,1,1);
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
