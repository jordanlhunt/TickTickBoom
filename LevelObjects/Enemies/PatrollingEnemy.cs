using Engine;
using Microsoft.Xna.Framework;
namespace TickTickBoom;

class PatrollingEnemy : AnimatedGameObject
{
    #region Member Variables
    protected Level level;
    protected Vector2 startingPosition;
    protected float waitTime;

    const float MAX_WAIT_TIME = .5f;
    const float WALK_SPEED = 120f;
    const string PATROLLING_ENEMY_DEFAULT_SPRITE_LOCATION = "Sprites/LevelObjects/Flame/spr_flame@9";
    const string PATROLLING_ENEMY_DEFAULT_ANIMATION_ID = "default";
    #endregion
    #region Constructor
    public PatrollingEnemy(Level level, Vector2 startingPosition) : base(TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS)
    {
        this.level = level;
        this.startingPosition = startingPosition;
        LoadAnimation(PATROLLING_ENEMY_DEFAULT_SPRITE_LOCATION, PATROLLING_ENEMY_DEFAULT_ANIMATION_ID, true, .1f);
        Reset();
    }
    #endregion

    #region Public Methods
    public override void Reset()
    {
        // Reverse direction of movement
        waitTime = 0;
        velocity.X = WALK_SPEED;
        PlayAnimation(PATROLLING_ENEMY_DEFAULT_ANIMATION_ID);
        // Go to start position
        localPosition = startingPosition;
        Origin = new Vector2(sprite.Width / 2, sprite.Height);
        base.Reset();
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // if waiting at the edge of platform turn around
        if (waitTime > 0)
        {
            waitTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (waitTime <= 0)
            {
                TurnAround();
            }
            // If unable to move forward begin to wait
            else if (!IsAbleToMoveForward())
            {
                waitTime = MAX_WAIT_TIME;
                velocity.X = 0;
            }
            // A collision with a player kills player
            if (HasPixelPreciseCollision(level.Player))
            {
                level.Player.Die();
            }
        }
    }

    protected bool IsAbleToMoveForward()
    {
        bool canMoveForward = false;
        // Determine the position to check
        Rectangle boundingBox = BoundingBox;
        Vector2 side;
        // Set the Y value to the ground itself
        side.Y = boundingBox.Bottom + 1;
        // Set the X value to whatever side the enemy is facing
        if (sprite.IsMirrored)
        {
            side.X = boundingBox.Left;
        }
        else
        {
            side.X = boundingBox.Right;
        }
        // Convert the position into Point coordinates
        Point tileAhead = level.GetTileAtCoordinates(side);
        if (level.GetTileType(tileAhead.X, tileAhead.Y) != Tile.TileType.Empty && level.GetTileType(tileAhead.X, tileAhead.Y - 1) != Tile.TileType.Wall)
        {
            canMoveForward = true;
        }
        return canMoveForward;
    }

    protected void TurnAround()
    {
        sprite.IsMirrored = !sprite.IsMirrored;
        velocity.X = WALK_SPEED;
        if (sprite.IsMirrored)
        {
            velocity.X *= -1;
        }
    }


    #endregion
}