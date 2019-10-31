using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FillTheCup.World_elems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FillTheCup.States
{
    public class GameState : State
    {

        private List<Component> _components;                    //not used now!
        private List<Component> _flowers;
        private Texture2D _background;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = _content.Load<Texture2D>("World/background");
            
            _flowers = new List<Component>();

            for(int i=0; i<9; i++)
                _flowers.Add(new Flower(_content.Load<Texture2D>("World/flower_" + (i + 1)), graphicsDevice.Viewport.Width));
    }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);

            foreach (var flower in _flowers)
            {
                flower.Draw(gameTime, spriteBatch);
            }
                

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            }

        }
    }
}
