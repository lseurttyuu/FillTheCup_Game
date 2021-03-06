﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FillTheCup.Controls
{
    /// <summary>
    /// Klasa odpowiedzialna za przyciski (z naniesionymi teksturami).
    /// </summary>
    public class Button : Component
    {
        #region Fields

        private MouseState _currentMouse;
        private readonly SpriteFont _font;
        private bool _isHovering;
        private MouseState _previousMouse;
        private Texture2D _texture;
        private Texture2D _texture_c;
        private Texture2D _texture_visible;

        #endregion


        #region Properties
        /// <summary>
        /// Reprezentacja metody, która obsłuży zdarzenie bez dodatkowych danych o wydarzeniu.
        /// </summary>
        public event EventHandler Click;
        /// <summary>
        /// Kolor tekstu wyświetlanego w środku przycisku.
        /// </summary>
        public Color PenColour { get; set; }
        /// <summary>
        /// Pozycja przycisku.
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Prostokąt będący reprezentacją przycisku.
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }
        /// <summary>
        /// Tekst wyświetlany w środku przycisku.
        /// </summary>
        public string Text { get; set; }

        #endregion


        #region Methods
        /// <summary>
        /// Konstruktor przycisku.
        /// </summary>
        /// <param name="texture">Tekstura dla przycisku w momencie kiedy myszka nie jest nad przyciskiem.</param>
        /// <param name="texture_c">Tekstura dla przycisku w momencie kiedy myszja jest nad przyciskiem.</param>
        /// <param name="font">Czcionka jaką ma się wyświetlać tekst w środku przycisku.</param>
        public Button(Texture2D texture, Texture2D texture_c, SpriteFont font)
        {
            _texture = texture;
            _texture_c = texture_c;
            _font = font;
            PenColour = Color.Black;
        }

        /// <summary>
        /// Rysowanie przycisku.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_isHovering)
                _texture_visible = _texture_c;
            else
                _texture_visible = _texture;

            spriteBatch.Draw(_texture_visible, Rectangle, Color.White);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        /// <summary>
        /// Aktualizacja przycisku (czy myszka nie jest nad nim oraz czy nie został on wciśnięty).
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Aktualizacja tekstur w celu wyróżnienia zaznaczonego przycisku (przydatne przy opcjach).
        /// </summary>
        /// <param name="selected">Tekstura aktualizowana dla przycisku, gdy myszka jest nad przyciskiem.</param>
        /// <param name="unselected">Tekstura aktualizowana dla przycisku, gdy myszka nie jest nad przyciskiem.</param>
        public void UpdateSelectTexture(Texture2D selected, Texture2D unselected)
        {
            _texture_c = selected;
            _texture = unselected;
        }

        #endregion
    }
}
