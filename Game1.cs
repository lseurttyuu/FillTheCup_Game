using FillTheCup.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FillTheCup
{
    /// <summary>
    /// Klasa definiująca całą grę zgodnie z wytycznymi frameworku Monogame
    /// </summary>
    public class Game1 : Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private State _currentState;
        private State _nextState;
        public short _musicPlaying = -2;          //means not playing anything

        #endregion

        #region Methods


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Pozwala grze na wykonanie dowolnej inicjalizacji przed uruchomieniem.
        /// W tym miejscu można wyszukiwać dowolne wymagane usługi (treści) i je załadować.
        /// </summary>
        protected override void Initialize()
        {
            Texture2D cursor = Content.Load<Texture2D>("Controls/cursor");
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 0, 0));
            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContentjest uruchamiany tylko raz i pozwala załadować
        /// wszystkie zdefiniowane treści.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);                              // Create a new SpriteBatch, which can be used to draw textures.
            _currentState = new MenuState(this, graphics.GraphicsDevice, Content);
        }

        /// <summary>
        /// UnloadContent pozwala zwolnić zasoby związane z grą.
        /// Ta funkcja jest uruchamiana tylko 1 raz.
        /// </summary>
        protected override void UnloadContent()
        {
            //Unload any non ContentManager content here
        }


        public void ChangeState(State state)
        {
            _nextState = state;
        }

        /// <summary>
        /// Umożliwia działanie całej logiki w grze, takiej jak aktualizowanie świata,
        /// sprawdzanie kolizji, zbieranie danych wejściowych i odtwarzanie dźwięku.
        /// </summary>
        /// <param name="gameTime">Parametr pozwala uzyskać czas gry (moment).</param>
        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);         //Function now unused;

            base.Update(gameTime);
        }

        /// <summary>
        /// Funkcja odpowiedzialna za rysowanie wszelkich elementów.
        /// </summary>
        /// <param name="gameTime">Parametr pozwala uzyskać czas gry (moment).</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _currentState.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        #endregion
    }
}
