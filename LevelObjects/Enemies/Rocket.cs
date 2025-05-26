using Engine;
using Microsoft.Xna.Framework;
namespace TickTickBoom;

class Rocket : AnimatedGameObject
{
    #region Member Variables
    Level level;
    Vector2 startPosition;
    const float SPEED = 500f;
    #endregion

    #region Constants
    const string ROCKET_SPRITE_LOCATION = "Sprites/LevelObjects/Rocket/spr_rocket@3";
    const string ROCKET_ANIMATION_ID = "rocket";
    #endregion

    #region Constructor
    public Rocket(Level level, Vector2 startPosition, bool isFacingLeft) : base(TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS)
    {
        this.level = level;
        LoadAnimation(ROCKET_SPRITE_LOCATION, ROCKET_ANIMATION_ID, true, 0.1f);
        PlayAnimation(ROCKET_ANIMATION_ID);
        SetOriginToCenter();
        sprite.IsMirrored = isFacingLeft;
        if (sprite.IsMirrored == true)
        {
            velocity.X -= SPEED;
            this.startPosition = startPosition + new Vector2(2 * SPEED, 0);
        }
        else
        {
            velocity.X = SPEED;
            this.startPosition = startPosition - new Vector2(2 * SPEED, 0);
        }
        Reset();
    }
    #endregion

    #region Public Methods

    public override void Reset()
    {
        LocalPosition = startPosition;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // if the rocket is off-screen on the left side then reset it
        if (sprite.IsMirrored && BoundingBox.Right < level.BoundingBox.Left)
        {
            Reset();
        }
        // if the rocket is off-screen on the right side then reset it
        else if (!sprite.IsMirrored && BoundingBox.Left > level.BoundingBox.Right)
        {
            Reset();
        }
        // If the player hits the player then the player dies
        if (HasPixelPreciseCollision(level.Player))
        {
            level.Player.Die();
        }
    }
    #endregion
}