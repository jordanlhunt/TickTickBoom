using Engine;
using Microsoft.Xna.Framework;
using System;
using TickTickBoom.GameStates;

namespace TickTickBoom
{
    public class TickTickBoom : ExtendedGameWithLevels
    {

        #region Constants
        public const float DEPTH_LAYER_BACKGROUND = 0;
        public const float DEPTH_LAYER_UI_BACKGROUND = .9f;
        public const float DEPTH_LAYER_UI_FOREGROUND = 1.0f;
        public const float DEPTH_LAYER_LEVEL_TILES = 0.5f;
        public const float DEPTH_LAYER_LEVEL_OBJECTS = .6f;
        public const float DEPTH_LAYER_LEVEL_PLAYER = .7f;
        #endregion

        [STAThread]
        static void Main()
        {
            TickTickBoom game = new TickTickBoom();
            game.Run();
        }
        public TickTickBoom()
        {
            IsMouseVisible = true;
            Window.Title = "TickTickBoom";
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            gameWorldSize = new Point(1440, 825);
            windowSize = new Point(1024, 586);
            IsFullScreen = false;
            // Load the player's progress from a file
            LoadProgress();
            // Add the game states
            GameStateManager.AddGameState(STATENAME_TITLE, new TitleGameState());
            GameStateManager.AddGameState(STATENAME_LEVELSELECT, new LevelSelectionGameState());
            GameStateManager.AddGameState(STATENAME_HELP, new HelpGameState());
            GameStateManager.AddGameState(STATENAME_PLAYING, new PlayingGameState());
            // Start at the title screen
            GameStateManager.SwitchGameState(STATENAME_TITLE);
            // Play Background Music
            AssetManager.PlaySong("Sounds/snd_music", true);
        }
    }
}
