using System;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace TickTickBoom
{
    class BombTimer : GameObjectList
    {
        #region Constants
        const string LABEL_BACKGROUND_LOCATION = "Sprites/UI/spr_timer";
        const string FONT_LOCATION = "Fonts/MainFont";
        #endregion
        #region Member Variables
        double timeLeft;
        TextGameObject label;
        #endregion
        #region Properties
        public bool IsRunning
        {
            get;
            set;
        }
        public bool HasTimeLeftExpired
        {
            get
            {
                return timeLeft <= 0;
            }
        }
        public float TimeMultiplier
        {
            get;
            set;
        }
        #endregion
        #region Constructor
        public BombTimer()
        {
            localPosition = new Vector2(20, 20);
            SpriteGameObject background = new SpriteGameObject(LABEL_BACKGROUND_LOCATION, TickTickBoom.DEPTH_LAYER_UI_BACKGROUND);
            AddChild(background);
            label = new TextGameObject(FONT_LOCATION, TickTickBoom.DEPTH_LAYER_UI_FOREGROUND, Color.Yellow, TextGameObject.Alignment.Center);
            label.LocalPosition = new Vector2(50, 25);
            AddChild(label);
            Reset();
        }
        #endregion
        #region Public Methods
        public override void Update(GameTime gameTime)
        {
            if (!IsRunning)
            {
                return;
            }
            double previousTimeLeft = timeLeft;

            if (!HasTimeLeftExpired)
            {
                timeLeft -= gameTime.ElapsedGameTime.TotalSeconds * TimeMultiplier;
            }
            int secondsLeft = (int)Math.Ceiling(timeLeft);
            label.Text = CreateTimeLeftString(secondsLeft);
            if (secondsLeft <= 10 && secondsLeft % 2 == 0)
            {
                label.Color = Color.Red;
            }
            else
            {
                label.Color = Color.Yellow;
            }
        }

        public override void Reset()
        {
            IsRunning = true;
            timeLeft = 30;
        }
        #endregion

        #region Private Methods
        private string CreateTimeLeftString(int secondsLeft)
        {
            return (secondsLeft / 60).ToString().PadLeft(2, '0')
            + ":"
            + (secondsLeft % 60).ToString().PadLeft(2, '0');
        }

        #endregion
    }
}