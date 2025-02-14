using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

namespace TickTickBoom.GameStates
{
    class TitleGameState : GameState
    {
        #region Member Variables
        Button playButton;
        Button helpButton;
        string TITLE_SCREEN_LOCATION = "Sprites/Backgrounds/spr_title";
        string PLAY_BUTTON_LOCATION = "Sprites/UI/spr_button_play";
        string HELP_BUTTON_LOCATION = "Sprites/UI/spr_button_help";

        #endregion

        #region Constructor
        public TitleGameState()
        {
            // Load the title screen
            SpriteGameObject titleScreen = new SpriteGameObject(TITLE_SCREEN_LOCATION, TickTickBoom.DEPTH_LAYER_BACKGROUND);
            gameObjectList.AddChild(titleScreen);
            // Add the play button
            playButton = new Button(PLAY_BUTTON_LOCATION, TickTickBoom.DEPTH_LAYER_UI_FOREGROUND);
            playButton.LocalPosition = new Vector2(600, 540);
            // Add a help button
            helpButton = new Button(HELP_BUTTON_LOCATION, TickTickBoom.DEPTH_LAYER_UI_FOREGROUND);
            helpButton.LocalPosition = new Vector2(600, 600);
            // Add buttons to game list
            gameObjectList.AddChild(playButton);
            gameObjectList.AddChild(helpButton);
        }
        #endregion

        #region Public Methods
        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (playButton.Pressed)
            {
                ExtendedGame.GameStateManager.SwitchGameState(ExtendedGameWithLevels.STATENAME_LEVELSELECT);
            }
            else if (helpButton.Pressed)
            {
                ExtendedGame.GameStateManager.SwitchGameState(ExtendedGameWithLevels.STATENAME_HELP);
            }
        }
        #endregion
    }
}
