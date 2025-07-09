using System;
using System.Collections.Generic;
using System.Threading;
using Engine;
using Microsoft.Xna.Framework;

namespace TickTickBoom
{
    partial class Level : GameObjectList
    {
        #region Constants
        public const int TileWidth = 72;
        public const int TileHeight = 55;
        public const string BACKGROUND_SKY = "Sprites/Backgrounds/spr_sky";

        public const string BACKGROUND_MOUNTAINS = "Sprites/Backgrounds/spr_mountain_";
        #endregion

        #region Member Variables
        Tile[,] tiles;
        List<Waterdrop> waterDrops;
        List<SpriteGameObject> backgrounds;
        SpriteGameObject goal;
        Player player;

        bool isLevelComplete;

        BombTimer bombTimer;
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

        public BombTimer Timer
        {
            get
            {
                return bombTimer;
            }
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

        public Boolean IsAllDropsCollected
        {
            get
            {
                bool isWaterdropsVisable = true;
                foreach (Waterdrop waterdrop in waterDrops)
                {
                    if (waterdrop.IsVisible)
                    {
                        isWaterdropsVisable = false;
                    }
                }
                return isWaterdropsVisable;
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
            isLevelComplete = false;
            bombTimer = new BombTimer();
            AddMountains();
            AddClouds();
            AddChild(bombTimer);
        }

        #endregion

        #region Public Methods 
        public Vector2 GetCellPosition(int x, int y)
        {
            return new Vector2(x * TileWidth, y * TileHeight);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Check if the level is completed
            if (isLevelComplete == false && IsAllDropsCollected == true && Player.HasPixelPreciseCollision(goal))
            {
                isLevelComplete = true;
                bombTimer.IsRunning = false;
                ExtendedGameWithLevels.GetPlayingState().LevelCompleted(LevelIndex);
                Player.Celebrate();
            }
            else if (Player.IsAlive && bombTimer.HasTimeLeftExpired)
            {
                Player.Explode();
            }
        }

        public override void Reset()
        {
            base.Reset();
            isLevelComplete = false;
        }
        #endregion

        #region Private Methods
        private Vector2 GetCellBottomCenter(int x, int y)
        {
            return GetCellPosition(x, y + 1) + new Vector2(TileWidth / 2, 0);
        }

        private void AddMountains()
        {
            for (int i = 0; i < 4; i++)
            {
                SpriteGameObject backgroundMountain = new SpriteGameObject(BACKGROUND_MOUNTAINS + (ExtendedGame.Random.Next(2) + 1), TickTickBoom.DEPTH_LAYER_BACKGROUND + .01f * (float)ExtendedGame.Random.NextDouble());
                backgroundMountain.LocalPosition = new Vector2(backgroundMountain.Width * (i - 1) * 0.4f, BoundingBox.Height - backgroundMountain.Height);
                backgrounds.Add(backgroundMountain);
            }
        }

        private void AddClouds()
        {
            for (int i = 0; i < 6; i++)
            {
                backgrounds.Add(new Cloud(this));
            }
        }

        #endregion
    }
}
