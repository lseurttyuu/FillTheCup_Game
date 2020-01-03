using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup
{
    /// <summary>
    /// Klasa abstrakcyjna <c>Component</c> pozwalająca tworzyć różne elementy świata, które
    /// muszą być aktualizowane i rysowane na ekranie.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Abstrakcyjna metoda rysująca obiekt.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Abstrakcyjna metoda aktualizująca wskazany obiekt.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public abstract void Update(GameTime gameTime);
    }
}
