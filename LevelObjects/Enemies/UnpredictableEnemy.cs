using System;
using Engine;
using Microsoft.Xna.Framework;
namespace TickTickBoom;

class UnpredictableEnemy : PatrollingEnemy
{
    #region  Member Variables
    const float MIN_SPEED = 80.0f;
    const float MAX_SPEED = 140.0f;
    #endregion

    #region Constructor 
    public UnpredictableEnemy(Level level, Vector2 startPosition) : base(level, startPosition)
    {

    }
    #endregion

    #region Public Methods

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (waitTime <= 0 && ExtendedGame.Random.NextDouble() <= 0.01)
        {
            TurnAround();
            // Set movement to a random speed
            float newRandomSpeed = MIN_SPEED + (float)ExtendedGame.Random.NextDouble() * (MAX_SPEED - MIN_SPEED);
            velocity.X = Math.Sign(velocity.X) * newRandomSpeed;
        }
    }
}