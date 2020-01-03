using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.World_elems
{
    /// <summary>
    /// Klasa odpowiedzialna za stworzenie obiektów dekoracyjnych (kwiatów) w danym poziomie <c>Level</c>.
    /// </summary>
    public class Flower : Component
    {
        #region Fields

        private Texture2D _texture;
        private static readonly Random random = new Random();

        #endregion

        /// <summary>
        /// Pozycja kwiatów.
        /// </summary>
        public Vector2 Position { get; set; }


        #region Methods

        /// <summary>
        /// Konstruktor kwiatów. Jest wywoływany tyle razy, ile potrzeba stworzyć kwiatów w danym poziomie.
        /// </summary>
        /// <param name="randomNumbers">Lista losowych liczb, która wspomaga generowanie losowych, niepowtarzających się pozycji kwiatów.</param>
        /// <param name="texture">Tekstura danej rośliny.</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
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



        /// <summary>
        /// Rysowanie rośliny.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }

        /// <summary>
        /// Aktualizacja; Nieużywana w tej klasie!
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
