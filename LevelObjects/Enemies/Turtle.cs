using System.Numerics;
using System.Windows.Forms;
using Engine;
namespace TickTickBoom;

class Turtle : AnimatedGameObject
{
    #region Member Variables
    Level level;
    Vector2 startPosition;
    bool isSneezing;
    float timer;
    const float SPEED = 245.0f;
    const float ANIMATION_SWITCH_DURATION = 5.0f;
    const float BOUNCE_VELOCITY = 1500.0f;
    const string TURTLE_IDLE_SPRITE_LOCATION = "Sprites/LevelObjects/Turtle/spr_idle";
    const string TURTLE_SNEEZE_SPRITE_LOCATION = "Sprites/LevelObjects/Turtle/spr_sneeze@9";
    const string TURTLE_IDLE_ANIMATION_ID = "idle";
    const string TURTLE_SNEEZE_ANIMATION_ID = "sneeze";
    #endregion

    #region Constructor
    public Turtle(Level level) : base(TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS)
    {
        this.level = level;
        LoadAnimation(TURTLE_SNEEZE_SPRITE_LOCATION, TURTLE_SNEEZE_ANIMATION_ID, false, .1f);
        LoadAnimation(TURTLE_IDLE_SPRITE_LOCATION, TURTLE_IDLE_ANIMATION_ID, true, 0.1f);
        PlayAnimation(TURTLE_IDLE_ANIMATION_ID);
        isSneezing = false;
        SetOriginToCenter();
        Reset();
    }
    #endregion

    #region Properties
    bool HasSpikesOut
    {
        get
        {
            return isSneezing && sprite.SheetIndex >= 2;
        }

    }
    #endregion

    #region Public Methods
    public override void Reset()
    {
        isSneezing = false;
        PlayAnimation(TURTLE_IDLE_ANIMATION_ID);
        SetOriginToCenter();
        timer = ANIMATION_SWITCH_DURATION;
        // bottom of the turtle
        Origin = new Vector2(sprite.Width / 2, 120);
    }

    public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
    {
        base.Update(gameTime);

        if (isSneezing)
        {

            PlayAnimation(TURTLE_SNEEZE_ANIMATION_ID);
        }
        else
        {
            PlayAnimation(TURTLE_IDLE_ANIMATION_ID);
        }
        timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (timer <= 0.0f)
        {
            timer = ANIMATION_SWITCH_DURATION;
            isSneezing = !isSneezing;
        }
        CollisionCheck(level.Player);
    }
    #endregion

    #region Private Methods
    public void CollisionCheck(Player player)
    {
        // If collision with player is true
        if (HasPixelPreciseCollision(player) && HasSpikesOut)
        {
            player.Die();
        }
        if (HasPixelPreciseCollision(player) && player.IsFalling)
        {
            player.Jump(BOUNCE_VELOCITY);
        }
    }
    #endregion
}