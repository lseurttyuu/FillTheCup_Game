using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FillTheCup.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.Controls
{
    /// <summary>
    /// Klasa przejściowa, która jest odpowiedzialna za prezentację ekranu pomiędzy dwoma kolejnymi poziomami
    /// </summary>
    public class TransBoard : Component
    {

        #region Fields

        string[] _infoText = { "Well done!", "Game over", "Score:", "Total score:","","" };

        protected ContentManager _content;
        protected Game1 _game;
        protected Level _level;
        protected GraphicsDevice _graphicsDevice;
        private List<SpriteFont> _spriteFonts;              //From BIG ones to small ones; Just like in html - h0, h1, etc
        private Texture2D _layerTrans;
        private Texture2D _layerSolid;
        private Vector2 _laSoSize;
        private Vector2 _laSoOrigin;

        private List<Component> _buttons;

        private int _score;
        private int _totalScore;
        private bool _hasWon;

        #endregion


        #region Methods
        public TransBoard(Game1 game, Level level, GraphicsDevice graphicsDevice, ContentManager content, int score, int totalScore, bool hasWon)
        {
            _game = game;
            _level = level;
            _graphicsDevice = graphicsDevice;
            _content = content;
            _hasWon = hasWon;

            _score = score;
            _totalScore = totalScore;
            _spriteFonts = new List<SpriteFont>()
            {
                _content.Load<SpriteFont>("Fonts/Titles"),
                _content.Load<SpriteFont>("Fonts/Score"),
            };

            _infoText[4] = _score.ToString();
            _infoText[5] = _totalScore.ToString();


            _layerTrans = new Texture2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

            _laSoSize = new Vector2((int)(_graphicsDevice.Viewport.Width * 0.7578125f), (int)(_graphicsDevice.Viewport.Height * 0.78125f));
            _laSoOrigin = new Vector2((_graphicsDevice.Viewport.Width - _laSoSize.X) / 2, (_graphicsDevice.Viewport.Height - _laSoSize.Y) / 2);
            _layerSolid = new Texture2D(_graphicsDevice, (int)_laSoSize.X, (int)_laSoSize.Y);


            Color[] transColor = new Color[_graphicsDevice.Viewport.Width * _graphicsDevice.Viewport.Height];

            for (int i = 0; i < transColor.Length; ++i)
                if(_hasWon)
                    transColor[i] = Color.FromNonPremultiplied(0x10, 0x72, 0x26, 125);
                else
                    transColor[i] = Color.FromNonPremultiplied(0xCC, 0x33, 0x4A, 165);
            _layerTrans.SetData(transColor);

            Color[] solidColor = new Color[(int)_laSoSize.X * (int)_laSoSize.Y];

            for(int i=0; i<solidColor.Length;++i)
                if(_hasWon)
                    solidColor[i] = Color.FromNonPremultiplied(0x24, 0xB1, 0xDB, 200);
                else
                    solidColor[i] = Color.FromNonPremultiplied(0x72, 0x10, 0x30, 200);
            _layerSolid.SetData(solidColor);


            Texture2D buttonTxUnc = _content.Load<Texture2D>("Controls/btn_unc");
            Texture2D buttonTxC = _content.Load<Texture2D>("Controls/btn_c");
            SpriteFont buttonFont = _content.Load<SpriteFont>("Fonts/Menu");


            Button nextLevel = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 1.8f),
                Text = "Next level",
            };

            nextLevel.Click += NextLevel_Click;


            Button newGame = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 1.8f),
                Text = "New game",
            };

            newGame.Click += NextLevel_Click;


            Button quitLvl = new Button(buttonTxUnc, buttonTxC, buttonFont)
            {
                Position = new Vector2(_graphicsDevice.Viewport.Width / 2 - buttonTxUnc.Width / 2, (int)_graphicsDevice.Viewport.Height / 1.4f),
                Text = "Quit",
            };

            quitLvl.Click += QuitLvl_Click;


            _buttons = new List<Component>();


            if (_hasWon)
                _buttons.Add(nextLevel);
            else
                _buttons.Add(newGame);
            _buttons.Add(quitLvl);

        }

        private void QuitLvl_Click(object sender, EventArgs e)
        {
            _level._waterStart.Stop();
            _level._waterLoop.Stop();
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }

        private void NextLevel_Click(object sender, EventArgs e)
        {
            _level._waterStart.Stop();
            _level._waterLoop.Stop();
            if (!_hasWon)
            {
                GameState._lvlDifficulty = 0;
                GameState._totalScore = 0;
            }

            GameState._lvlDifficulty++;
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_layerTrans, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(_layerSolid, _laSoOrigin, Color.White);

            foreach (var component in _buttons)
                component.Draw(gameTime, spriteBatch);

            if (_hasWon)
            {
                spriteBatch.DrawString(_spriteFonts[0], _infoText[0], new Vector2((_graphicsDevice.Viewport.Width - _spriteFonts[0].MeasureString(_infoText[0]).X) / 2, _graphicsDevice.Viewport.Height / 10f), Color.White);
                spriteBatch.DrawString(_spriteFonts[1], _infoText[2], new Vector2(_graphicsDevice.Viewport.Width / 6.9f, _graphicsDevice.Viewport.Height / 3.75f), Color.White);
                spriteBatch.DrawString(_spriteFonts[1], _infoText[4], new Vector2(_graphicsDevice.Viewport.Width / 1.65f, _graphicsDevice.Viewport.Height / 3.75f), Color.White);
                spriteBatch.DrawString(_spriteFonts[1], _infoText[3], new Vector2(_graphicsDevice.Viewport.Width / 6.9f, _graphicsDevice.Viewport.Height / 2.63f), Color.White);
                spriteBatch.DrawString(_spriteFonts[1], _infoText[5], new Vector2(_graphicsDevice.Viewport.Width / 1.65f, _graphicsDevice.Viewport.Height / 2.63f), Color.White);
            }
            else
            {
                spriteBatch.DrawString(_spriteFonts[0], _infoText[1], new Vector2((_graphicsDevice.Viewport.Width - _spriteFonts[0].MeasureString(_infoText[1]).X) / 2, _graphicsDevice.Viewport.Height / 10f), Color.White);
                spriteBatch.DrawString(_spriteFonts[1], _infoText[3], new Vector2(_graphicsDevice.Viewport.Width / 6.9f, _graphicsDevice.Viewport.Height / 2.75f), Color.White);
                spriteBatch.DrawString(_spriteFonts[1], _infoText[5], new Vector2(_graphicsDevice.Viewport.Width / 1.65f, _graphicsDevice.Viewport.Height / 2.75f), Color.White);
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _buttons)
                component.Update(gameTime);
        }

        #endregion
    }
}
