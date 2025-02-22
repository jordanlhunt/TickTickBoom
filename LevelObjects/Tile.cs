using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TickTickBoom
{
    class Tile : GameObject
    {
        #region Enums
        public enum TileType
        {
            Empty, Wall, Platform
        };
        public enum SurfaceType
        {
            Normal, Hot, Ice
        };
        #endregion

        #region Member Variables
        TileType tileType;
        SurfaceType surfaceType;
        SpriteGameObject tileImage;
        #endregion

        #region Properties
        public TileType TypeOfTile
        {
            get
            {
                return tileType;
            }
        }
        public SurfaceType Surface
        {
            get
            {
                return surfaceType;
            }
        }
        #endregion

        #region Constructor
        public Tile(TileType tileType, SurfaceType surfaceType)
        {
            this.tileType = tileType;
            this.surfaceType = surfaceType;

            // Add an image depending on the type
            string surfaceExtenion = "";
            if (surfaceType == SurfaceType.Hot)
            {
                surfaceExtenion = "_hot";
            }
            else if (surfaceType == SurfaceType.Ice)
            {
                surfaceExtenion = "_ice";
            }
            if (tileType == TileType.Wall)
            {
                tileImage = new SpriteGameObject("Sprites/Tiles/spr_wall" + surfaceExtenion, TickTickBoom.DEPTH_LAYER_LEVEL_TILES);
            }
            else if (tileType == TileType.Platform)
            {
                tileImage = new SpriteGameObject("Sprites/Tiles/spr_platform" + surfaceExtenion, TickTickBoom.DEPTH_LAYER_LEVEL_TILES);
            }
            // if there is an image then make it a child of this object
            if (tileImage != null)
            {
                tileImage.Parent = this;
            }
        }
        #endregion
        #region Public Methods
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (tileImage != null)
            {
                tileImage.Draw(gameTime, spriteBatch);
            }
        }
        #endregion
    }
}
