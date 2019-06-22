using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

using platformerGame.Utilities;
using platformerGame.Particles;
using platformerGame.App;
using platformerGame.Rendering;
using platformerGame.Effects;

namespace platformerGame.GameObjects
{
    class cMonster : cCharacter
    {
        bool killed;

        cLight eye;

        bool attacking;
        cRegulator attackCharger;

        Sprite bloom;

        public cMonster(GameScene scene, Vector2f pos) : base(scene, pos)
        {

            p_followLight = new cLight();
            p_followLight.Pos = this.Bounds.center;
            p_followLight.Radius = 100.0f;
            p_followLight.LinearizeFactor = 0.4f;
            p_followLight.Bleed = 2.0f;
            p_followLight.OriginalColor = new Color(239, 129, 37);
            p_followLight.Color = new Color(239, 129, 37); //  new Color(20, 184, 87);
            
            this.Scene.LightMap.AddStaticLight(p_followLight);

            
            eye = new cLight();
            eye.Radius = 10.0f;
            eye.LinearizeFactor = 0.98f;
            eye.Bleed = 5.0f;
            eye.OriginalColor = new Color(255, 39, 13);
            eye.Color = new Color(255, 39, 13);
            this.Scene.LightMap.AddStaticLight(eye);

            this.sleep();

            bloom = new Sprite(Scene.Assets.GetTexture("bloom"));
            bloom.Scale = new Vector2f(0.2f, 0.2f);
            bloom.Origin = new Vector2f(bloom.Texture.Size.X / 2.0f, bloom.Texture.Size.Y / 2.0f);
            bloom.Color = Color.Red;

            this.attacking = false;
            this.attackCharger = new cRegulator();
            this.attackCharger.resetByFrequency(2);
            this.boundingRadius = 12.0f;
        }

