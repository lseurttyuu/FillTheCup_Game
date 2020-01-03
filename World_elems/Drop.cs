using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;

namespace FillTheCup.World_elems
{
    /// <summary>
    /// Klasa odpowiedzialna za pojedynnczą kroplę wody.
    /// </summary>
    public class Drop : Component
    {
        #region Fields

        GraphicsDevice _graphicsDevice;
        /// <summary>
        /// Fizyczna instancja kropli.
        /// </summary>
        public Body _drop;
        private Texture2D _dropSprite;
        private Vector2 _dropOrigin;

        #endregion


        #region Methods


        /// <summary>
        /// Stworzenie jednej kropli tam, gdzie jest kran.
        /// </summary>
        /// <param name="dropSprite">Tekstura kropli.</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice).</param>
        /// <param name="_world">Świat fizyczny <c>VelcroPhysics</c> utworzony w klasie <c>Level</c>.</param>
        /// <param name="variance">Odchyłka pozycji generowanej kropli (lepsze wrażenia artystyczne).</param>
        public Drop(Texture2D dropSprite ,GraphicsDevice graphicsDevice, World _world, int variance)
        {
            _graphicsDevice = graphicsDevice;
            _dropSprite = dropSprite;


            _dropOrigin = new Vector2(_dropSprite.Width / 2, _dropSprite.Height / 2);


            Vector2 _dropPosition = ConvertUnits.ToSimUnits(new Vector2(_graphicsDevice.Viewport.Width / 2f, _graphicsDevice.Viewport.Height / 5f) + new Vector2(variance,0));


            _drop = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(5), 1, _dropPosition, BodyType.Dynamic);
            _drop.Restitution = 0.5f;
            _drop.Friction = 0f;
            _drop.Mass = 0.01f;
            _drop.ApplyForce(new Vector2(0, 5));

        }

        /// <summary>
        /// Rysowanie pojedynczej kropli.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_dropSprite, ConvertUnits.ToDisplayUnits(_drop.Position), null, Color.White, _drop.Rotation, _dropOrigin, 1f, SpriteEffects.None, 0f);
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
        /// Sprawdza, czy kropla nie wyszła poza ekran (w uproszczeniu).
        /// </summary>
        /// <returns>Jeżeli kropla wyszła poza wskazany obszar - wartość <c>true</c>.</returns>
        public bool CheckPos()
        {
            int posX = (int)ConvertUnits.ToDisplayUnits(_drop.Position.X);
            int maxY = (int)(0.03 * posX + _graphicsDevice.Viewport.Height/1.12);
            if (ConvertUnits.ToDisplayUnits(_drop.Position.Y) > maxY)
            {
                _drop.Enabled = false;
                return true;
            }
            return false;
        }

        #endregion
    }
}
