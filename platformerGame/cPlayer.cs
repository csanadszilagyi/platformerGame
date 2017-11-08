using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cPlayer : cCharacter
    {
        cWeapon weapon;
        public cPlayer(cGameScene scene, Vector2f pos) : base(scene, pos)
        {
            p_followLight = new cLight();
            p_followLight.Radius = 150.0f;
            p_followLight.LinearizeFactor = 0.9f;
            p_followLight.Color = new Color(240, 219, 164);
            this.Scene.LightMap.AddStaticLight(p_followLight);

            this.weapon = new cWeapon(this, Constants.DEFAULT_WEAPON_FIRING_FREQUENCY); // 9
        }

        protected override void initSprites()
        {
            base.initSprites();

            IntRect viewRect = Constants.CHAR_VIEW_RECT;

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         0,
                                         0,
                                         3,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         1,
                                         0,
                                         3,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         1,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         1,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);


            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         6,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         6,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);
        }
        protected override void init()
        {
            base.init();

        }

        public override void Update(float step_time)
        {
            base.Update(step_time);
        }

        public override void Render(RenderTarget destination)
        {
            
            //viewPosition = cAppMath.Interpolate(position, lastPosition, alpha);

            p_followLight.Pos = GetCenterViewPos();

            spriteControl.Render(destination, viewPosition);
            base.Render(destination);
            /*
            shape.Position = viewPosition; // - (viewSize / 2.0f);
            destination.Draw(shape);
            */
        }

        public cWeapon CurrentWeapon
        {
            get { return weapon; }
        }

        ~cPlayer()
        {
            spriteControl.Clear();
        }
    }
}
