using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cMonster : cCharacter
    {
        bool disabled;
        bool killed;
        public cMonster(cGameScene scene, Vector2f pos) : base(scene, pos)
        {
            p_followLight = new cLight();
            p_followLight.Radius = 80.0f;
            p_followLight.LinearizeFactor = 0.9f;
            p_followLight.Color = new Color(240, 219, 164);
            this.Scene.LightMap.AddStaticLight(p_followLight);
        }

        protected override void initSprites()
        {
            base.initSprites();

            IntRect viewRect = Constants.CHAR_VIEW_RECT;

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         0,
                                         0,
                                         3,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         0,
                                         1,
                                         0,
                                         3,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         1,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         1,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);


            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_LEFT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         6,
                                         0,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_RIGHT),
                                         cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                         Constants.CHAR_FRAME_WIDTH,
                                         Constants.CHAR_FRAME_HEIGHT,
                                         6,
                                         1,
                                         0,
                                         1,
                                         FRAME_TIME,
                                         viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.LIE, HorizontalFacing.FACING_LEFT),
                             cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                             Constants.CHAR_FRAME_WIDTH,
                             Constants.CHAR_FRAME_HEIGHT,
                             9,
                             0,
                             0,
                             1,
                             FRAME_TIME,
                             viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.LIE, HorizontalFacing.FACING_RIGHT),
                             cAssetManager.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                             Constants.CHAR_FRAME_WIDTH,
                             Constants.CHAR_FRAME_HEIGHT,
                             9,
                             1,
                             0,
                             1,
                             FRAME_TIME,
                             viewRect);

        }
        protected override void init()
        {
            base.init();
            this.disabled = false;
            this.killed = false;
            this.health = 20;
        }

        public override bool isActive()
        {
            return !killed;
        }

        public void Kill()
        {
            this.Scene.LightMap.remove(this.p_followLight);
            this.killed = true;
        }

        public void Disable()
        {
            this.Scene.LightMap.remove(this.p_followLight);
            this.Scene.ParticleManager.AddNormalBloodExplosion(this.Bounds.center);
            this.spriteControl.ChangeState(new cSpriteState(MotionType.LIE, this.spriteControl.getCurrentState().HorizontalFacing));
            this.disabled = true;
            this.health = 0;
        }

        public override void Update(float step_time)
        {

            if (!disabled)
            {
                if (this.health <= 0)
                {
                    this.Disable();
                }
                else
                {
                    Vector2f playerCenter = this.Scene.Player.Bounds.center;

                    if (playerCenter.X > this.Position.X)
                        this.StartMovingRight();

                    if (playerCenter.X < this.Position.X)
                        this.StartMovingLeft();

                    this.spriteControl.Update(this.GetSpriteState());
                }  
                
            }
            else
            {
                if (cAppMath.Vec2IsZero(this.velocity))
                {
                    // draw its crops or "grave"
                    spriteControl.Render(this.Scene.StaticTexture, viewPosition);
                    this.Kill();
                }
            }    

            base.updateMovement(step_time);

           
            //base.Update(step_time);
        }

        public override void Render(RenderTarget destination)
        {
            p_followLight.Pos = GetCenterViewPos();
            base.Render(destination);
        }

        public override void Hit(int amount)
        {
            base.Hit(amount);
            this.spriteControl.ChangeState(new cSpriteState(MotionType.FALL, this.spriteControl.getCurrentState().HorizontalFacing));
            this.Scene.ParticleManager.AddLittleBloodExplosion(this.Bounds.center, 3);
        }

        public bool Disabled
        {
            get { return this.disabled; }
        }
    }
}
