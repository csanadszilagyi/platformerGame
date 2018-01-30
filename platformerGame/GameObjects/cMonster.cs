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
        bool killed;

        cLight eye;

        cTimer locateTime;

        public cMonster(cGameScene scene, Vector2f pos) : base(scene, pos)
        {
            p_followLight = new cLight();
            p_followLight.Radius = 80.0f;
            p_followLight.LinearizeFactor = 0.9f;
            p_followLight.Bleed = 2.0f;
            p_followLight.Color = new Color(20, 184, 87);
            //this.Scene.LightMap.AddStaticLight(p_followLight);


            eye = new cLight();
            eye.Radius = 10.0f;
            eye.LinearizeFactor = 0.98f;
            eye.Bleed = 5.0f;
            eye.OriginalColor = new Color(255, 39, 13);
            eye.Color = new Color(255, 39, 13);

            this.Scene.LightMap.AddStaticLight(eye);

            locateTime = new cTimer();

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
            this.killed = false;
            this.health = Constants.MONSTER_MAX_HEALTH;
            walkSpeed = Constants.MONSTER_WALK_SPEED;
            maxWalkSpeed = Constants.MONSTER_MAX_WALK_SPEED;
        }

        public override bool isActive()
        {
            return !killed;
        }

        public void Kill(cGameObject by)
        {
            this.Scene.LightMap.remove(this.p_followLight);
            this.Scene.LightMap.remove(this.p_followLight);
            this.Scene.LightMap.remove(this.eye);
            // this.spriteControl.ChangeState(new cSpriteState(MotionType.LIE, this.spriteControl.getCurrentState().HorizontalFacing));

            this.health = 0;
            this.killed = true;

            Vector2f emitDirection = cAppMath.Vec2NormalizeReturn(by.Velocity);
            this.Scene.QueueCommand(new GameCommands.comNormalBloodExplosion(this.Scene, new Particles.cEmissionInfo(this.Bounds.center, emitDirection)));

            /*if (emitDirection.Y > 0.0f)
                emitDirection.Y = -emitDirection.Y;
                */

            cAABB playerAABB = this.getBoundingBox(position);

            this.Scene.QueueCommand(new platformerGame.GameCommands.comPlacePickup(
                    this.Scene,
                    new GameObjects.cPickupAble(
                        this.Scene,
                        this.Scene.EntityPool.SpatialGrid,
                        new Vector2f(playerAABB.center.X, playerAABB.center.Y),
                        emitDirection)

                        )
                );

        }

        public override void Update(float step_time)
        {

                Vector2f playerCenter = this.Scene.Player.Bounds.center;
                double sqrDistFromPlayer = cAppMath.Vec2DistanceSqrt(playerCenter, this.Bounds.center);

                Vector2i posA = new Vector2i((int)this.Bounds.center.X, (int)this.Bounds.center.Y);
                Vector2i posB = new Vector2i((int)this.Scene.Player.Bounds.center.X, (int)this.Scene.Player.Bounds.center.Y);
                bool playerHiddenForMe = true;
                Vector2f intersectionPoint = new Vector2f(0.0f, 0.0f);

                cAppMath.Raytrace(posA.X, posA.Y, posB.X, posB.Y, new VisitMethod(
                   (int x, int y) =>
                   {
                       playerHiddenForMe = this.m_pScene.World.IsObastacleAtPos(new Vector2f(x, y));

                       return playerHiddenForMe;
                   }
                 )
               );

                if (!playerHiddenForMe && sqrDistFromPlayer <= 1000000.0) // 100 unit distance
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

                this.spriteControl.Update(this.GetSpriteState());


            base.updateMovement(step_time);

            //base.Update(step_time);
        }

        public override void Render(RenderTarget destination)
        {
            Vector2f cw = GetCenterViewPos();
            p_followLight.Pos = cw;
            eye.Pos = new Vector2f(cw.X + 2, cw.Y - 5);
            base.Render(destination);

        }

        public override void Hit(int amount, cGameObject entity_by)
        {
            base.Hit(amount, entity_by);

            if (this.health <= 0)
            {
                this.Kill(entity_by);
                //this.Scene.ParticleManager.AddNormalBloodExplosion(this.Bounds.center);
                

                return;
            }

            //this.spriteControl.ChangeState(new cSpriteState(MotionType.FALL, this.spriteControl.getCurrentState().HorizontalFacing));
            //this.Scene.ParticleManager.AddLittleBloodExplosion(this.Bounds.center, 3);

            this.Scene.QueueCommand(new GameCommands.comLittleBloodExplosion(this.Scene, this.Bounds.center));
        }

        public bool IsKilled
        {
            get { return killed; }
        }

        protected void wake()
        {
            this.eye.TurnOn();
        }

        protected void sleep()
        {
            this.eye.TurnOff();
        }
    }
}
