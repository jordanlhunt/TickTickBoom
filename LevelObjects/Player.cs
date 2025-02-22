using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace TickTickBoom

{
    class Player : AnimatedGameObject
    {
        #region Constants
        // Walking speed in pixels per second
        const float WALKING_SPEED = 400f;
        const string IDLE_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_idle";
        const string RUN_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_run@13";
        const string JUMP_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_jump@14";
        const string CELEBRATION_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_celebrate@14";
        const string DIE_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_die@5";
        const string EXPLODE_ANIMATION_LOCATION = "Sprites/LevelObjects/Player/spr_explode@5x5";
        const float IDLE_ANIAMTION_FRAMETIME = .1f;
        const float RUN_ANIMATION_FRAMETIME = .04f;
        const float JUMP_ANIMATION_FRAMETIME = .8f;
        const float CELEBRATE_ANIMATION_FRAMETIME = .05f;
        const float DIE_ANIMATION_FRAMETIME = .1f;
        const float EXPLODE_ANIMATION_FRAMETIME = .04f;
        #endregion
        #region Member Variables
        // Flag to check if player is facing left or not
        bool isFacingLeft;


        #endregion
        #region Constructor
        public Player() : base(TickTickBoom.DEPTH_LAYER_LEVEL_PLAYER)
        {
            // Load the Player's various Animations
            LoadAnimation(IDLE_ANIMATION_LOCATION, "idle", true, IDLE_ANIAMTION_FRAMETIME);
            LoadAnimation(RUN_ANIMATION_LOCATION, "run", true, RUN_ANIMATION_FRAMETIME);
            LoadAnimation(JUMP_ANIMATION_LOCATION, "jump", true, JUMP_ANIMATION_FRAMETIME);
            LoadAnimation(CELEBRATION_ANIMATION_LOCATION, "celebrate", false, CELEBRATE_ANIMATION_FRAMETIME);
            LoadAnimation(DIE_ANIMATION_LOCATION, "die", true, DIE_ANIMATION_FRAMETIME);
            LoadAnimation(EXPLODE_ANIMATION_LOCATION, "explode", false, EXPLODE_ANIMATION_FRAMETIME);
            // Start with Idle animation
            PlayAnimation("idle");
            SetOriginToBottomCenter();
            isFacingLeft = false;
        }
        #endregion
        #region Public Methods
        public override void HandleInput(InputHelper inputHelper)
        {
            // Arrow keys: move left and wright 
            if (inputHelper.IsKeyDown(Keys.Left) || inputHelper.IsKeyDown(Keys.A))
            {
                isFacingLeft = true;
                velocity.X = -WALKING_SPEED;
                PlayAnimation("run");
            }
            else if (inputHelper.IsKeyDown(Keys.Right) || inputHelper.IsKeyDown(Keys.D))
            {
                isFacingLeft = false;
                velocity.X = WALKING_SPEED;
                PlayAnimation("run");
            }
            else
            {
                velocity.X = 0;
                PlayAnimation("idle");
            }
            SetOriginToBottomCenter();
            sprite.IsMirrored = isFacingLeft;
        }
        #endregion

        #region Private Methods
        private void SetOriginToBottomCenter()
        {
            Origin = new Vector2(sprite.Width / 2, sprite.Height);
        }
        #endregion
    }
}
