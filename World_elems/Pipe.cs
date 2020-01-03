using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.Dynamics;

namespace FillTheCup.World_elems
{
    /// <summary>
    /// Klasa abstrakcyjna odpowiedzialna za rury (w przypadku gdyby zaszła potrzeba konstrukcji większej liczby rodzajów rur).
    /// </summary>
    public abstract class Pipe : Component
    {
        /// <summary>
        /// Rysowanie rur.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public abstract override void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Aktualizacja rur.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public abstract override void Update(GameTime gameTime);
    }
}
