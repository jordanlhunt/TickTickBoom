﻿using System;
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

        const string JUMP_SOUND_EFFECT_LOCATION = "Sounds/snd_player_jump";
        const string EXPLODE_SOUND_EFFECT_LOCATION = "Sounds/snd_player_explode";

        const string DIE_SOUND_EFFECT_LOCATION = "Sounds/snd_player_die";

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
        bool isExploding;
        private bool isStandingOnIceTile;
        private bool isStandingOnHotTile;
        private bool isCelebrating;
        Level level;
        float desiredHorizontalSpeed;
        Vector2 startingPosition;
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
        public bool IsMoving
        {
            get
            {
                return velocity != Vector2.Zero;
            }
        }
        public bool IsAlive
        {
            get;
            private set;
        }
        public bool CanCollideWithObjects
        {
            get
            {
                return IsAlive && !isCelebrating;
            }
        }
        #endregion
        #region Constructor
        public Player(Level level, Vector2 startingPosition) : base(TickTickBoom.DEPTH_LAYER_LEVEL_PLAYER)
        {
            // Load the Player's various Animations
            LoadAnimation(IDLE_ANIMATION_LOCATION, "idle", true, IDLE_ANIMATION_FRAMETIME);
            LoadAnimation(RUN_ANIMATION_LOCATION, "run", true, RUN_ANIMATION_FRAMETIME);
            LoadAnimation(JUMP_ANIMATION_LOCATION, "jump", false, JUMP_ANIMATION_FRAMETIME);
            LoadAnimation(CELEBRATION_ANIMATION_LOCATION, "celebrate", false, CELEBRATE_ANIMATION_FRAMETIME);
            LoadAnimation(DIE_ANIMATION_LOCATION, "die", true, DIE_ANIMATION_FRAMETIME);
            LoadAnimation(EXPLODE_ANIMATION_LOCATION, "explode", false, EXPLODE_ANIMATION_FRAMETIME);
            this.startingPosition = startingPosition;
            this.level = level;
            Reset();
        }
        #endregion
        #region Public Methods
        public override void HandleInput(InputHelper inputHelper)
        {
            if (!CanCollideWithObjects)
            {
                return;
            }
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
            Vector2 previousPosition = localPosition;
            if (isCelebrating == false)
            {
                if (CanCollideWithObjects)
                {
                    ApplyFriction(gameTime);
                    float friction = ApplyFriction(gameTime);
                    float velocityModifier = MathHelper.Clamp(friction * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
                    velocity.X += (desiredHorizontalSpeed - velocity.X) * velocityModifier;
                    if (Math.Abs(velocity.X) < 1)
                    {
                        velocity.X = 0;
                    }
                }
            }
            else
            {
                velocity.X = 0;
            }
            ApplyGravity(gameTime);
            base.Update(gameTime);
            // Only do the Tile Collision if the player is alive
            if (IsAlive)
            {
                HandleTileCollisions(previousPosition);
                // check if we've fallen down through the level
                if (BoundingBox.Center.Y > level.BoundingBox.Bottom)
                {
                    Die();
                }
                if (isStandingOnHotTile)
                {
                    level.Timer.TimeMultiplier = 2;
                }
                else
                {
                    level.Timer.TimeMultiplier = 1;
                }
            }
        }
        public void Celebrate()
        {
            isCelebrating = true;
            PlayAnimation("celebrate");
            SetOriginToBottomCenter();
            // Stop Moving
            velocity = Vector2.Zero;
        }
        public void Die()
        {
            IsAlive = false;
            PlayAnimation("die");
            velocity = new Vector2(0, -JUMP_SPEED);
            ExtendedGame.AssetManager.PlaySoundEffect(DIE_SOUND_EFFECT_LOCATION);
        }
        public void Explode()
        {
            IsAlive = false;
            isExploding = true;
            PlayAnimation("explode");
            velocity = Vector2.Zero;
            ExtendedGame.AssetManager.PlaySoundEffect(EXPLODE_SOUND_EFFECT_LOCATION);

        }

        public void Jump(float speed = JUMP_SPEED)
        {
            velocity.Y -= speed;
            PlayAnimation("jump");
            ExtendedGame.AssetManager.PlaySoundEffect(JUMP_SOUND_EFFECT_LOCATION);
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
        public float ApplyFriction(GameTime gameTime)
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
            return friction;
        }
        public override void Reset()
        {
            localPosition = startingPosition;
            velocity = Vector2.Zero;
            desiredHorizontalSpeed = 0;
            // Start with idle sprite
            PlayAnimation("idle", true);
            SetOriginToBottomCenter();
            isCelebrating = false;
            isFacingLeft = false;
            isStandingOnHotTile = false;
            isStandingOnIceTile = false;
            IsAlive = true;
            isGrounded = true;
            isExploding = false;
        }
        #endregion
    }
}
