using System;
using Engine;
using Microsoft.Xna.Framework;
namespace TickTickBoom
{
    class Waterdrop : SpriteGameObject
    {
        #region Member Variables
        protected float bounce;
        public Vector2 startPosition;
        Level level;
        const string WATER_SPRITE_LOCATION = "Sprites/LevelObjects/spr_water";
        const string WATER_COLLECTED_SOUND_EFFECT = "Sounds/snd_watercollected";
        #endregion
        #region Constructor
        public Waterdrop(Level level, Vector2 startingPosition) : base(WATER_SPRITE_LOCATION, TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS)
        {
            startPosition = startingPosition;
            this.level = level;
            SetOriginToCenter();
            Reset();
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
                ExtendedGame.AssetManager.PlaySoundEffect(WATER_COLLECTED_SOUND_EFFECT);
            }
        }

        public override void Reset()
        {
            localPosition = startPosition;
            IsVisible = true;

        }
        #endregion

        #region Private Method
        bool IsCollected(Player player)
        {
            return HasPixelPreciseCollision(player) && player.CanCollideWithObjects && IsVisible;
        }
        #endregion
    }
}
