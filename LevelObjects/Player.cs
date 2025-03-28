﻿using Engine;
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
        const float IDLE_ANIAMTION_FRAMETIME = .1f;
        const float RUN_ANIMATION_FRAMETIME = .04f;
        const float JUMP_ANIMATION_FRAMETIME = .8f;
        const float CELEBRATE_ANIMATION_FRAMETIME = .05f;
        const float DIE_ANIMATION_FRAMETIME = .1f;
        const float EXPLODE_ANIMATION_FRAMETIME = .04f;
        const float JUMP_SPEED = 900.0f;
        const float GRAVITY = 2300.0f;
        const float MAX_FALL_SPEED = 1200.0f;
        #endregion
        #region Member Variables
        // Flag to check if player is facing left or not
        bool isFacingLeft;
        bool isGrounded;
        Level level;
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
        #endregion
        #region Constructor
        public Player(Level level) : base(TickTickBoom.DEPTH_LAYER_LEVEL_PLAYER)
        {
            // Load the Player's various Animations
            LoadAnimation(IDLE_ANIMATION_LOCATION, "idle", true, IDLE_ANIAMTION_FRAMETIME);
            LoadAnimation(RUN_ANIMATION_LOCATION, "run", true, RUN_ANIMATION_FRAMETIME);
            LoadAnimation(JUMP_ANIMATION_LOCATION, "jump", false, JUMP_ANIMATION_FRAMETIME);
            LoadAnimation(CELEBRATION_ANIMATION_LOCATION, "celebrate", false, CELEBRATE_ANIMATION_FRAMETIME);
            LoadAnimation(DIE_ANIMATION_LOCATION, "die", true, DIE_ANIMATION_FRAMETIME);
            LoadAnimation(EXPLODE_ANIMATION_LOCATION, "explode", false, EXPLODE_ANIMATION_FRAMETIME);
            // Start with Idle animation
            PlayAnimation("idle");
            SetOriginToBottomCenter();
            isFacingLeft = false;
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
                velocity.X = -WALKING_SPEED;
                if (isGrounded)
                {
                    PlayAnimation("run");
                }
            }
            else if (inputHelper.IsKeyDown(Keys.Right) || inputHelper.IsKeyDown(Keys.D))
            {
                isFacingLeft = false;
                velocity.X = WALKING_SPEED;
                if (isGrounded)
                {
                    PlayAnimation("run");
                }
            }
            else
            {
                velocity.X = 0;
                if (isGrounded)
                {
                    PlayAnimation("idle");
                }
            }
            // Allow for jumping
            if (isGrounded && inputHelper.IsKeyPressed(Keys.Space))
            {
                velocity.Y = -JUMP_SPEED;
                PlayAnimation("jump");
            }
            // Show the falling animation
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
            Vector2 previousPosition = localPosition;
            ApplyGravity(gameTime);
            base.Update(gameTime);
            HandleTileCollisions(previousPosition);
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
            Rectangle boundingBox = BoundingBoxForCollisions;
            Point topLeftTile = level.GetTileAtCoordinates(new Vector2(boundingBox.Left, boundingBox.Top)) - new Point(1, 1);
            Point bottomRightTile = level.GetTileAtCoordinates(new Vector2(boundingBox.Right, boundingBox.Bottom)) + new Point(1, 1);
            // Check for collisions
            for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
            {
                for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
                {
                    Tile.TileType tileType = level.GetTileType(x, y);
                    Vector2 tilePosition = level.GetCellPosition(x, y);
                    Rectangle tileBounds = new Rectangle((int)tilePosition.X, (int)tilePosition.Y, Level.TileWidth, Level.TileHeight);
                    if (tileType == Tile.TileType.Empty)
                    {
                        continue;
                    }
                    if ((tileType == Tile.TileType.Platform) && (localPosition.Y > tilePosition.Y) && (previousPosition.Y > tilePosition.Y))
                    {
                        continue;
                    }
                    if (!tileBounds.Intersects(boundingBox))
                    {
                        continue;
                    }

                }
            }



        }
        #endregion
    }
}
