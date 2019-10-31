using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FillTheCup.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.States
{
    public class MenuState : State
    {
        private List<Component> _components;


        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTxUnc = _content.Load<Texture2D>("Controls/btn_unc");
            var buttonTxC = _content.Load<Texture2D>("Controls/btn_c");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Menu");

            var newGameButton = new Button(buttonTxUnc, buttonFont)
            {
                Position = new Vector2(300, 200),
                Text = "Start",
            };

            newGameButton.Click += newGameButton_Click;



            var QuitButton = new Button(buttonTxUnc, buttonFont)
            {
                Position = new Vector2(300, 300),
                Text = "Quit",
            };

            QuitButton.Click += QuitButton_Click;

            _components = new List<Component>()
            {
                newGameButton,
                QuitButton,
            };
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            //remove sprites - TODO!!!!
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }
    }
}
