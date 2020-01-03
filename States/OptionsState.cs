using FillTheCup.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillTheCup.States
{
    /// <summary>
    /// Klasa odpowiedzialna za wyświetlanie opcji w grze.
    /// </summary>
    public class OptionsState : State
    {
        #region Fields

        private List<Button> _buttons;

        private Texture2D _background;
        private Texture2D _selectedButton;
        private Texture2D _buttonTxUnc;
        private Texture2D _buttonTxC;

        private SpriteFont _levelTitle;
        private SpriteFont _smallerFont;

        #endregion

        #region Methods

        /// <summary>
        /// Konstruktor klasy stanu z różnymi opcjami. Jest zgodny z klasą abstrakcyjną State.
        /// </summary>
        /// <param name="game">Instancja gry (klasa Game1)</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        /// <param name="content">Obecny Content Manager od Monogame (klasa ContentManager)</param>
        public OptionsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = new Texture2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            _levelTitle = _content.Load<SpriteFont>("Fonts/Subtitles");
            _smallerFont = _content.Load<SpriteFont>("Fonts/Smaller");
            _selectedButton = _content.Load<Texture2D>("Controls/btn_c_small_sel");

            _buttonTxUnc = _content.Load<Texture2D>("Controls/btn_unc_small");
            _buttonTxC = _content.Load<Texture2D>("Controls/btn_c_small");


            var easyButton = new Button(_buttonTxUnc, _buttonTxC, _smallerFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width/2.7f, _graphicsDevice.Viewport.Height / 4.5f),
                Text = "Easy",
            };

            easyButton.Click += EasyButton_Click;


            var mediumButton = new Button(_buttonTxUnc, _buttonTxC, _smallerFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 1.84f, _graphicsDevice.Viewport.Height / 4.5f),
                Text = "Medium",
            };

            mediumButton.Click += MediumButton_Click;


            var hardButton = new Button(_buttonTxUnc, _buttonTxC, _smallerFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 1.4f, _graphicsDevice.Viewport.Height / 4.5f),
                Text = "Hard",
            };

            hardButton.Click += HardButton_Click;

            var backButton = new Button(_buttonTxUnc, _buttonTxC, _smallerFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 60f, _graphicsDevice.Viewport.Height / 1.1f),
                Text = "Back",
            };

            backButton.Click += BackButton_Click;

            _buttons = new List<Button>()
            {
                easyButton,
                mediumButton,
                hardButton,
                backButton,
            };


            if(GameState._globalLvlDifficulty==1)
                _buttons[0].UpdateSelectTexture(_selectedButton, _selectedButton);
            else if(GameState._globalLvlDifficulty == 2)
                _buttons[1].UpdateSelectTexture(_selectedButton, _selectedButton);
            else
                _buttons[2].UpdateSelectTexture(_selectedButton, _selectedButton);


            Color[] backgroundColor = new Color[_graphicsDevice.Viewport.Width * _graphicsDevice.Viewport.Height];

            for (int i = 0; i < backgroundColor.Length; ++i)
                backgroundColor[i] = Color.FromNonPremultiplied(0xD6, 0x97, 0x18, 255);

            _background.SetData(backgroundColor);

        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        private void HardButton_Click(object sender, EventArgs e)
        {
            GameState._globalLvlDifficulty = 3;
            _buttons[2].UpdateSelectTexture(_selectedButton, _selectedButton);
            _buttons[1].UpdateSelectTexture(_buttonTxC, _buttonTxUnc);
            _buttons[0].UpdateSelectTexture(_buttonTxC, _buttonTxUnc);
        }

        private void MediumButton_Click(object sender, EventArgs e)
        {
            GameState._globalLvlDifficulty = 2;
            _buttons[1].UpdateSelectTexture(_selectedButton, _selectedButton);
            _buttons[2].UpdateSelectTexture(_buttonTxC, _buttonTxUnc);
            _buttons[0].UpdateSelectTexture(_buttonTxC, _buttonTxUnc);
        }

        private void EasyButton_Click(object sender, EventArgs e)
        {
            GameState._globalLvlDifficulty = 1;
            _buttons[0].UpdateSelectTexture(_selectedButton, _selectedButton);
            _buttons[1].UpdateSelectTexture(_buttonTxC, _buttonTxUnc);
            _buttons[2].UpdateSelectTexture(_buttonTxC, _buttonTxUnc);
        }

        /// <summary>
        /// Metoda odpowiedzialna za rysowanie wszystkich elementów związanych wyborem opcji.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(_levelTitle, "Options", new Vector2(_graphicsDevice.Viewport.Width / 3f, _graphicsDevice.Viewport.Height / 80f), Color.White);
            spriteBatch.DrawString(_smallerFont, "Difficulty", new Vector2(_graphicsDevice.Viewport.Width / 40f, _graphicsDevice.Viewport.Height / 4.4f), Color.White);

            foreach (var button in _buttons)
                button.Draw(gameTime, spriteBatch);

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
        /// Metoda odpowiedzialna za aktualizowanie stanów przycisków (aby można było w nie klikać).
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (var button in _buttons)
                button.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));


        }

        #endregion
    }
}
