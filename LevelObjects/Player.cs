using System;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace TickTickBoom
{
    class Player : AnimatedGameObject
    {
        #region Constants
        // Walking speed in pixels per second
        const string IDLE_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_idle";
        const string RUN_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_run@13";
        const string JUMP_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_jump@14";
        const string CELEBRATION_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_celebrate@14";
        const string DIE_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_die@5";
        const string EXPLODE_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_explode@5x5";
        const float WALKING_SPEED = 425.5f;
        const float IDLE_ANIMATION_FRAMETIME = .1f;
        const float RUN_ANIMATION_FRAMETIME = .04f;
        const float JUMP_ANIMATION_FRAMETIME = .08f;
        const float CELEBRATE_ANIMATION_FRAMETIME = .05f;
        const float DIE_ANIMATION_FRAMETIME = .1f;
        const float EXPLODE_ANIMATION_FRAMETIME = .04f;
        const float JUMP_SPEED = 900.0f;
        const float GRAVITY = 2300.0f;
        const float MAX_FALL_SPEED = 1200.0f;
        const float ICE_FRICTION = 1;
        const float NORMAL_FRICTION = 20;
        const float AIR_FRICTION = 5;
        #endregion
        #region Member Variables
        // Flag to check if player is facing left or not
        bool isFacingLeft;
        bool isGrounded;
        bool isStandingOnIceTile;
        bool isStandingOnHotTile;
        Level level;
        float desiredHorizontalSpeed;
        #endregion
        #region Properties
        Rectangle BoundingBoxForCollisions
        {
            get
            {
                Rectangle boundingBox = BoundingBox;
                boundingBox.X += 20;
                boundingBox.Width -= 40;
                boundingBox.Height += 1;
                return boundingBox;
            }
        }

        public bool IsFalling
        {
            get
            {
                return velocity.Y > 0 && !isGrounded;
            }
        }
        #endregion
        #region Constructor
        public Player(Level level) : base(TickTickBoom.DEPTH_LAYER_LEVEL_PLAYER)
        {
            // Load the Player's various Animations
            LoadAnimation(IDLE_ANIMATION_LOCATION, "idle", true, IDLE_ANIMATION_FRAMETIME);
            LoadAnimation(RUN_ANIMATION_LOCATION, "run", true, RUN_ANIMATION_FRAMETIME);
            LoadAnimation(JUMP_ANIMATION_LOCATION, "jump", false, JUMP_ANIMATION_FRAMETIME);
            LoadAnimation(CELEBRATION_ANIMATION_LOCATION, "celebrate", false, CELEBRATE_ANIMATION_FRAMETIME);
            LoadAnimation(DIE_ANIMATION_LOCATION, "die", true, DIE_ANIMATION_FRAMETIME);
            LoadAnimation(EXPLODE_ANIMATION_LOCATION, "explode", false, EXPLODE_ANIMATION_FRAMETIME);
            // Start with Idle animation
            PlayAnimation("idle");
            SetOriginToBottomCenter();
            isFacingLeft = false;
            isStandingOnHotTile = false;
            isStandingOnIceTile = false;
            isGrounded = true;
            this.level = level;
        }
        #endregion
        #region Public Methods
        public override void HandleInput(InputHelper inputHelper)
        {
            // Arrow keys: move left and right 
            if (inputHelper.IsKeyDown(Keys.Left) || inputHelper.IsKeyDown(Keys.A))
            {
                isFacingLeft = true;
                desiredHorizontalSpeed = -WALKING_SPEED;
                if (isGrounded)
                {
                    PlayAnimation("run");
                }
            }
            else if (inputHelper.IsKeyDown(Keys.Right) || inputHelper.IsKeyDown(Keys.D))
            {
                isFacingLeft = false;
                desiredHorizontalSpeed = WALKING_SPEED;
                if (isGrounded)
                {
                    PlayAnimation("run");
                }
            }
            else
            {
                desiredHorizontalSpeed = 0;
                if (isGrounded)
                {
                    PlayAnimation("idle");
                }
            }
            // Allow for jumping
            if (isGrounded && inputHelper.IsKeyPressed(Keys.Space))
            {
                Jump();
            }
            if (!isGrounded)
            {
                int fallSpriteIndex = 8;
                PlayAnimation("jump", false, fallSpriteIndex);
            }
            SetOriginToBottomCenter();
            sprite.IsMirrored = isFacingLeft;
        }
        public void ApplyGravity(GameTime gameTime)
        {
            velocity.Y += GRAVITY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (velocity.Y > MAX_FALL_SPEED)
            {
                velocity.Y = MAX_FALL_SPEED;
            }
        }
        public override void Update(GameTime gameTime)
        {
            float friction;
            if (isStandingOnIceTile)
            {
                friction = ICE_FRICTION;
            }
            else if (isGrounded)
            {
                friction = NORMAL_FRICTION;
            }
            else
            {
                friction = AIR_FRICTION;
            }
            float velocityModifier = MathHelper.Clamp(friction * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
            Vector2 previousPosition = localPosition;
            velocity.X += (desiredHorizontalSpeed - velocity.X) * velocityModifier;
            if (Math.Abs(velocity.X) < 1)
            {
                velocity.X = 0;
            }
            ApplyGravity(gameTime);
            base.Update(gameTime);
            HandleTileCollisions(previousPosition);

        }

        public void Die()
        {
            Console.WriteLine("[PLAYER.CS] - Player.Die() has been called");
        }

        public void Jump(float speed = JUMP_SPEED)
        {
            velocity.Y -= speed;
            PlayAnimation("jump");
        }
        #endregion
        #region Private Methods
        private void SetOriginToBottomCenter()
        {
            Origin = new Vector2(sprite.Width / 2, sprite.Height);
        }
        private void HandleTileCollisions(Vector2 previousPosition)
        {
            isGrounded = false;
            isStandingOnHotTile = false;
            isStandingOnIceTile = false;
            Rectangle boundingBox = BoundingBoxForCollisions;
            Point topLeftTile = level.GetTileAtCoordinates(new Vector2(boundingBox.Left, boundingBox.Top)) - new Point(1, 1);
            Point bottomRightTile = level.GetTileAtCoordinates(new Vector2(boundingBox.Right, boundingBox.Bottom)) + new Point(1, 1);
            for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
            {
                for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
                {
                    Tile.TileType tileType = level.GetTileType(x, y);
                    if (tileType == Tile.TileType.Empty)
                    {
                        continue;
                    }
                    Vector2 tilePosition = level.GetCellPosition(x, y);
                    if ((tileType == Tile.TileType.Platform) && (localPosition.Y > tilePosition.Y) && (previousPosition.Y > tilePosition.Y))
                    {
                        continue;
                    }
                    Rectangle tileBounds = new Rectangle((int)tilePosition.X, (int)tilePosition.Y, Level.TileWidth, Level.TileHeight);
                    if (!tileBounds.Intersects(boundingBox))
                    {
                        continue;
                    }
                    Rectangle collisionOverlap = CollisionDetection.CalculateIntersection(boundingBox, tileBounds);
                    // Horizontal collision
                    if (collisionOverlap.Width < collisionOverlap.Height)
                    {
                        if ((velocity.X >= 0 && boundingBox.Center.X < tileBounds.Left) || (velocity.X <= 0 && boundingBox.Center.X > tileBounds.Right))
                        {
                            localPosition.X = previousPosition.X;
                            velocity.X = 0;
                        }
                    }
                    // Vertical collision
                    else
                    {
                        // Edge Case: Very close to edge of a tile
                        if (velocity.Y >= 0 && boundingBox.Center.Y < tileBounds.Top && collisionOverlap.Width > 6)
                        {
                            isGrounded = true;
                            Tile.SurfaceType surfaceType = level.GetSurfaceType(x, y);
                            if (surfaceType == Tile.SurfaceType.Hot)
                            {
                                isStandingOnHotTile = true;
                            }
                            else if (surfaceType == Tile.SurfaceType.Ice)
                            {
                                isStandingOnIceTile = true;
                            }
                            velocity.Y = 0;
                            localPosition.Y = tileBounds.Top;
                        }
                        // Edge Case: ceiling bumping head on edge of a tile
                        else if (velocity.Y <= 0 && boundingBox.Center.Y > tileBounds.Bottom && collisionOverlap.Height > 2)
                        {
                            localPosition.Y = previousPosition.Y;
                            velocity.Y = 0;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
