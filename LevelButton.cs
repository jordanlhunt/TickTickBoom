using Engine;
using Microsoft.Xna.Framework;
namespace TickTickBoom
{
    class LevelButton : Engine.UI.LevelButton
    {
        public LevelButton(int levelIndex, LevelStatus startStatus) : base(levelIndex, startStatus)
        {
            // add a label that shows the level index
            buttonLabel = new TextGameObject("Fonts/MainFont", 1, Color.White, TextGameObject.Alignment.Right);
            buttonLabel.LocalPosition = new Vector2(sprite.Width - 15, 10);
            buttonLabel.Parent = this;
            buttonLabel.Text = levelIndex.ToString();
        }
    }
}
