using FillTheCup.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillTheCup.States
{
    /// <summary>
    /// Klasa odpowiedzialna za ekran "O grze".
    /// </summary>
    public class CreditsState : State
    {
        #region Fields

        private Texture2D _background;
        private SpriteFont _levelTitle;
        private SpriteFont _smallerFont;
        private Button backButton;
        private Song _creditsMusic;

        #endregion

        #region Methods

        /// <summary>
        /// Konstruktor stanu z ekranem "O grze". Jest zgodny z klasą abstrakcyjną State.
        /// </summary>
        /// <param name="game">Instancja gry (klasa Game1)</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        /// <param name="content">Obecny Content Manager od Monogame (klasa ContentManager)</param>
        public CreditsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = new Texture2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            _levelTitle = _content.Load<SpriteFont>("Fonts/Subtitles");
            _smallerFont = _content.Load<SpriteFont>("Fonts/Smaller");
            _creditsMusic = _content.Load<Song>("audio/music/credits");


            MediaPlayer.Stop();
            MediaPlayer.Play(_creditsMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1.0f;
            _game._musicPlaying = -1;


            var buttonTxUnc = _content.Load<Texture2D>("Controls/btn_unc_small");
            var buttonTxC = _content.Load<Texture2D>("Controls/btn_c_small");

            backButton = new Button(buttonTxUnc, buttonTxC, _smallerFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width/60f, _graphicsDevice.Viewport.Height / 1.1f),
                Text = "Back",
            };

            backButton.Click += BackButton_Click;



            Color[] backgroundColor = new Color[_graphicsDevice.Viewport.Width * _graphicsDevice.Viewport.Height];

            for (int i = 0; i < backgroundColor.Length; ++i)
                backgroundColor[i] = Color.FromNonPremultiplied(0x41, 0x1F, 0xC0, 255);

            _background.SetData(backgroundColor);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        /// <summary>
        /// Metoda odpowiedzialna za rysowanie wszystkich elementów związanych z informacjami o autorze.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(_levelTitle, "Credits", new Vector2(_graphicsDevice.Viewport.Width / 2.9f, _graphicsDevice.Viewport.Height / 80f), Color.White);
            spriteBatch.DrawString(_smallerFont, "Author", new Vector2(_graphicsDevice.Viewport.Width / 15f, _graphicsDevice.Viewport.Height / 4.4f), Color.White);
            spriteBatch.DrawString(_smallerFont, "lseurttyuu", new Vector2(_graphicsDevice.Viewport.Width / 3f, _graphicsDevice.Viewport.Height / 4.4f), Color.White);
            spriteBatch.DrawString(_smallerFont, "Music", new Vector2(_graphicsDevice.Viewport.Width / 15f, _graphicsDevice.Viewport.Height / 3.7f), Color.White);
            spriteBatch.DrawString(_smallerFont, "Bensound (Royalty free music)", new Vector2(_graphicsDevice.Viewport.Width / 3f, _graphicsDevice.Viewport.Height / 3.7f), Color.White);
            backButton.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// Metoda w której mogą być wykonywane czynności po funkcji <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void PostUpdate(GameTime gameTime)
        {
            
        }

        /// <summary>
        /// Aktualizacja, umożliwia nawigowanie z ekranu "O grze" do menu głównego.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {
            backButton.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        #endregion
    }
}
