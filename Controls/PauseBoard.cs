using FillTheCup.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillTheCup.Controls
{
    /// <summary>
    /// Klasa odpowiedzialna za ekran pauzy.
    /// </summary>
    public class PauseBoard : Component
    {

        #region Fields

        /// <summary>
        /// Obiekt _game dla możliwości utworzenia nowego stanu (kolejny poziom/koniec gry - menu).
        /// </summary>
        protected Game1 _game;
        /// <summary>
        /// Obiekt _level dla dostępu do pól dotyczących obsługi efektów dźwiękowych.
        /// </summary>
        protected Level _level;
        /// <summary>
        /// Obecny stan reprezentujący dany poziom (w szerokim znaczeniu). Służy do zastosowania kary czasowej za pauzę gry.
        /// </summary>
        protected GameState _gameState;
        /// <summary>
        /// Pole _graphicsDevice służące dla dostępu do właściwości i pól okna graficznego
        /// </summary>
        protected GraphicsDevice _graphicsDevice;
        /// <summary>
        /// Obiekt _content dla dostępu do zawartości ładowanej przez Monogame Content Pipeline
        /// </summary>
        protected ContentManager _content;

        private List<Component> _components;
        private Texture2D _background;
        private SpriteFont _levelTitle;
        private SpriteFont _smallerFont;

        #endregion

        #region Methods

        /// <summary>
        /// Konstruktor ekranu pauzy umożliwia stworzenie nowego ekranu pauzy.
        /// </summary>
        /// <param name="game">Instancja gry (klasa Game1)</param>
        /// <param name="gameState">Obecna instancja poziomu (stan <c>GameState</c>).</param>
        /// <param name="level">Obecny poziom (klasa Level).</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        /// <param name="content">Obecny Content Manager od Monogame (klasa ContentManager)</param>
        public PauseBoard(Game1 game, GameState gameState, Level level, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;
            _level = level;
            _graphicsDevice = graphicsDevice;
            _content = content;
            _gameState = gameState;
            

            var buttonTxUnc = _content.Load<Texture2D>("Controls/btn_unc");
            var buttonTxC = _content.Load<Texture2D>("Controls/btn_c");
            var buttonFont = _content.Load<SpriteFont>("Fonts/Menu");
            _levelTitle = _content.Load<SpriteFont>("Fonts/Subtitles");
            _smallerFont = _content.Load<SpriteFont>("Fonts/Smaller");

            _background = new Texture2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            Color[] backgroundColor = new Color[_graphicsDevice.Viewport.Width*_graphicsDevice.Viewport.Height];

            for(int i=0; i<backgroundColor.Length; ++i)
                backgroundColor[i]= Color.FromNonPremultiplied(0x46, 0x46, 0x46, 255);

            _background.SetData(backgroundColor);


            var resumeButton = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 2.5f),
                Text = "Resume",
            };

            resumeButton.Click += ResumeButton_Click;


            var quitButton = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 1.8f),
                Text = "Quit",
            };

            quitButton.Click += QuitButton_Click;


            _components = new List<Component>()
            {
                resumeButton,
                quitButton,
            };

        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            if (_level != null)
            {
                _level._waterStart.Stop();
                _level._waterLoop.Stop();
            }
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        private void ResumeButton_Click(object sender, EventArgs e)
        {
            if (_level != null && _level._hasChosen)
            {
                _level._soundFxState = 2;
                _level._waterLoop.Play();
            }

            if (_gameState._previousState == 1)
                _gameState._timeElapsed += (float)((GameState._maxPlayerTime-_gameState._timeElapsed) * 0.3);

            _gameState._innerState = _gameState._previousState;

        }

        /// <summary>
        /// Metoda odpowiedzialna za rysowanie tablicy pauzy. Brak <c>spriteBarch.Begin()</c> oraz <c>spriteBatch.End()</c>
        /// jest spowodowane występowaniem tych wywołań w klasie wyższej.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(_levelTitle, "Level "+GameState._lvlDifficulty, new Vector2(_graphicsDevice.Viewport.Width / 2.8f, 10), Color.White);
            spriteBatch.DrawString(_smallerFont, "Warning! Every pause costs you 30% time penalty!", new Vector2(_graphicsDevice.Viewport.Width / 80f, _graphicsDevice.Viewport.Height / 1.05f), Color.White);

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Aktualizowanie przycisków będących na rysowanej tablicy.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }
        #endregion
    }
}
