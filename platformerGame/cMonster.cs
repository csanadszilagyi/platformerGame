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
        bool canThink;
        public cMonster(cGameScene scene, Vector2f pos) : base(scene, pos)
        {
            p_followLight = new cLight();
            p_followLight.Radius = 80.0f;
            p_followLight.LinearizeFactor = 0.9f;
            p_followLight.Color = new Color(240, 219, 164);
            this.Scene.LightMap.AddStaticLight(p_followLight);
            this.canThink = true;
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
            this.health = 5;
        }

        public override bool isAlive()
        {
            return this.health > 0;
        }

        public void Kill()
        {
            this.Scene.LightMap.remove(this.p_followLight);
            this.Scene.ParticleManager.AddExplosion(this.Bounds.center);
            this.spriteControl.ChangeState(new cSpriteState(MotionType.LIE, this.spriteControl.getCurrentState().HorizontalFacing));
            this.canThink = false;
        }
        public override void Update(float step_time)
        {
            if (canThink)
            {
                Vector2f playerCenter = this.Scene.Player.Bounds.center;

                if (playerCenter.X > this.Position.X)
                    this.StartMovingRight();

                if (playerCenter.X < this.Position.X)
                    this.StartMovingLeft();

                this.spriteControl.Update(this.GetSpriteState());
            }

            base.updateMovement(step_time);
            //base.Update(step_time);
        }

        public override void Render(RenderTarget destination)
        {
            p_followLight.Pos = GetCenterViewPos();
            spriteControl.Render(destination, viewPosition);
            base.Render(destination);
        }

        public bool CanThink
        {
            get { return canThink; }
        }
    }
}
