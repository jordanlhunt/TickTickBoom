using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

namespace TickTickBoom
{
    class LevelSelectionGameState : GameState
    {
        #region Member Values
        Button backButton;
        // An Array of extra refrerences to level buttons. Used to check if a button has been pressed
        LevelButton[] levelButtons;
        const string BACKGROUND_SPRITE_LOCATION = "Sprites/Backgrounds/spr_levelselect";
        const string BACKBUTTON_LOCATION = "Sprites/UI/spr_button_back";
        int backButtonX = 720;
        int backButtonY = 690;
        int gridOffsetX = 395;
        int gridOffsetY = 175;

        #endregion
        #region Constructor 
        public LevelSelectionGameState()
        {
            // Add background
            SpriteGameObject background = new SpriteGameObject(BACKGROUND_SPRITE_LOCATION, TickTickBoom.DEPTH_LAYER_BACKGROUND);
            gameObjectList.AddChild(background);
            // Add the back button
            backButton = new Button(BACKBUTTON_LOCATION, TickTickBoom.DEPTH_LAYER_UI_FOREGROUND);
            backButton.LocalPosition = new Vector2(backButtonX, backButtonY);
            backButton.SetOriginToCenter();
            gameObjectList.AddChild(backButton);
            // Add a level button for each level
            levelButtons = new LevelButton[ExtendedGameWithLevels.NumberOfLevels];
            Vector2 gridOffset = new Vector2(gridOffsetX, gridOffsetY);
            const int BUTTONS_PER_ROW = 4;
            const int ROW_COLUMN_BUFFER = 20;
            for (int i = 0; i < ExtendedGameWithLevels.NumberOfLevels; i++)
            {
                // create the button
                LevelButton levelButton = new LevelButton(i + 1, ExtendedGameWithLevels.GetLevelStatus(i + 1));
                // Give it the correct position
                int row = i / BUTTONS_PER_ROW;
                int column = i % BUTTONS_PER_ROW;
                levelButton.LocalPosition = gridOffset + new Vector2(column * (levelButton.Width + ROW_COLUMN_BUFFER), row * (levelButton.Height + ROW_COLUMN_BUFFER));
                // Add the button as child objects
                gameObjectList.AddChild(levelButton);
                levelButtons[i] = levelButton;
            }
        }
        #endregion
        #region Public Methods
        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            // If the back button is pressed. Go back to the title screen
            if (backButton.Pressed)
            {
                ExtendedGame.GameStateManager.SwitchGameState(ExtendedGameWithLevels.STATENAME_TITLE);
            }
            // If level is unlocked go to the level. Otherwise stay at the screen.

            foreach (LevelButton levelButton in levelButtons)
            {

                if (levelButton.Pressed && levelButton.Status != LevelStatus.Locked)
                {
                    // Go to the playing state
                    ExtendedGame.GameStateManager.SwitchGameState(ExtendedGameWithLevels.STATENAME_PLAYING);
                    // Go the correct level for that playing state
                    ExtendedGameWithLevels.GetPlayingState().LoadLevel(levelButton.LevelIndex);
                    return;
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (LevelButton button in levelButtons)
            {
                if (button.Status != ExtendedGameWithLevels.GetLevelStatus(button.LevelIndex))
                {
                    button.Status = ExtendedGameWithLevels.GetLevelStatus(button.LevelIndex);
                }
            }
        }
        #endregion
    }
}
