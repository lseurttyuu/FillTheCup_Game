using System;
using System.Collections.Generic;
using FillTheCup.Controls;
using FillTheCup.World_elems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;

namespace FillTheCup.States
{
    public class GameState : State
    {

        //private List<Component> _components;                  //not used now!
        private List<Component> _flowers;
        private Texture2D _background;
        private Texture2D _background_grass;
        private SpriteFont _fontCounter;
        private SpriteFont _lvlFont;
        private Level _level;
        private TransBoard _transBoard;
        private PauseBoard _pauseBoard;
        private KeyboardState _previousKeyboard = Keyboard.GetState();
        private KeyboardState _currentKeyboard = Keyboard.GetState();
        
        public static int _totalScore = 0;
        private int _score = 0;

        public int _innerState = 0;                                //variable resposible for states: begining (counter), choice+animation, lvl end screen
        public int _previousState = 0;
        public float _timeElapsed = 0f;                             //gametime to measure start screen 3.. 2.. 1..
        private float _timeStamp = 0f;
        public static int _lvlDifficulty = 1;                         //single level difficulty (number of cups)
        public static int _globalLvlDifficulty = 1;                     //1 - easy, 2 - medium, 3 - hard (time restriction)
        public static float _maxPlayerTime;
        private readonly float _maxTimeEasyDef = 10;
        private readonly float _maxTimeMediumDef = 5;
        private readonly float _maxTimeHardDef = 3;


        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = _content.Load<Texture2D>("World/background");
            _background_grass = _content.Load<Texture2D>("World/background_grass");
            _fontCounter = _content.Load<SpriteFont>("Fonts/Counter");
            _lvlFont = _content.Load<SpriteFont>("Fonts/Menu");

            List<int> randomNumbersFlowers = new List<int>();
            _flowers = new List<Component>();

            for(int i=0; i<9; i++)
                _flowers.Add(new Flower(randomNumbersFlowers, _content.Load<Texture2D>("World/flower_" + (i + 1)), _graphicsDevice));

            if (_lvlDifficulty == 1)
            {
                if (_globalLvlDifficulty == 1)
                    _maxPlayerTime = _maxTimeEasyDef;
                else if (_globalLvlDifficulty == 2)
                    _maxPlayerTime = _maxTimeMediumDef;
                else
                    _maxPlayerTime = _maxTimeHardDef;
            }
            


        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();

            if (_innerState == -1)
                _pauseBoard.Draw(gameTime, spriteBatch);

            if (_innerState >= 0)
            {
                spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(_lvlFont, "Level " + _lvlDifficulty, new Vector2((int)(_graphicsDevice.Viewport.Width / 4), 10), Color.White);
            }
                


            if (_innerState == 0)
            {
                string[] counter = { (3 - (int)_timeElapsed).ToString(), "Go!" };
                short final_count = 0;
                if ((int)_timeElapsed == 3)
                    final_count = 1;

                int posX = (_graphicsDevice.Viewport.Width - (int)_fontCounter.MeasureString(counter[final_count]).X) / 2;
                int posY = (_graphicsDevice.Viewport.Height - (int)_fontCounter.MeasureString(counter[final_count]).Y) / 2;
                spriteBatch.DrawString(_fontCounter, counter[final_count], new Vector2(posX, posY), Color.Black);

                    
            }


            if (_innerState > 0)
                foreach (var flower in _flowers)
                    flower.Draw(gameTime, spriteBatch);

            if(_innerState!=-1)
                spriteBatch.Draw(_background_grass, new Vector2(0, _graphicsDevice.Viewport.Height - _background_grass.Height), Color.White);

            if (_innerState > 0)
                _level.Draw(gameTime, spriteBatch);


            if (_innerState == 4)
            {
                _transBoard.Draw(gameTime, spriteBatch);
            }
           



            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (_innerState == -1)
                _pauseBoard.Update(gameTime);
            else
                _timeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f;
                
            
            if (_timeElapsed > 4 && _innerState == 0)
            {
                _level = new Level(this, _game, _graphicsDevice, _content, _lvlDifficulty);
                _innerState++;
                _timeElapsed = 0;
            }

            if (_innerState == 2 && _timeStamp == 0f)
                _timeStamp = _timeElapsed;


            if (_innerState > 0 && _innerState < 3 && _timeElapsed > _maxPlayerTime && _level._hasChosen==false)
                _innerState = 3;
            else if(_level != null && _innerState!=-1)
            {
                _level.Update(gameTime);
                if(_level._endLvl ==false && _level._hasChosen == false)
                    _level.UpdateBar(_timeElapsed, _maxPlayerTime);
                    
            }
                

            if(_innerState == 3)
            {
                _level._endLvl = true;
                if (_level._hasWon)
                    _score = (400 + _lvlDifficulty * (100 - (int)(_timeStamp * 20)))*_globalLvlDifficulty;
                _totalScore += _score;


                _transBoard = new TransBoard(_game, _graphicsDevice, _content, _score, _totalScore, _level._hasWon);
                _innerState++;

            }

            if(_innerState == 4)
            {
                _transBoard.Update(gameTime);
            }


            _previousKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();

            if (_previousKeyboard != _currentKeyboard)
            {
                if (_currentKeyboard.IsKeyDown(Keys.Escape))
                {
                        if (_innerState != -1)
                        {
                            _previousState = _innerState;
                            _innerState = -1;
                            _pauseBoard = new PauseBoard(_game, this, _graphicsDevice, _content);
                        }
                        else
                        {
                            if (_previousState == 1)
                            {
                                _timeElapsed += (float)((_maxPlayerTime - _timeElapsed) * 0.3);
                            }
                        _innerState = _previousState;
                        }
                }
            }



        }
    }
}
