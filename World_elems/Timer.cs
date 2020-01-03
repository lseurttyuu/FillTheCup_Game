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
    /// <summary>
    /// Klasa definiująca licznik czasu (do wyświetlenia podczas trwania danego poziomu w znaczeniu <c>Level</c>).
    /// </summary>
    public class Timer : Component
    {
        #region Fields

        SpriteFont _timeFont;
        /// <summary>
        /// Obiekt _content dla dostępu do zawartości ładowanej przez Monogame Content Pipeline
        /// </summary>
        protected ContentManager _content;
        /// <summary>
        /// Pole _graphicsDevice służące dla dostępu do właściwości i pól okna graficznego
        /// </summary>
        protected GraphicsDevice _graphicsDevice;
        string _timeView;

        #endregion

        #region Methods

        /// <summary>
        /// Konstruktor licznika czasu.
        /// </summary>
        /// <param name="content">Obecny Content Manager od Monogame (klasa ContentManager)</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        public Timer(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _timeFont = _content.Load<SpriteFont>("Fonts/Menu");

        }


        /// <summary>
        /// Rysowanie licznika czasu.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_timeFont, "Time", new Vector2((int)(_graphicsDevice.Viewport.Width / 5.5), (int)(_graphicsDevice.Viewport.Height / 1.1)), Color.White);
            spriteBatch.DrawString(_timeFont, _timeView, new Vector2((int)(_graphicsDevice.Viewport.Width / 1.45), (int)(_graphicsDevice.Viewport.Height / 1.1)), Color.White);
        }

        /// <summary>
        /// Aktualizacja; Nieużywana w tej klasie!
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Metoda odpowiedzialna za aktualizację pozycji paska z pozostałym czasem.
        /// </summary>
        /// <param name="timeElapsed">Czas który upłynął od początku poziomu (tj. odkąd pojawiły się kubeczki).</param>
        /// <param name="maxPlayerTime">Maksymalny czas na podjęcie decyzji.</param>
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
