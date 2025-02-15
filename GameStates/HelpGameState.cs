using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;

namespace TickTickBoom
{
    class HelpGameState : GameState
    {
        #region Member Variables
        Button backButton;
        string BACKGROUND_SPRITE_LOCATION = "Sprites/Backgrounds/spr_help";
        string BACKBUTTON_LOCATION = "Sprites/UI/spr_button_back";
        #endregion

        public HelpGameState()
        {
            // add a background
            gameObjectList.AddChild(new SpriteGameObject(BACKGROUND_SPRITE_LOCATION, 1));
            // Add a back button
            backButton = new Button(BACKBUTTON_LOCATION, TickTickBoom.DEPTH_LAYER_UI_FOREGROUND);
            backButton.LocalPosition = new Vector2(1180, 750);
            gameObjectList.AddChild(backButton);
        }
        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (backButton.Pressed)
            {
                ExtendedGame.GameStateManager.SwitchGameState(ExtendedGameWithLevels.STATENAME_TITLE);
            }
        }

    }
}
