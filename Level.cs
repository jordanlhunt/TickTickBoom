using System;
using System.Collections.Generic;
using Engine;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1.Effects;

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
        public Player Player
        {
            get;
            private set;
        }
        public Tile.TileType GetTileType(int x, int y)
        {
            Tile.TileType tileType = Tile.TileType.Platform;
            if (x < 0 || x >= tiles.GetLength(0))
            {
                tileType = Tile.TileType.Wall;
            }
            else if (y < 0 || y >= tiles.GetLength(1))
            {
                tileType = Tile.TileType.Empty;
            }
            else
            {

                tileType = tiles[x, y].TypeOfTile;
            }
            return tileType;
        }
        public Tile.SurfaceType GetSurfaceType(int x, int y)
        {
            Tile.SurfaceType surfaceType = Tile.SurfaceType.Normal;

            // Check if coordinates are valid
            if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
            {
                // Update surfaceType only if the tile's surface is not Normal
                if (tiles[x, y].Surface != Tile.SurfaceType.Normal)
                {
                    surfaceType = tiles[x, y].Surface;
                }
            }

            return surfaceType;
        }
        public Point GetTileAtCoordinates(Vector2 position)
        {
            return new Point((int)Math.Floor(position.X / TileWidth), (int)Math.Floor(position.Y / TileHeight));
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(0, 0, tiles.GetLength(0) * TileWidth, tiles.GetLength(1) * TileHeight);
            }
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

        #region Private Methods
        private Vector2 GetCellBottomCenter(int x, int y)
        {
            return GetCellPosition(x, y + 1) + new Vector2(TileWidth / 2, 0);
        }
        #endregion
    }
}
