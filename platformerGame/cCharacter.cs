using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cCharacter : cGameObject
    {
        public const uint FRAME_TIME = 60; //(uint)(1.0f / 0.0166f);

        protected RectangleShape shape;
        protected bool isOnGround;
        protected bool isJumpActive;
        protected bool isOnOnewWayPlatform;

        protected HorizontalFacing horizontalFacing;
        protected cSpriteStateController spriteControl;

        protected int health;
        protected cLight p_followLight = null;

        /*
        public cCharacter() : base()
        {

            spriteControl = new cSpriteStateController();
            init();
            InitSprites();
        }
    */
        public cCharacter(cGameScene scene, Vector2f pos) : base(scene, pos)
        {
            spriteControl = new cSpriteStateController();
            initSprites();
            init();

        }

        protected virtual void initSprites()
        {
            spriteControl.Clear();
        }
        protected virtual void init()
        {
            mapCollisionRect = new cAABB(0,0,1,1);
            mapCollisionRect.SetDims(new Vector2f(Constants.CHAR_COLLISON_RECT.Width, Constants.CHAR_COLLISON_RECT.Height));
            mapCollisionRect.SetPosByTopLeft(position);

            shape = new RectangleShape();
            shape.FillColor = Color.Green;
            shape.Size = new Vector2f(mapCollisionRect.dims.X, mapCollisionRect.dims.Y);

            isJumpActive = false;
            isOnGround = false;
            isOnOnewWayPlatform = false;

            horizontalFacing = HorizontalFacing.FACING_RIGHT;

            //must call, else not working
            spriteControl.ChangeState(this.GetSpriteState());

            this.health = 1;

        }

        public override cAABB Bounds
        {
            get
            {
                return getBoundingBox(this.position);
            }

            set
            {
                this.Bounds = value;
            }
        }
   
        
        public cAABB getBoundingBox(Vector2f pos_by)
        {
            return new cAABB(pos_by, mapCollisionRect.dims);
        }
        
        public bool IsOnOneWayPlatform
        {
            get { return isOnOnewWayPlatform; }
            set { isOnOnewWayPlatform = true; }
        }

        //hogy kívülről is leheseen offsettelni
        public void Move(float offset_x, float offset_y)
        {
            //lastPosition = position;
            position.X += offset_x;
            position.Y += offset_y;
        }

        public Vector2f GetCenterViewPos()
        {
            return new Vector2f(viewPosition.X + mapCollisionRect.halfDims.X, viewPosition.Y + mapCollisionRect.halfDims.Y);
        }
        protected virtual void updateX(float step_time, cWorld world)
        {
            acceleration.X = force.X;
            velocity.X += acceleration.X * step_time;

            if (acceleration.X < 0.0f)
            {
                velocity.X = cAppMath.Max<float>(velocity.X, -Constants.MAX_WALK_SPEED);
            }
            else if (acceleration.X > 0.0f)
            {
                velocity.X = cAppMath.Min<float>(velocity.X, Constants.MAX_WALK_SPEED);
            }
            else
            // if (isOnGround)
            {
                velocity.X = isOnGround ? velocity.X * Constants.GROUND_SLOW_DOWN_FACTOR
                    : velocity.X * Constants.AIR_SLOW_DOWN_FACTOR;
            }

            velocity.X = Math.Abs(velocity.X) <= 0.05f ? 0.0f : velocity.X;

            float delta = velocity.X * step_time;

            if (delta <= 0.0f)
            {
                float wallRightX;

                if (hasLeftWall2(world, delta, out wallRightX))
                {
                    position.X = wallRightX;
                    velocity.X = 0.0f;
                }
                else
                {
                    position.X += delta;
                }
            }
            else
            {
                float wallLeftX;
                if (hasRightWall2(world, delta, out wallLeftX))
                {
                    position.X = wallLeftX - mapCollisionRect.dims.X;
                    velocity.X = 0.0f;
                }
                else
                {
                    position.X += delta;
                }
            }
        }

        protected virtual void updateY(float step_time, cWorld world)
        {
            float gravity = (isJumpActive && velocity.Y < 0.0f) ? Constants.JUMP_GRAVITY : Constants.GRAVITY;

            force.Y += gravity;

            acceleration.Y = force.Y;

            velocity.Y += acceleration.Y * step_time;

            //velocity.Y = Math.Min(velocity.Y + gravity * step_time, Constants.MAX_Y_SPEED);
            velocity.Y = Math.Min(velocity.Y, Constants.MAX_Y_SPEED);

            float groundY;

            float delta = velocity.Y * step_time;

            if (delta >= 0.0f)
            {
                if (hasGround(world, delta, out groundY))
                {
                    position.Y = groundY - mapCollisionRect.dims.Y;
                    isOnGround = true;
                    velocity.Y = 0.0f;

                    // bouncing
                    //velocity.Y = Math.Abs(velocity.Y) <= 65.0f ? 0.0f : -(velocity.Y * 0.8f);
                }
                else
                {
                    isOnGround = false;
                    position.Y += delta;
                }
            }
            else
            {
                float bottomY;
                if (hasCeiling(world, delta, out bottomY))
                {
                    position.Y = bottomY + 1.0f; //- mapCollisionRect.dims.Y;

                    velocity.Y = 0.0f;
                }
                else
                {
                    isOnGround = false;
                    position.Y += delta;
                }
            }
            //float delta = (velocity.Y * step_time);

        }

        protected virtual void updateMovement(float step_time)
        {
            cWorld world = m_pScene.World;

            lastPosition.X = position.X;
            lastPosition.Y = position.Y;

           // mapCollisionRect.SetPosByTopLeft(position);


            updateX(step_time, world);
            updateY(step_time, world);

            this.force.X = 0.0f;
            this.force.Y = 0.0f;

        }

        public void StartMovingRight()
        {
            //acceleration.X = Constants.WALK_SPEED;
            this.AddForce(new Vector2f( Constants.WALK_SPEED, 0.0f));
            horizontalFacing = HorizontalFacing.FACING_RIGHT;
        }
        public void StartMovingLeft()
        {
            //acceleration.X = -Constants.WALK_SPEED;
            this.AddForce(new Vector2f( -Constants.WALK_SPEED, 0.0f));
            horizontalFacing = HorizontalFacing.FACING_LEFT;
            //m_HorizontalFacing = HorizontalFacing::FACING_LEFT;
        }
        public void StartJumping()
        {
            isJumpActive = true;

            if (isOnGround)
            {
                //m_pSpriteControl->ChangeState(Sprite_State(MotionType::JUMP, m_HorizontalFacing));
                velocity.Y = -Constants.JUMP_SPEED; // * cGame::StepTime;

                isOnGround = false;
            }
        }
        public void StopJumping()
        {
            isJumpActive = false;
        }
        public void StopMoving()
        {
            acceleration.X = 0.0f;
            //velocity.X = 0.0f;
        }

        public void StopMovingX()
        {
            // acceleration.X = 0.0f;
            velocity.X = 0.0f;
        }

        public cSpriteState GetSpriteState()
        {
            MotionType motion = MotionType.STAND;

            motion = isOnGround ? (Acceleration.X == 0.0f) ? MotionType.STAND : MotionType.WALK : (velocity.Y < 0.0f) ? MotionType.JUMP : MotionType.FALL;
            return new cSpriteState(motion, horizontalFacing);

        }

        public override void Update(float step_time)
        {

            updateMovement(step_time);
            spriteControl.Update(this.GetSpriteState());
        }

        public override void Render(RenderTarget destination)
        {

            //viewPosition = cAppMath.Interpolate(position, lastPosition, alpha);

            

            spriteControl.Render(destination, viewPosition);
            /*
            shape.Position = viewPosition; // - (viewSize / 2.0f);
            destination.Draw(shape);
            */
        }

        protected bool hasCeiling(cWorld world, float delta, out float bottomY)
        {

            bottomY = 0.0f;

            float predictedPosY = position.Y + delta;

            Vector2f oldTopLeft = new Vector2f(position.X, position.Y);
            Vector2f newTopLeft = new Vector2f(position.X, predictedPosY);
            Vector2f newTopRight = new Vector2f(newTopLeft.X + mapCollisionRect.dims.X - 2.0f, newTopLeft.Y);

            int endY = world.ToMapPos(oldTopLeft).Y; //mMap.GetMapTileYAtPoint(newBottomLeft.y);
            int begY = Math.Min(world.ToMapPos(newTopLeft).Y, endY);


            int dist = Math.Max(Math.Abs(endY - begY), 1);

            int tileIndexX;

            for (int tileIndexY = begY; tileIndexY <= endY; ++tileIndexY)
            {
                var topLeft = cAppMath.Interpolate(newTopLeft, oldTopLeft, (float)Math.Abs(endY - tileIndexY) / dist);
                var topRight = new Vector2f(topLeft.X + mapCollisionRect.dims.X - 1, topLeft.Y);

                for (var checkedTile = topLeft; ; checkedTile.X += Constants.TILE_SIZE)
                {
                    checkedTile.X = Math.Min(checkedTile.X, topRight.X);

                    tileIndexX = world.ToMapPos(checkedTile).X;

                    if (world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).Type == TileType.WALL)
                    {
                        bottomY = (((float)tileIndexY) * Constants.TILE_SIZE + world.WorldBounds.topLeft.Y + Constants.TILE_SIZE);
                        return true;
                    }

                    if (checkedTile.X >= topRight.X)
                        break;
                }
            }

            return false;
        }

        protected bool hasGround(cWorld world, float delta, out float groundY)
        {
            /*var oldCenter = lastPosition;
            var center = position;*/

            groundY = 0.0f;

            float predictedPosY = position.Y + delta;

            Vector2f up = new Vector2f(0, -1);
            Vector2f bottom = new Vector2f(0, 1);
            Vector2f left = new Vector2f(-1, 0);
            Vector2f right = new Vector2f(1, 0);

            Vector2f oldBottomLeft = new Vector2f(position.X, position.Y + mapCollisionRect.dims.Y);
            Vector2f newBottomLeft = new Vector2f(position.X, predictedPosY + mapCollisionRect.dims.Y);
            Vector2f newBottomRight = new Vector2f(newBottomLeft.X + mapCollisionRect.dims.X - 2.0f, newBottomLeft.Y);

            int endY = world.ToMapPos(newBottomLeft).Y; //mMap.GetMapTileYAtPoint(newBottomLeft.y);
            int begY = Math.Max(world.ToMapPos(oldBottomLeft).Y - 1, endY);

            int dist = Math.Max(Math.Abs(endY - begY), 1);

            int tileIndexX;

            for (int tileIndexY = begY; tileIndexY <= endY; ++tileIndexY)
            {
                var bottomLeft = cAppMath.Interpolate(newBottomLeft, oldBottomLeft, (float)Math.Abs(endY - tileIndexY) / dist);
                var bottomRight = new Vector2f(bottomLeft.X + mapCollisionRect.dims.X - 2.0f, bottomLeft.Y);

                for (var checkedTile = bottomLeft; ; checkedTile.X += Constants.TILE_SIZE)
                {
                    checkedTile.X = Math.Min(checkedTile.X, bottomRight.X);

                    tileIndexX = world.ToMapPos(checkedTile).X;

                    //world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).PlayerCollidable = true;

                    groundY = (int)((float)tileIndexY * Constants.TILE_SIZE + world.WorldBounds.topLeft.Y);

                    TileType tile = world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).Type;
                    if (tile == TileType.WALL)
                    {
                        isOnOnewWayPlatform = false;
                        return true;
                    }
                    else if (tile == TileType.ONEWAY_PLATFORM && position.Y <= groundY - MapCollisionRect.dims.Y)
                    {
                        isOnOnewWayPlatform = true;
                        return true;
                    }

                    if (checkedTile.X >= bottomRight.X)
                    {
                        /*if(isOnOnewWayPlatform)
                            return true;*/
                        break;
                    }
                }
            }

            return false;
        }


        protected bool hasGround2(cWorld world, float delta, out float groundY)
        {
            groundY = 0.0f;

            float predictedPosY = position.Y + delta;

            Vector2f oldBottomRight = new Vector2f(position.X + mapCollisionRect.halfDims.X, position.Y + mapCollisionRect.dims.Y);

            Vector2f oldBottomLeft = new Vector2f(position.X, position.Y + mapCollisionRect.dims.Y);
            Vector2f newBottomLeft = new Vector2f(oldBottomLeft.X, predictedPosY + mapCollisionRect.dims.Y);
            Vector2f newBottomRight = new Vector2f(newBottomLeft.X + mapCollisionRect.halfDims.X, newBottomLeft.Y);

            int tileEndX = world.ToMapPos(oldBottomLeft).X+1;
            int tileBeginX = Math.Min(world.ToMapPos(oldBottomLeft).X, tileEndX);

            int tileBeginY = world.ToMapPos(oldBottomLeft).Y;
            int tileEndY = world.ToMapPos(newBottomLeft).Y;

            for (int tileIndexY = tileBeginY; tileIndexY <= tileEndY; ++tileIndexY)
            {
                for (int tileIndexX = tileBeginX; tileIndexX <= tileEndX; ++tileIndexX)
                {

                    
                    if (world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).Type == TileType.WALL)
                    /*world.isRectOnWall()*/
                    {
                        world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).PlayerCollidable = true;

                        //world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).PlayerCollidable = true;
                        groundY = (int)(tileIndexY * Constants.TILE_SIZE + world.WorldBounds.topLeft.Y);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// balról jobbra, alulról felfelé
        /// </summary>
        /// <param name="world"></param>
        /// <param name="delta"></param>
        /// <param name="wallRightX"></param>
        /// <returns></returns>
        protected bool hasLeftWall2(cWorld world, float delta, out float wallRightX)
        {
            wallRightX = 0.0f;

            float predictedPosX = position.X + delta;

            Vector2f oldTopLeft = new Vector2f(position.X, position.Y);
            Vector2f newTopLeft = new Vector2f(predictedPosX - 1, position.Y);
            Vector2f newBottomLeft = new Vector2f(newTopLeft.X, newTopLeft.Y + mapCollisionRect.dims.Y - 2.0f);

            int tileEndX = world.ToMapPos(newTopLeft).X - 1;
            int tileBeginX = Math.Max(world.ToMapPos(oldTopLeft).X, tileEndX);

            int tileBeginY = world.ToMapPos(newBottomLeft).Y;
            int tileEndY = world.ToMapPos(newTopLeft).Y;

            for (int tileIndexX = tileBeginX; tileIndexX > tileEndX; --tileIndexX)
            {
                for (int tileIndexY = tileBeginY; tileIndexY >= tileEndY; --tileIndexY)
                {
                    //world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).PlayerCollidable = false;
                    if (world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).Type == TileType.WALL)
                    /*world.isRectOnWall()*/
                    {
                        //world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).PlayerCollidable = true;
                        wallRightX = (int)(tileIndexX * Constants.TILE_SIZE + Constants.TILE_SIZE + world.WorldBounds.topLeft.X);
                        return true;
                    }
                }
            }

            return false;
        }
        

        protected bool hasRightWall2(cWorld world, float delta, out float wallLeftX)
        {
            wallLeftX = 0.0f;

            float predictedPosX = position.X + mapCollisionRect.dims.X + delta;

            Vector2f oldTopRight = new Vector2f(position.X + mapCollisionRect.dims.X, position.Y);
            Vector2f newTopRight = new Vector2f(predictedPosX, position.Y);
            Vector2f newBottomRight = new Vector2f(newTopRight.X+2, newTopRight.Y + mapCollisionRect.dims.Y - 2.0f);

            int tileEndX = world.ToMapPos(newTopRight).X + 1;
            int tileBeginX = Math.Min(world.ToMapPos(oldTopRight).X, tileEndX);

            int tileBeginY = world.ToMapPos(newBottomRight).Y;
            int tileEndY = world.ToMapPos(newTopRight).Y;

            for (int tileIndexX = tileBeginX; tileIndexX < tileEndX; ++tileIndexX)
            {
                for (int tileIndexY = tileBeginY; tileIndexY >= tileEndY; --tileIndexY)
                {
                    //world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).PlayerCollidable = false;
                    if (world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).Type == TileType.WALL)
                    /*world.isRectOnWall()*/
                    {
                        //world.GetCurrentLevel().GetTileAtXY(tileIndexX, tileIndexY).PlayerCollidable = true;
                        wallLeftX = (int)(tileIndexX * Constants.TILE_SIZE + world.WorldBounds.topLeft.X);
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void Hit(int amount)
        {
            this.health -= amount;
        }

        ~cCharacter()
        {
            spriteControl.Clear();
        }

    }
}
