using System;
using System.Collections.Generic;
using FillTheCup.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.States
{
    public class MenuState : State
    {
        private List<Component> _components;
        private Texture2D _background;
        private Texture2D _backgroundGrass;
        private SpriteFont _titleFont;


        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTxUnc = _content.Load<Texture2D>("Controls/btn_unc");
            var buttonTxC = _content.Load<Texture2D>("Controls/btn_c");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Menu");

            _background = _content.Load<Texture2D>("World/background");
            _backgroundGrass = _content.Load<Texture2D>("World/grass_menu");
            _titleFont = content.Load<SpriteFont>("Fonts/Titles");


            var newGameButton = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height/2.93f),
                Text = "New game",
            };

            newGameButton.Click += newGameButton_Click;


            var optionsButton = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 2.07f),
                Text = "Options",
            };

            optionsButton.Click += OptionsButton_Click;


            var creditsButton = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 1.6f),
                Text = "Credits",
            };

            creditsButton.Click += CreditsButton_Click;


            var ExitButton = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 1.3f),
                Text = "Exit",
            };

            ExitButton.Click += QuitButton_Click;


            _components = new List<Component>()
            {
                newGameButton,
                optionsButton,
                creditsButton,
                ExitButton,
            };
        }

        private void CreditsButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new CreditsState(_game, _graphicsDevice, _content));
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new OptionsState(_game, _graphicsDevice, _content));
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            GameState._lvlDifficulty = 1;
            GameState._totalScore = 0;
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(_backgroundGrass, new Vector2(0, _graphicsDevice.Viewport.Height - _backgroundGrass.Height), Color.White);
            spriteBatch.DrawString(_titleFont, "Fill The Cup", new Vector2(_graphicsDevice.Viewport.Width / 4.6f, _graphicsDevice.Viewport.Height / 40f), Color.White);


            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }
    }
}
