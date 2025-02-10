using Engine;
using Microsoft.Xna.Framework;
using System;

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

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            base.LoadContent();

            gameWorldSize = new Point(1440, 825);
            windowSize = new Point(1024, 586);
            IsFullScreen = false;
            LoadProgress();
            // GameStateManager.AddGameState(STATENAME_TITLE, new Title)
        }
    }
}
