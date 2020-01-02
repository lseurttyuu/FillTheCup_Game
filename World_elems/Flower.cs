using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.World_elems
{
    class Flower : Component
    {
        #region Fields

        private Texture2D _texture;
        private static Random random = new Random();

        #endregion

        public Vector2 Position { get; set; }


        #region Methods
        public Flower(List<int> randomNumbers, Texture2D texture, GraphicsDevice graphicsDevice)
        {
            _texture = texture;

            int smallPositionX;

            do
            {
                smallPositionX = random.Next(1, 18);
            } while (randomNumbers.Contains(smallPositionX));
            randomNumbers.Add(smallPositionX);

            Position = new Vector2(smallPositionX*(int)(graphicsDevice.Viewport.Width/18.618), (int)(0.03* smallPositionX*graphicsDevice.Viewport.Width / 18.618 + graphicsDevice.Viewport.Height/1.35));
        }



        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
