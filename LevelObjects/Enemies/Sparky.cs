using Engine;
using Microsoft.Xna.Framework;
namespace TickTickBoom;

class Sparky : AnimatedGameObject
{
    #region Member Variables
    Level level;
    Vector2 basePosition;
    float timeUntilDrop;
    const float ANIMATION_FRAME_TIME = .01f;
    const float MIN_TIME_TO_DROP = 3.0f;
    const float MAX_TIME_TO_DROP = 5.0f;
    const float FLOAT_ABOVE_GROUND = 120.0f;
    const string SPARKY_IDLE_SPRITE_LOCATION = "Sprites/LevelObjects/Sparky/spr_idle";
    const string SPARKY_ELECTROCUTE_SPRITE_LOCATION = "Sprites/LevelObjects/Sparky/spr_electrocute@6x5";
    const string SPARKY_IDLE_ANIMATION_ID = "idle";
    const string SPARKY_ELECTROCUTE_ANIMATION_ID = "electrocute";
    const float FALL_SPEED = 300.0f;
    const float RISE_SPEED = 60.0f;
    #endregion
    #region Properties 
    bool IsDeadly
    {
        get
        {
            return sprite.SheetIndex >= 11 && sprite.SheetIndex <= 27 && timeUntilDrop <= 0;
        }
    }
    #endregion
    #region Constructor
    public Sparky(Level level, Vector2 basePosition) : base(TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS)
    {
        this.level = level;
        this.basePosition = basePosition;
        LoadAnimation(SPARKY_IDLE_SPRITE_LOCATION, SPARKY_IDLE_ANIMATION_ID, true, ANIMATION_FRAME_TIME);
        LoadAnimation(SPARKY_ELECTROCUTE_SPRITE_LOCATION, SPARKY_ELECTROCUTE_ANIMATION_ID, false, ANIMATION_FRAME_TIME);
        Reset();
    }
    #endregion
    #region Public Methods
    public override void Reset()
    {
        PlayAnimation(SPARKY_IDLE_ANIMATION_ID);
        Origin = new Vector2(sprite.Width / 2, 135);
        localPosition = basePosition;
        localPosition.Y -= FLOAT_ABOVE_GROUND;
        velocity = Vector2.Zero;
        timeUntilDrop = MIN_TIME_TO_DROP + (float)ExtendedGame.Random.NextDouble() * (MAX_TIME_TO_DROP - MIN_TIME_TO_DROP);
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (timeUntilDrop > 0)
        {
            timeUntilDrop -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeUntilDrop <= 0)
            {
                PlayAnimation(SPARKY_ELECTROCUTE_ANIMATION_ID);
                velocity.Y = FALL_SPEED;
            }
        }
        else
        {
            if (velocity.Y > 0 && localPosition.Y >= basePosition.Y)
            {
                velocity.Y = 0;
            }
            if (velocity.Y == 0 && (sprite as Animation).HasAnimationEnded)
            {
                velocity.Y = -RISE_SPEED;
            }
            if (velocity.Y < 0 && localPosition.Y <= basePosition.Y - FLOAT_ABOVE_GROUND)
            {
                Reset();
            }
            if (IsDeadly && HasPixelPreciseCollision(level.Player))
            {
                level.Player.Die();
            }
        }
    }
    #endregion
}