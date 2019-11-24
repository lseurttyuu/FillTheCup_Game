using System;
using System.Collections.Generic;
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
        private SpriteFont _font;
        private Level _level;

        public int _innerState;                                //variable resposible for states: begining (counter), choice+animation, lvl end screen
        private float _timeElapsed;                             //gametime to measure start screen 3.. 2.. 1..
        private int _lvlDifficulty = 1;                         //single level difficulty (number of cups)


        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {

            _innerState = new int();
            _innerState = 0;

            _timeElapsed = new float();
            _timeElapsed = 0f;


            _background = _content.Load<Texture2D>("World/background");
            _background_grass = _content.Load<Texture2D>("World/background_grass");
            _font = _content.Load<SpriteFont>("Fonts/Menu");                                    //to be modified --> another font size, color, etc


            _flowers = new List<Component>();

            for(int i=0; i<9; i++)
                _flowers.Add(new Flower(_content.Load<Texture2D>("World/flower_" + (i + 1)), _graphicsDevice.Viewport.Width));


        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();


            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);


            if (_innerState == 0)
                if((int)_timeElapsed==3)
                    spriteBatch.DrawString(_font, "Go!", new Vector2(100, 100), Color.White);
                else
                    spriteBatch.DrawString(_font, (3 - (int)_timeElapsed).ToString(), new Vector2(100, 100), Color.White);

            if(_innerState > 0)
            {
                foreach (var flower in _flowers)
                    flower.Draw(gameTime, spriteBatch);


                spriteBatch.Draw(_background_grass, new Vector2(0, _graphicsDevice.Viewport.Height - _background_grass.Height), Color.White);

                _level.Draw(gameTime, spriteBatch);

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
            


            if (_timeElapsed > 5 && _level._hasChosen==false && _innerState!=0)
                _innerState = 3;
            else if(_level != null)
                _level.Update(gameTime);

            if(_innerState == 3)
            {

            }




            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            }

        }
    }
}
