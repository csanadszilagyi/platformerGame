using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;
using platformerGame.Rendering;
using platformerGame.Utilities;

namespace platformerGame
{
    class cAnimation
    {
        AnimationInfo animData;

        bool m_FlipHorizontally;
        bool m_FlipVertically;

        uint m_FrameTime; //in milliseconds
        uint m_StartFrame;

        int m_CurrentFrame;
        double m_CurrentAnimTime;
        double m_LastAnimTime;
        
       // MyIntRect viewOffsetRect; //view rect offsets inside animation rect

        public bool Active { get; set; }

	    public cAnimation(Texture sprite_sheet, MyIntRect view_offset_rect)
        {
            animData = new AnimationInfo(sprite_sheet, new List<MyIntRect>(), view_offset_rect);
            init();
        }

        public cAnimation(AnimationInfo ref_anim)
        {
            this.animData = ref_anim;
            this.SetFrames(ref_anim.Frames);
            init();
        }

        public void init()
        {
            m_FlipHorizontally = false;
            m_FlipVertically = false;
            m_CurrentFrame = 0;
            Active = true;
            m_CurrentAnimTime = cGlobalClock.GetTimeInMilliseconds();
            m_FrameTime = Constants.ANIM_FRAME_TIME;
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
                        if(this.animData.Repeat)
                        {
                            m_CurrentFrame = 0;
                            return;
                        }
                        m_CurrentFrame = this.GetSize() - 1;
                        Active = false;
                    }
                }
           }
            else
                m_CurrentFrame = 0;
                
        }

        public void RenderCentered(RenderTarget destination, Vector2f pos)
        {
            MyIntRect frame = this.GetCurrentFrame().DeepCopy();

            frame.Left += this.animData.ViewOffsetRect.Left;
            frame.Top += this.animData.ViewOffsetRect.Top;

            frame.Width = this.animData.ViewOffsetRect.Width;
            frame.Height = this.animData.ViewOffsetRect.Height;

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

        public void Render(RenderTarget destination, Vector2f pos) //MyIntRect view_rect = new MyIntRect()
        {
            MyIntRect frame = this.GetCurrentFrame().DeepCopy(); //.DeepCopy is very important for drawing!!

            frame.Left += this.animData.ViewOffsetRect.Left;
            frame.Top += this.animData.ViewOffsetRect.Top;

            frame.Width = this.animData.ViewOffsetRect.Width;
            frame.Height = this.animData.ViewOffsetRect.Height;

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
            this.animData.Frames.Clear();
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

        public void AddFrame(MyIntRect rect)
        {
            this.animData.Frames.Add(rect);
        }

        public void SetFrames(List<MyIntRect> frames)
        {
            this.animData.Frames = new List<MyIntRect>(frames);
        }

        /*
        public void SetSpriteSheet(Texture texture)
        {
            m_pTexture = texture;
        }
        */
        public Texture GetTexture()
        {
            return animData.refTexture;
        }

        public AnimationInfo AnimData
        {
            get { return this.animData; }
        }

        public int GetSize()
        {
            return this.animData.Frames.Count;
        }

        public MyIntRect GetCurrentFrame()
        {
            return GetFrame(m_CurrentFrame);
        }

        public MyIntRect GetFrame(int n)
        {
            //n = cAppMath.Clamp<int>(n, 0, GetSize());
            //n = n < 0 ? 0 : n >= GetSize() ? GetSize() : n;
           /* if (m_Frames.Count > 0)
                return m_Frames[n];
            else
                return new MyIntRect(0, 0, 32, 32);*/

            return animData.Frames.Count > 0 ? animData.Frames[n] : new MyIntRect(0,0,1,1);
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
