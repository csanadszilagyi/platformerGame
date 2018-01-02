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

        cLight eye;
        public cMonster(cGameScene scene, Vector2f pos) : base(scene, pos)
        {
            p_followLight = new cLight();
            p_followLight.Radius = 80.0f;
            p_followLight.LinearizeFactor = 0.9f;
            p_followLight.Bleed = 2.0f;
            p_followLight.Color = new Color(20, 184,87);
            //this.Scene.LightMap.AddStaticLight(p_followLight);


            eye = new cLight();
            eye.Radius = 10.0f;
            eye.LinearizeFactor = 0.98f;
            eye.Bleed = 5.0f;
            eye.OriginalColor = new Color(255, 39, 13);
            eye.Color = new Color(255, 39, 13);
            
            this.Scene.LightMap.AddStaticLight(eye);

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
            this.health = Constants.MONSTER_MAX_HEALTH;
            walkSpeed = Constants.MONSTER_WALK_SPEED;
            maxWalkSpeed = Constants.MONSTER_MAX_WALK_SPEED;
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
            this.Scene.LightMap.remove(this.eye);

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

                    if (cAppMath.Vec2DistanceSqrt(playerCenter, this.Bounds.center) <= 100.0 * 100.0)
                    {
                        //this.wake();

                        if (playerCenter.X > this.Position.X)
                        {
                            if (velocity.X < 0.0f) this.StopMovingX();
                            this.StartMovingRight();
                        }

                        if (playerCenter.X < this.Position.X)
                        {
                            if (velocity.X > 0.0f) this.StopMovingX();
                            this.StartMovingLeft();
                        }

                        if (this.Scene.Player.Bounds.topLeft.Y < this.Bounds.topLeft.Y)
                            this.StartJumping();
                        else
                            this.StopJumping();


                    }
                    else
                    {
                        this.StopMoving();
                        //this.sleep();
                    }

                    
                }

                this.spriteControl.Update(this.GetSpriteState());

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
            Vector2f cw = GetCenterViewPos();
            p_followLight.Pos = cw;
            eye.Pos = new Vector2f(cw.X+2, cw.Y - 5);
            base.Render(destination);

        }

        public override void Hit(int amount)
        {
            base.Hit(amount);
            this.spriteControl.ChangeState(new cSpriteState(MotionType.FALL, this.spriteControl.getCurrentState().HorizontalFacing));
            this.Scene.ParticleManager.AddLittleBloodExplosion(this.Bounds.center, 3);
        }

        protected void wake()
        {
            this.eye.TurnOn();
        }

        protected void sleep()
        {
            this.eye.TurnOff();
        }
        public bool Disabled
        {
            get { return this.disabled; }
        }
    }
}
