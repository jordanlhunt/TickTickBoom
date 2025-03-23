using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TickTickBoom
{
    partial class Level : GameObjectList
    {
        #region Constants
        public const int TileWidth = 72;
        public const int TileHeight = 55;
        public string BACKGROUND_SKY = "Sprites/Backgrounds/spr_sky";
        #endregion

        #region Member Variables
        Tile[,] tiles;
        List<Waterdrop> waterDrops;
        SpriteGameObject goal;
        Player player;
        #endregion

        #region Properties
        public int LevelIndex
        {
            get;
            private set;
        }
        public Tile.TileType GetTileType(int x, int y)
        {
            return tiles[x, y].TypeOfTile;
        }
        public Point GetTileAtCoordinates(Vector2 position)
        {
            return new Point((int)Math.Floor(position.X / TileWidth), (int)Math.Floor(position.Y / TileHeight));
        }
        #endregion

        #region Constructor
        public Level(int levelIndex, string fileName)
        {
            LevelIndex = levelIndex;
            // Load the background
            GameObjectList backgrounds = new GameObjectList();
            SpriteGameObject backgroundSky = new SpriteGameObject(BACKGROUND_SKY, TickTickBoom.DEPTH_LAYER_BACKGROUND);
            backgroundSky.LocalPosition = new Vector2(0, 825 - backgroundSky.Height);
            backgrounds.AddChild(backgroundSky);
            AddChild(backgroundSky);
            LoadLevelFromFile(fileName);
        }
        #endregion

        #region Public Methods 
        public Vector2 GetCellPosition(int x, int y)
        {
            return new Vector2(x * TileWidth, y * TileHeight);
        }
        #endregion
    }
}
