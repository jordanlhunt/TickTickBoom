using Engine;
using Engine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace TickTickBoom
{
    class PlayingGameState : GameState, IPlayingState
    {
        #region Member Variables
        Level level;
        Button quitButton;
        SpriteGameObject completedOverlay;
        SpriteGameObject gameOverlay;
        const int QUIT_BUTTON_X = 1290;
        const int QUIT_BUTTON_Y = 20;
        const int OVERLAY_X = 720;
        const int OVERLAY_Y = 412;
        const string QUIT_BUTTON_LOCATION = "Sprites/UI/spr_button_quit";
        const string WELL_DONE_LOCATION = "Sprites/UI/spr_welldone";
        const string GAME_OVER_LOCATION = "Sprites/UI/spr_gameover";
        const string WIN_SOUND_LOCATION = "Sounds/snd_won";
        #endregion
        #region Constructor
        public PlayingGameState()
        {
            // add a quit button
            quitButton = new Button(QUIT_BUTTON_LOCATION, TickTickBoom.DEPTH_LAYER_UI_FOREGROUND);
            quitButton.LocalPosition = new Vector2(QUIT_BUTTON_X, QUIT_BUTTON_Y);
            gameObjectList.AddChild(quitButton);
            // add overlay images
            completedOverlay = AddOverlay(WELL_DONE_LOCATION);
            gameOverlay = AddOverlay(GAME_OVER_LOCATION);
        }
        #endregion
        #region Public Methods
        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (level != null)
            {
                // if the "completed" overlay is visible. Pressing the spacebar should send the player to the next level
                if (completedOverlay.IsVisible)
                {
                    if (inputHelper.IsKeyPressed(Keys.Space))
                    {
                        Console.Write("[PlayingStateState.cs] - HandleInput() - Space bar has been pressed");
                        ExtendedGameWithLevels.GoToNextLevel(level.LevelIndex);
                    }
                }
                // Else update the level itself and check for button presses
                else
                {
                    level.HandleInput(inputHelper);
                    if (quitButton.Pressed)
                    {
                        ExtendedGame.GameStateManager.SwitchGameState(ExtendedGameWithLevels.STATENAME_LEVELSELECT);
                    }
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (level != null)
            {
                level.Update(gameTime);
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            if (level != null)
            {
                level.Draw(gameTime, spriteBatch);
            }
        }
        public void LoadLevel(int levelIndex)
        {
            level = new Level(levelIndex, ExtendedGame.ContentRootDirectory + "/Levels/level" + levelIndex + ".txt");
            // Hide the overlay images
            completedOverlay.IsVisible = false;
            gameOverlay.IsVisible = false;
        }
        public void LevelCompleted(int levelIndex)
        {
            // show the completed overlay
            completedOverlay.IsVisible = true;
            // Play the level is complete sound
            ExtendedGame.AssetManager.PlaySoundEffect(WIN_SOUND_LOCATION);
            // mark the level as solved and unlock the next level
            ExtendedGameWithLevels.MarkLevelAsSolved(levelIndex);
        }
        #endregion
        #region Private Methods
        SpriteGameObject AddOverlay(string assetName)
        {
            SpriteGameObject overlay = new SpriteGameObject(assetName, 1);
            overlay.SetOriginToCenter();
            overlay.LocalPosition = new Vector2(OVERLAY_X, OVERLAY_Y);
            gameObjectList.AddChild(overlay);
            return overlay;
        }
        #endregion
    }
}
