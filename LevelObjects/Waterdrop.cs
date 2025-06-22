using System;
using Engine;
using Microsoft.Xna.Framework;
namespace TickTickBoom
{
    class Waterdrop : SpriteGameObject
    {
        #region Member Variables
        protected float bounce;
        Level level;
        const string WATER_SPRITE_LOCATION = "Sprites/LevelObjects/spr_water";
        #endregion
        #region Constructor
        public Waterdrop(Level level) : base(WATER_SPRITE_LOCATION, TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS)
        {
            this.level = level;
            SetOriginToCenter();
        }
        #endregion
        #region Public Methods
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            double timeInterval = gameTime.TotalGameTime.TotalSeconds * 3.0f + localPosition.X;
            // Give the 
            bounce = (float)Math.Sin(timeInterval) * 0.2f;
            localPosition.Y += bounce;
            if (IsCollected(level.Player))
            {
                IsVisible = false;
            }
        }
        #endregion

        #region Private Method
        bool IsCollected(Player player)
        {
            return HasPixelPreciseCollision(player);
        }
        #endregion
    }
}
