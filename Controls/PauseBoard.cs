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
    class PauseBoard : Component
    {

        #region Fields

        protected Game1 _game;
        protected Level _level;
        protected GameState _gameState;
        protected GraphicsDevice _graphicsDevice;
        protected ContentManager _content;

        private List<Component> _components;
        private Texture2D _background;
        private SpriteFont _levelTitle;
        private SpriteFont _smallerFont;

        #endregion

        #region Methods

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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(_levelTitle, "Level "+GameState._lvlDifficulty, new Vector2(_graphicsDevice.Viewport.Width / 2.8f, 10), Color.White);
            spriteBatch.DrawString(_smallerFont, "Warning! Every pause costs you 30% time penalty!", new Vector2(_graphicsDevice.Viewport.Width / 80f, _graphicsDevice.Viewport.Height / 1.05f), Color.White);

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }
        #endregion
    }
}
