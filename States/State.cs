using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.States
{
    /// <summary>
    /// Klasa abstrakcyjna umożliwiająca tworzenie różnych klas stanów.
    /// </summary>
    public abstract class State
    {

        #region Fields

        /// <summary>
        /// Obiekt _content dla dostępu do zawartości ładowanej przez Monogame Content Pipeline.
        /// </summary>
        protected ContentManager _content;
        /// <summary>
        /// Obiekt _game dla możliwości utworzenia nowego stanu (kolejny poziom/koniec gry - menu).
        /// </summary>
        protected Game1 _game;
        /// <summary>
        /// Pole _graphicsDevice służące dla dostępu do właściwości i pól okna graficznego.
        /// </summary>
        protected GraphicsDevice _graphicsDevice;

        #endregion

        #region Methods

        /// <summary>
        /// Rysowanie elementów danego stanu.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Aktualizacja elementów danego stanu po wykonaniu kodu zawartego w <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public abstract void PostUpdate(GameTime gameTime);

        /// <summary>
        /// Definicja konstruktora.
        /// </summary>
        /// <param name="game">Instancja gry (klasa Game1)</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        /// <param name="content">Obecny Content Manager od Monogame (klasa ContentManager)</param>
        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
        }

        /// <summary>
        /// Aktualizacja elementów danego stanu.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public abstract void Update(GameTime gameTime);

        #endregion

    }
}
