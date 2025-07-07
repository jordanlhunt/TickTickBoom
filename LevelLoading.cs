using System.Collections.Generic;
using System.IO;
using Engine;
using Microsoft.Xna.Framework;

namespace TickTickBoom
{
    partial class Level : GameObjectList
    {
        #region Public Methods
        void LoadLevelFromFile(string fileName)
        {
            // Open the file
            StreamReader streamReader = new StreamReader(fileName);
            string levelDescription = streamReader.ReadLine();
            // Read the rows of the grip; keep track of the longest row
            int gridWidth = 0;
            List<string> gridRows = new List<string>();
            string line = streamReader.ReadLine();
            while (line != null)
            {
                if (line.Length > gridWidth)
                {
                    gridWidth = line.Length;
                }
                gridRows.Add(line);
                line = streamReader.ReadLine();
            }
            // Close the file
            streamReader.Close();
            // Create all the game objects from the grid
            AddPlayingField(gridRows, gridWidth, gridRows.Count);
            // Add game objects to show that general level information
            AddLevelInfoObjects(levelDescription);
        }

        private void AddLevelInfoObjects(string levelDescription)
        {
            const string FRAME_HINT_LOCATION = "Sprites/UI/spr_frame_hint";
            const string HINT_TEXT_FONT = "Fonts/HintFont";
            const int hintTextX = 510;
            const int hintTextY = 40;
            const int frameX = 720;
            const int frameY = 50;
            // Background box
            SpriteGameObject frame = new SpriteGameObject(FRAME_HINT_LOCATION, TickTickBoom.DEPTH_LAYER_UI_BACKGROUND);
            frame.SetOriginToCenter();
            frame.LocalPosition = new Vector2(frameX, frameY);
            // Text
            TextGameObject hintText = new TextGameObject(HINT_TEXT_FONT, TickTickBoom.DEPTH_LAYER_UI_FOREGROUND, Color.Black);
            hintText.Text = levelDescription;
            hintText.LocalPosition = new Vector2(hintTextX, hintTextY);
            // Add them as children
            AddChild(frame);
            AddChild(hintText);
        }
        #endregion
        #region Private Methods
        private void AddPlayingField(List<string> gridRows, int gridWidth, int gridHeight)
        {
            // Create a parent object for everything
            GameObjectList playField = new GameObjectList();
            // initialize the list of water drops
            waterDrops = new List<Waterdrop>();
            // prepare for grid array
            tiles = new Tile[gridWidth, gridHeight];
            // Load the tiles
            for (int y = 0; y < gridHeight; y++)
            {
                string row = gridRows[y];
                for (int x = 0; x < gridWidth; x++)
                {
                    // If The row is too show then set as an empty tile
                    char symbol = '.';
                    if (x < row.Length)
                    {
                        symbol = row[x];
                    }
                    // Load the tile
                    AddTile(x, y, symbol);
                }
            }
        }
        private void AddTile(int x, int y, char symbolToInsert)
        {

            // Load the static part of the tile
            Tile tile = CharToStaticTile(symbolToInsert);
            tile.LocalPosition = GetCellPosition(x, y);
            AddChild(tile);
            // Store a reference to that tile in the grid 
            tiles[x, y] = tile;
            if (symbolToInsert == '1')
            {
                LoadPlayerCharacter(x, y);
            }
            else if (symbolToInsert == 'X')
            {
                LoadGoal(x, y);
            }
            else if (symbolToInsert == 'W')
            {
                LoadWaterDrop(x, y);
            }
            else if (symbolToInsert == 'R')
            {
                LoadRocketEnemy(x, y);
            }
            else if (symbolToInsert == 'T')
            {
                LoadTurtleEnemy(x, y);
            }
            else if (symbolToInsert == 'S')
            {
                LoadSparkyEnemy(x, y);
            }
            else if (symbolToInsert == 'A' || symbolToInsert == 'B' || symbolToInsert == 'C')
            {
                LoadFlameEnemy(x, y, symbolToInsert);
            }
        }
        private Tile CharToStaticTile(char symbol)
        {
            var tileMap = new Dictionary<char, Tile>
            {
                { '-', new Tile(Tile.TileType.Platform, Tile.SurfaceType.Normal) },
                { '#', new Tile(Tile.TileType.Wall, Tile.SurfaceType.Normal) },
                { 'h', new Tile(Tile.TileType.Platform, Tile.SurfaceType.Hot) },
                { 'H', new Tile(Tile.TileType.Wall, Tile.SurfaceType.Hot) },
                { 'i', new Tile(Tile.TileType.Platform, Tile.SurfaceType.Ice) },
                { 'I', new Tile(Tile.TileType.Wall, Tile.SurfaceType.Ice) }
            };
            Tile resultTile = new Tile(Tile.TileType.Empty, Tile.SurfaceType.Normal);
            if (tileMap.TryGetValue(symbol, out Tile tileInDictionary))
            {
                resultTile = tileInDictionary;
            }
            return resultTile;
        }
        private void LoadPlayerCharacter(int x, int y)
        {
            Vector2 startingPosition = GetCellPosition(x, y) + new Vector2(TileWidth / 2, TileHeight);
            Player = new Player(this, startingPosition);
            Player.LocalPosition = GetCellPosition(x, y) + new Vector2(TileWidth / 2, TileHeight);
            AddChild(Player);
        }
        private void LoadGoal(int x, int y)
        {
            // Create the exit object
            const string GOAL_SPRITE_LOCATION = "Sprites/LevelObjects/spr_goal";
            goal = new SpriteGameObject(GOAL_SPRITE_LOCATION, TickTickBoom.DEPTH_LAYER_LEVEL_OBJECTS);
            goal.LocalPosition = GetCellPosition(x, y + 1);
            goal.Origin = new Vector2(0, goal.Height);
            AddChild(goal);
        }
        private void LoadWaterDrop(int x, int y)
        {
            // Create the water drop
            Vector2 startingPosition = GetCellPosition(x, y) + new Vector2(TileWidth / 2, TileHeight / 3);
            Waterdrop waterDrop = new Waterdrop(this, startingPosition);
            AddChild(waterDrop);
            waterDrops.Add(waterDrop);
        }

        private void LoadRocketEnemy(int x, int y)
        {
            Rocket rocket = new Rocket(this, GetCellPosition(x, y), x != 0);
            AddChild(rocket);
        }

        private void LoadTurtleEnemy(int x, int y)
        {
            Turtle turtle = new Turtle(this);
            turtle.LocalPosition = GetCellBottomCenter(x, y);
            AddChild(turtle);
        }

        private void LoadSparkyEnemy(int x, int y)
        {
            Sparky sparky = new Sparky(this, GetCellBottomCenter(x, y));
            AddChild(sparky);
        }

        private void LoadFlameEnemy(int x, int y, char loadSymbol)
        {
            Vector2 position = GetCellBottomCenter(x, y);
            PatrollingEnemy enemy;
            if (loadSymbol == 'A')
            {
                enemy = new PatrollingEnemy(this, position);
            }
            else if (loadSymbol == 'B')
            {
                enemy = new PlayerFollowingEnemy(this, position);
            }
            else
            {
                enemy = new UnpredictableEnemy(this, position);
            }
            AddChild(enemy);
        }

        #endregion
    }
}
