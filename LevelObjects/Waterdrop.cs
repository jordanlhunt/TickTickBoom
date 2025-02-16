using Engine;
using Microsoft.Xna.Framework;
using System;
namespace TickTickBoom
{
    class Waterdrop : SpriteGameObject
    {
        #region Member Variables
        protected float bounce;
        const string WATER_SPRITE_LOCATION = "Sprites/LevelObjects/spr_water";
        #endregion
        #region Constructor
        public Waterdrop() : base(WATER_SPRITE_LOCATION, TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS)
        {
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
        }
        #endregion
    }
}
