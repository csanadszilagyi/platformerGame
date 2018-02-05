using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

using platformerGame.Weapons;
using platformerGame.Utilities;
using platformerGame.GameObjects.PickupInfo;

namespace platformerGame
{
    class cPlayer : cCharacter
    {
        cWeapon weapon;
        Text healthText;
        public cPlayer(cGameScene scene, Vector2f pos) : base(scene, pos)
        {
            p_followLight = new cLight();
            p_followLight.Radius = 80.0f;
            p_followLight.LinearizeFactor = 0.9f;
            p_followLight.Color = new Color(240, 219, 164);
            this.Scene.LightMap.AddStaticLight(p_followLight);

            this.weapon = new cShotgun(this, Constants.DEFAULT_WEAPON_FIRING_FREQUENCY); // 9

            healthText = new Text("", cAssetManager.GetFont("pf_tempesta_seven"));
            healthText.Position = new Vector2f(pscene.AppController.MainWindow.DefaultView.Size.X - 500, 30);
            healthText.CharacterSize = 28; // in pixels, not points!
            healthText.Color = Color.White;
            healthText.Style = Text.Styles.Bold;
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
                                         0, // 0
                                         3, // 3
                                         60, //FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         1,
                                         0, // 0
                                         3, // 3
                                         60, //FRAME_TIME,
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
            walkSpeed = Constants.PLAYER_WALK_SPEED;
            maxWalkSpeed = Constants.PLAYER_MAX_WALK_SPEED;
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
            healthText.DisplayedString = health.ToString();
            destination.Draw(healthText);

        }

        public void pickUp(cPickupInfo pickup)
        {
            pickup.applyEffect(this);
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
