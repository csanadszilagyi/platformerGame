using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cSpriteStateController
    {
        Dictionary<cSpriteState, cAnimation> m_SpriteStates;

        cSpriteState m_PrevState;
        cSpriteState m_CurrentState;

        cAnimation m_pCurrentAnim;

        public cSpriteStateController()
        {
            m_SpriteStates = new Dictionary<cSpriteState, cAnimation>();
            m_pCurrentAnim = null;
        }

        public void AddAnimState_by_Full_Texture( cSpriteState state_type,
										Texture texture,
										int a_frame_width,
                                        int a_frame_height,
                                        int num_columns_of_frames,
                                        int num_rows_of_frames,
                                        uint frame_time )
        {
            cAnimation temp = new cAnimation(texture);
            //temp.SetSpriteSheet(texture);

            for (int y = 0; y < num_rows_of_frames; y++)
            {
                for (int x = 0; x < num_columns_of_frames; x++)
                {
                    temp.AddFrame(new IntRect(x * a_frame_width,
                                               y * a_frame_height,
                                               a_frame_width,
                                               a_frame_height));
                }
            }

            temp.SetFrameTime(frame_time);
            temp.SetStartFrame(0);

            //KeyValuePair<cSpriteState, cAnimation> state_pair = new KeyValuePair<cSpriteState, cAnimation>(state_type, temp);

            m_SpriteStates.Add(state_type, temp);
        }

        public void AddAnimState(cSpriteState state_type,
					  Texture texture,
					  int a_frame_width,
                      int a_frame_height,
                      int start_frame_column, //x
                      int start_frame_row, //y
                      uint anim_start_frame,
                      int num_frames,
                      uint frame_time,
                      IntRect view_rect,
                      bool flip_vertically = false,
                      bool flip_horizontally = false)
        {
            if(view_rect.Width == 0 && view_rect.Height == 0)
            {
                view_rect.Width = a_frame_width;
                view_rect.Height = a_frame_height;
            }

            cAnimation anim = new cAnimation(texture, view_rect);
            anim.FlipHorizontally = flip_horizontally;
            anim.FlipVertically = flip_vertically;
            //anim.SetSpriteSheet(texture);

            //float a_frame_width = texture.getSize().x / max_frames;
            //float a_frame_height = texture.getSize().y;

            for (int x = 0; x < num_frames; ++x)
            {
                anim.AddFrame(new IntRect((start_frame_column + x) * a_frame_width,
                                               start_frame_row * a_frame_height,
                                               a_frame_width,
                                               a_frame_height));
            }

            anim.SetFrameTime(frame_time);
            anim.SetStartFrame(anim_start_frame);

            //std::pair<SpriteState, cAnimation> state_pair = std::pair<SpriteState, cAnimation>(state_type, anim);

            m_SpriteStates.Add(state_type, anim);
        }

        public void ChangeState(cSpriteState state_type)
        {
            m_CurrentState = state_type;

            m_pCurrentAnim = m_SpriteStates[state_type];
        }

        public cSpriteState getCurrentState()
        {
            return m_CurrentState;
        }
        public void Update(cSpriteState new_sprite_state)
        {
            //m_PrevState = m_CurrentState;

            m_CurrentState = new_sprite_state;
            m_pCurrentAnim = m_SpriteStates[m_CurrentState];

            //if(m_PrevState != m_CurrentState)
            //{
            //	m_pCurrentAnim = &m_SpriteStates[m_CurrentState];
            //}


            m_pCurrentAnim.Update();
        }

        public void Clear()
        {
            
            foreach(cAnimation a in m_SpriteStates.Values)
            {
                a.Clear();
            }

            m_SpriteStates.Clear();
        }
        public void Render(RenderTarget destination, Vector2f pos)
        {
            m_pCurrentAnim.Render(destination, pos);
        }

	    cAnimation GetCurrentAnimation()
        {
            return m_pCurrentAnim;
        }
    }
}
