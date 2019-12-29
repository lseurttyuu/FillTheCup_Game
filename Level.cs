using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Utilities;
using FillTheCup.World_elems;
using FillTheCup.States;

namespace FillTheCup
{
    public class Level
    {

        #region Fields

        protected ContentManager _content;
        protected Game1 _game;
        protected GameState _gameState;
        protected GraphicsDevice _graphicsDevice;

        public bool _hasChosen = false;                                    //for 2 states: choice (0) and animation (1)
        public int _cupChosen = 0;                                  //to be filled with number of a cup that has been choosen
        private int _cupWon = 0;
        public bool _hasWon = false;                                       //information to know what trigger after particular level is finished
        public bool _endLvl = false;                                       //if level ended - trigger next state in GameState

        private readonly World _physxWorld;
        private List<Cup> _cups;
        private List<Drop> _drops;
        private List<Component> _pipes;
        private Texture2D _dropTex;
        private Texture2D _tap_open;
        private Texture2D _tap_closed;
        private Texture2D _grassBar;
        private Texture2D _bar;
        private Vector2 _barPos;
        private Timer _timer;

        private int _updateCounter;
        private static readonly int _dropsStrength = 3;             // the lower number - the more drops are generated (linearly) - from 1 to inf;


        private Random random;

        #endregion



        public Level(GameState gameState, Game1 game, GraphicsDevice graphicsDevice, ContentManager content, int difficulty)
        {
            _game = game;
            _gameState = gameState;
            _content = content;
            _graphicsDevice = graphicsDevice;
            random = new Random();
            _hasChosen = false;

            _updateCounter = 0;

            _timer = new Timer(_content, _graphicsDevice);
            _cups = new List<Cup>();
            _pipes = new List<Component>();
            _dropTex = _content.Load<Texture2D>("World/drop_tx");
            _drops = new List<Drop>();
            _tap_open = _content.Load<Texture2D>("World/tap_open");
            _tap_closed = _content.Load<Texture2D>("World/tap_closed");
            _grassBar = _content.Load<Texture2D>("World/bckg_grass_bar");

            int barWidth = (int)(_graphicsDevice.Viewport.Width / 2.85);
            int barHeight = (int)(_graphicsDevice.Viewport.Height / 15.36);


            _bar = new Texture2D(_graphicsDevice, barWidth, barHeight);
            Color[] barColor = new Color[barWidth*barHeight];

            for(int i=0; i < barColor.Length; ++i)
               barColor[i] = Color.FromNonPremultiplied(0xFF, 0xFF, 0xDD, 255);

            _bar.SetData(barColor);

            _barPos = new Vector2((int)(_graphicsDevice.Viewport.Width / 2 - barWidth / 2), (int)(_graphicsDevice.Viewport.Height * 0.920572917));


            _physxWorld = new World(new Vector2(0, 20));                //new physics connected world (strong gravity 16g)
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);             //64 pixels = 1 meter when it comes to simulation



            #region temp_button



            #endregion





            //add cups - different difficulty level - different number of cups
            //switch (difficulty)
            switch(1)
            {
                case 1:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.8f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 3));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0));
                    break;

                default:    break;
            }

            

        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {


            spriteBatch.Draw(_bar, _barPos, Color.White);
            spriteBatch.Draw(_grassBar, new Vector2(0, _graphicsDevice.Viewport.Height - _grassBar.Height), Color.White);
            _timer.Draw(gameTime, spriteBatch);


            #region temp1
            foreach (var drop in _drops)
            {
                drop.Draw(gameTime, spriteBatch);
            }

            #endregion

            foreach (var cup in _cups)
                cup.Draw(gameTime, spriteBatch);
            
            foreach (var pipe in _pipes)
                pipe.Draw(gameTime, spriteBatch);

            if (_hasChosen)
                spriteBatch.Draw(_tap_open, new Vector2(_graphicsDevice.Viewport.Width - _tap_open.Width, 0), Color.White);
            else
                spriteBatch.Draw(_tap_closed, new Vector2(_graphicsDevice.Viewport.Width - _tap_closed.Width, 0), Color.White);



        }

        public void Update(GameTime gameTime)
        {

            if (_hasChosen)
            {
                if (_gameState._innerState == 1)
                    _gameState._innerState++;

                _updateCounter++;

                if (_updateCounter==_dropsStrength)
                {
                    _drops.Add(new Drop(_dropTex, _graphicsDevice, _physxWorld, random.Next(-10, 10)));
                    _updateCounter = 0;
                }
                

                if (_endLvl == false)
                {
                    if (_cupWon == 0)
                    {
                        int currentCup = 1;
                        foreach (var cup in _cups)
                        {
                            int drops_inside = 0;
                            foreach (Drop drop in _drops)
                            {
                                if (ConvertUnits.ToDisplayUnits(drop._drop.Position).X > cup._posX - Cup._cup_X / 2 &&
                                    ConvertUnits.ToDisplayUnits(drop._drop.Position).X < cup._posX + Cup._cup_X / 2 &&
                                    ConvertUnits.ToDisplayUnits(drop._drop.Position).Y > cup._posY - Cup._cup_Y / 2 &&
                                    ConvertUnits.ToDisplayUnits(drop._drop.Position).Y < cup._posY + Cup._cup_Y / 2)
                                {
                                    drops_inside++;
                                }
                            }
                            if (drops_inside > 85)
                            {
                                _cupWon = currentCup;
                                break;
                            }
                            currentCup++;
                        }

                    }
                    else
                    {
                        _endLvl = true;
                        _gameState._innerState++;
                        if (_cupWon == _cupChosen)
                        {
                            _hasWon = true;
                            
                        }
                        else
                        {
                            _hasWon = false;

                        }


                    }

                    //time bar to be updated
                }
                
                



            }
            else
            {
                foreach (var cup in _cups)
                    cup.Update(gameTime);
            }


            









            _physxWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);



            #region temp3

            for (int i = _drops.Count - 1; i >= 0; i--)
            {
                if (_drops[i].CheckPos())
                    _drops.RemoveAt(i);
            
            }
            #endregion

        }

        public void UpdateBar(float timeElapsed, float maxPlayerTime)
        {
            _barPos.X = (int)(_graphicsDevice.Viewport.Width / 2 - _bar.Width / 2 - _bar.Width*timeElapsed/maxPlayerTime);
            _timer.UpdateTime(timeElapsed, maxPlayerTime);
        }
    }
}
