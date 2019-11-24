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
        private Level _level;
        private TransBoard _transBoard;
        public static int _totalScore = 0;
        private int _score = 0;

        public int _innerState = 0;                                //variable resposible for states: begining (counter), choice+animation, lvl end screen
        private float _timeElapsed = 0f;                             //gametime to measure start screen 3.. 2.. 1..
        private float _timeStamp = 0f;
        public static int _lvlDifficulty = 1;                         //single level difficulty (number of cups)


        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = _content.Load<Texture2D>("World/background");
            _background_grass = _content.Load<Texture2D>("World/background_grass");
            _fontCounter = _content.Load<SpriteFont>("Fonts/Counter");


            _flowers = new List<Component>();

            for(int i=0; i<9; i++)
                _flowers.Add(new Flower(_content.Load<Texture2D>("World/flower_" + (i + 1)), _graphicsDevice.Viewport.Width));


        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();


            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);

            
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
                

            if(_innerState > 0)
            {
                foreach (var flower in _flowers)
                    flower.Draw(gameTime, spriteBatch);


                spriteBatch.Draw(_background_grass, new Vector2(0, _graphicsDevice.Viewport.Height - _background_grass.Height), Color.White);

                _level.Draw(gameTime, spriteBatch);

            }


            if(_innerState == 4)
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


            _timeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f;
            if (_timeElapsed > 4 && _innerState == 0)
            {
                _level = new Level(this, _game, _graphicsDevice, _content, _lvlDifficulty);
                _innerState++;
                _timeElapsed = 0;
            }

            if (_innerState == 2 && _timeStamp == 0f)
                _timeStamp = _timeElapsed;
            


            if (_timeElapsed > 5 && _level._hasChosen==false && _innerState!=0 && _innerState<3)
                _innerState = 3;
            else if(_level != null)
                _level.Update(gameTime);

            if(_innerState == 3)
            {
                _level._endLvl = true;
                if (_level._hasWon)
                    _score = 400 + _lvlDifficulty * (100 - (int)(_timeStamp * 20));
                _totalScore += _score;
                _lvlDifficulty++;


                _transBoard = new TransBoard(_game, _graphicsDevice, _content, _score, _totalScore, _level._hasWon);
                _innerState++;

            }

            if(_innerState == 4)
            {
                _transBoard.Update(gameTime);
            }




            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            }

        }
    }
}
