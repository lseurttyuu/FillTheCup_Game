using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillTheCup.World_elems
{
    class Timer : Component
    {
        #region Fields

        SpriteFont _timeFont;
        protected ContentManager _content;
        protected GraphicsDevice _graphicsDevice;
        string _timeView;

        #endregion

        #region Methods

        public Timer(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _timeFont = _content.Load<SpriteFont>("Fonts/Menu");

        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_timeFont, "Time", new Vector2((int)(_graphicsDevice.Viewport.Width / 5.5), (int)(_graphicsDevice.Viewport.Height / 1.1)), Color.White);
            spriteBatch.DrawString(_timeFont, _timeView, new Vector2((int)(_graphicsDevice.Viewport.Width / 1.45), (int)(_graphicsDevice.Viewport.Height / 1.1)), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void UpdateTime(float timeElapsed, float maxPlayerTime)
        {
            float timeFix = (float)(maxPlayerTime - timeElapsed - 0.02);
            if (timeFix < 0)
                timeFix = 0;

            _timeView = timeFix.ToString("F2", CultureInfo.CurrentCulture);
            _timeView += " s";
        }

        #endregion
    }
}
