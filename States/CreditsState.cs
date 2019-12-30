using FillTheCup.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillTheCup.States
{
    class CreditsState : State
    {
        private Texture2D _background;
        private SpriteFont _levelTitle;
        private SpriteFont _smallerFont;
        Button _backButton;

        public CreditsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = new Texture2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            _levelTitle = _content.Load<SpriteFont>("Fonts/Subtitles");
            _smallerFont = _content.Load<SpriteFont>("Fonts/Smaller");


            var buttonTxUnc = _content.Load<Texture2D>("Controls/btn_unc_small");
            var buttonTxC = _content.Load<Texture2D>("Controls/btn_c_small");

            _backButton = new Button(buttonTxUnc, buttonTxC, _smallerFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width/60f, _graphicsDevice.Viewport.Height / 1.1f),
                Text = "Back",
            };

            _backButton.Click += _backButton_Click;



            Color[] backgroundColor = new Color[_graphicsDevice.Viewport.Width * _graphicsDevice.Viewport.Height];

            for (int i = 0; i < backgroundColor.Length; ++i)
                backgroundColor[i] = Color.FromNonPremultiplied(0x41, 0x1F, 0xC0, 255);

            _background.SetData(backgroundColor);
        }

        private void _backButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(_levelTitle, "Credits", new Vector2(_graphicsDevice.Viewport.Width / 2.9f, _graphicsDevice.Viewport.Height / 80f), Color.White);
            _backButton.Draw(gameTime, spriteBatch);
            //to be filled when project finished

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            _backButton.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }
    }
}
