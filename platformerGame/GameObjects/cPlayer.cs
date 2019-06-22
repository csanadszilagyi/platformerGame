using SFML.Graphics;
using SFML.System;

using platformerGame.Inventory.Weapons;
using platformerGame.Utilities;
using platformerGame.GameObjects.PickupInfo;
using platformerGame.App;

namespace platformerGame.GameObjects
{
    class cPlayer : cCharacter
    {
        cWeapon weapon;
        Text healthText;

        cLight fireLight;

        public cPlayer(GameScene scene, Vector2f pos) : base(scene, pos)
        {
            fireLight = new cLight();
            fireLight.Pos = this.Bounds.center;
            fireLight.Bleed = 10.0f;
            fireLight.Radius = 120.0f;
            fireLight.LinearizeFactor = 0.96f;
            fireLight.OriginalColor = new Color(136, 216, 176);
            fireLight.Color = new Color(136, 216, 176);
            fireLight.TurnOff();
            this.Scene.LightMap.AddStaticLight(fireLight);


            p_followLight = new cLight();
            p_followLight.Pos = this.Bounds.center;
            p_followLight.Radius = 160.0f;
            p_followLight.LinearizeFactor = 0.9f;
            p_followLight.OriginalColor = new Color(240, 219, 164);
            p_followLight.Color = new Color(240, 219, 164);
            // p_followLight.TurnOff();
            this.Scene.LightMap.AddStaticLight(p_followLight);

            this.weapon = // new cMachineGun(this, Constants.DEFAULT_WEAPON_FIRING_FREQUENCY);
               new cShotgun(this, Constants.DEFAULT_WEAPON_FIRING_FREQUENCY); // 9

            this.health = 50;
            healthText = new Text("", Scene.Assets.GetFont("pf_tempesta_seven"));
            healthText.Position = new Vector2f(pscene.AppController.MainWindow.DefaultView.Size.X - 500, 30);
            healthText.CharacterSize = 28; // in pixels, not points!
            healthText.FillColor = Color.White;
            healthText.Style = Text.Styles.Bold;

            this.boundingRadius = 12.0f;
        }

        protected override void initSprites()
        {
            base.initSprites();

            MyIntRect viewRect = Constants.CHAR_VIEW_RECT;

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_LEFT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_RIGHT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_LEFT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         0,
                                         0, // 0
                                         3, // 3
                                         60, //FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_RIGHT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         1,
                                         0, // 0
                                         3, // 3
                                         60, //FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_LEFT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         1,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_RIGHT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         1,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);


            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_LEFT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         6,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_RIGHT),
                                         Scene.Assets.GetTexture(Constants.PLAYER_TEXTURE_NAME),
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
            fireLight.Pos = GetCenterViewPos();

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


        public void ItemAction(Vector2f mouse_pos)
        {
            if (weapon.Fire(mouse_pos))
            {
                //this.fireLight.TurnOn();
                return;
            }
            //this.fireLight.TurnOff();
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