        protected override void initSprites()
        {
            base.initSprites();

            MyIntRect viewRect = Constants.CHAR_VIEW_RECT;

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_LEFT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            0,
                                            0,
                                            0,
                                            1,
                                            FRAME_TIME,
                                            viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.STAND, HorizontalFacing.FACING_RIGHT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            0,
                                            1,
                                            0,
                                            1,
                                            FRAME_TIME,
                                            viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_LEFT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            0,
                                            0,
                                            0,
                                            3,
                                            FRAME_TIME,
                                            viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.WALK, HorizontalFacing.FACING_RIGHT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            0,
                                            1,
                                            0,
                                            3,
                                            FRAME_TIME,
                                            viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_LEFT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            1,
                                            0,
                                            0,
                                            1,
                                            FRAME_TIME,
                                            viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.JUMP, HorizontalFacing.FACING_RIGHT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            1,
                                            1,
                                            0,
                                            1,
                                            FRAME_TIME,
                                            viewRect);


            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_LEFT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            6,
                                            0,
                                            0,
                                            1,
                                            FRAME_TIME,
                                            viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.FALL, HorizontalFacing.FACING_RIGHT),
                                            Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                            Constants.CHAR_FRAME_WIDTH,
                                            Constants.CHAR_FRAME_HEIGHT,
                                            6,
                                            1,
                                            0,
                                            1,
                                            FRAME_TIME,
                                            viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.LIE, HorizontalFacing.FACING_LEFT),
                                Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
                                Constants.CHAR_FRAME_WIDTH,
                                Constants.CHAR_FRAME_HEIGHT,
                                9,
                                0,
                                0,
                                1,
                                FRAME_TIME,
                                viewRect);

            spriteControl.AddAnimState(new cSpriteState(MotionType.LIE, HorizontalFacing.FACING_RIGHT),
                                Scene.Assets.GetTexture(Constants.MONSTER_TEXTURE_NAME),
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

        public override void Kill(cGameObject by)
        {
            this.Scene.LightMap.remove(this.p_followLight);
            this.Scene.LightMap.remove(this.eye);
            // this.spriteControl.ChangeState(new cSpriteState(MotionType.LIE, this.spriteControl.getCurrentState().HorizontalFacing));

            this.health = 0;
            this.killed = true;

            // int y = AppRandom.Choose<int>(new[] { 1,-1});

            this.Scene.QueueAction(() =>
            {
                Vector2f emitDirection = AppMath.Vec2NormalizeReturn(by.Velocity); //  new Vector2f(0.0f, y); 

            float len = (float)AppMath.Vec2Length(this.Velocity);
            float speed = len > 1.0f ? len : 0.0f;


            ShakeScreen.Init(this.pscene.Camera.ActualPosition);
            ShakeScreen.StartShake();

            
                    Scene.Assets.PlaySound("blood_hit2", 25, this.position);
                    //pscene.ParticleManager.Fireworks.NormalExplosion(new Particles.cEmissionInfo(this.Bounds.center, emitDirection));
                    var e = pscene.ParticleManager["explosions"] as cExplosionController;
                    e.NormalBlood(new EmissionInfo(this.Bounds.center, emitDirection, speed));

                    e.Line(new EmissionInfo(this.Bounds.center, emitDirection));

                    this.Scene.QueueAction(() =>
                    {
                        /*
                        if (this.IsOnGround)
                        {
                            this.Scene.Effects.PlaceGround(this.Bounds.center.X, this.Bounds.rightBottom.Y, "side-explosion1");
                        }
                        else
                        */
                        {
                            this.Scene.Effects.Place(this.Bounds.center, "simple-explosion3"); // flash-black
                        }

                        

                        // this.Scene.Effects.Place(intersection, "fire1");
                    });

                    // e.FastBlood(new EmissionInfo(this.Bounds.center,new Vector2f(0.0f, 0.0f)));

                    //float gy = this.Bounds.rightBottom.Y; // ground y

                    //pscene.Effects.PlaceGround(this.Bounds.center.X, gy, "side-explosion1");

                    //pscene.EntityPool.getEntitiesInRadius()
                    // pscene.Effects.Place(this.bounds.center, "simple-explosion2");

                }
                
            );

            this.Scene.QueueAction(() =>
            {

                Scene.Assets.PlaySound("coin_drop1", 20);

                ProbabilityRoll<int> numPickables = new ProbabilityRoll<int>();
                numPickables.add(70, 2);
                numPickables.add(30, 3);

                int num = numPickables.roll();
                for (int i = 0; i < num; ++i)
                {
                    pscene.EntityPool.AddPickup(
                                        new cPickupAble(
                                                this.Scene,
                                                this.Bounds.center,
                                                AppMath.GetRandomUnitVec(), // emitDirection,
                                                PickupInfo.PickupType.COIN_GOLD)
                    );
                }
            });
                /*
                new platformerGame.GameCommands.comPlacePickup(
                    this.Scene,
                    new GameObjects.cPickupAble(
                        this.Scene,
                        this.Scene.EntityPool.SpatialGrid,
                        this.Bounds.center,
                        emitDirection) )
                        */

        }

        public override void Update(float step_time)
        {
            this.Marked = false;

            Vector2f playerCenter = this.Scene.Player.Bounds.center;
            double sqrDistFromPlayer = AppMath.Vec2DistanceSqrt(playerCenter, this.Bounds.center);

            Vector2i posA = new Vector2i((int)this.Bounds.center.X, (int)this.Bounds.center.Y);
            Vector2i posB = new Vector2i((int)this.Scene.Player.Bounds.center.X, (int)this.Scene.Player.Bounds.center.Y);
            bool playerHiddenForMe = true;
            Vector2f intersectionPoint = new Vector2f(0.0f, 0.0f);

            

            if (sqrDistFromPlayer <= 80000.0) // 100 unit distance  1000000.0
            {
                AppMath.Raytrace(posA.X, posA.Y, posB.X, posB.Y, 
                    (x, y) =>
                    {
                        playerHiddenForMe = this.pscene.World.IsObastacleAtPos(new Vector2f(x, y));

                        return playerHiddenForMe;
                    }
                    
                );

                //this.wake();



                if (playerHiddenForMe == false)
                {
                    this.wake();

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
                }
                    
                    if (attacking)
                    {
                        this.StopMoving();
                    }


                    /*
                    if (this.Scene.Player.Bounds.topLeft.Y < this.Bounds.topLeft.Y)
                        this.StartJumping();
                    else
                        this.StopJumping();
                        */
                
                
            }
            else
            {
                this.StopMoving();
                this.sleep();
            }

            this.spriteControl.Update(this.GetSpriteState());


            base.updateMovement(step_time);

            //base.Update(step_time);
        }

        public override void Render(RenderTarget destination)
        {
            Vector2f cw = GetCenterViewPos();
            bloom.Position = cw;

            
            p_followLight.Pos = cw;
            eye.Pos = new Vector2f(cw.X + 2, cw.Y - 5);
            

            base.Render(destination);

            /*
            if(this.Marked)
            {
                DrawingBase.DrawRectangleShape(destination, this.Bounds, new Color(255, 20, 20, 120), BlendMode.Alpha);
            }
            */

            // destination.Draw(bloom, new RenderStates(BlendMode.Add));
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

            this.Scene.QueueAction(
                () =>
                {
                    Scene.Assets.PlaySound("body_hit1", 8);
                    var e = pscene.ParticleManager["explosions"] as cExplosionController;
                    // e.LittleBlood(new Particles.EmissionInfo(this.Bounds.center));
                    var dir = AppMath.Vec2NormalizeReturn(entity_by.Velocity);

                    float maxLen = (float)AppMath.GetRandomDoubleInRange(10.0, this.Bounds.diagonalLength() );
                    Vector2f end = AppMath.ScaleVector(entity_by.Position, AppMath.Vec2NormalizeReturn(dir), maxLen);

                    // approximately:
                    Vector2f point = AppMath.GetRandomPointOnLine(entity_by.Position, end);
                    e.LittleLine(new Particles.EmissionInfo(point, dir));
                });
        }

        public void attemptMeleeAttack(cPlayer player)
        {
            if (cCollision.OverlapAABB(this.hitCollisionRect, player.HitCollisionRect))
            {
                if (!attacking)
                {
                    this.attacking = true;
                }
                else
                {
                    if (attackCharger.isReady())
                    {
                        player.MeleeHit(1, this);
                    }
                }
            }
            else
                this.attacking = false;
        }

        public bool IsKilled
        {
            get { return killed; }
        }

        protected void wake()
        {
            this.p_followLight.TurnOn();
            this.eye.TurnOn();
        }

        protected void sleep()
        {
            this.p_followLight.TurnOff();
            this.eye.TurnOff();
        }
    }
}
