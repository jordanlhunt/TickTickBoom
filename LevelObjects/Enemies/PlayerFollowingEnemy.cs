using System;
using System.Data;
using Engine;
using Microsoft.Xna.Framework;
using TickTickBoom;


class PlayerFollowingEnemy : PatrollingEnemy
{
    #region Constructor
    public PlayerFollowingEnemy(Level level, Vector2 startPosition) : base(level, startPosition)
    {
    }
    #endregion
    #region Public Methods
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // Check if the player is moving, and if the enemy is not already waiting
        if (level.Player.IsMoving && velocity.X != 0 && level.Player.CanCollideWithObjects)
        {
            float deltaX = level.Player.GlobalPosition.X - GlobalPosition.X;
            if (Math.Sign(deltaX) != Math.Sign(velocity.X) && Math.Abs(deltaX) > 100)
            {
                TurnAround();
            }
        }
    }
    #endregion
}