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


namespace FillTheCup
{
    public class Level
    {

        #region Fields

        protected ContentManager _content;
        protected Game1 _game;
        protected GraphicsDevice _graphicsDevice;

        private bool _hasChosen;                                    //for 2 states: choice (0) and animation (1)
        private bool _hasWon;                                       //information to know what trigger after particular level is finished
        private bool _endLvl;                                       //if level ended - trigger next state in GameState

        private readonly World _physxWorld;
        private List<Cup> _cupsNormal;
        private List<Cup_last> _cupsLast;
        private List<Component> _drops;
        private Texture2D _dropTex;

        private int _updateCounter;
        private static readonly int _dropsStrength = 2;             // the higher number - the more drops are generated (irruptive); has to be > 0, otherwise no drops will be generated

        private Random random;

        Pipe _testPipe;                                             //TO BE REPLACED WITH A LIST!
        Pipe _testPipe2;                                            //TO BE REPLACED WITH A LIST!

        #endregion



        public Level(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, int difficulty)
        {
            _game = game;
            _content = content;
            _graphicsDevice = graphicsDevice;
            random = new Random();
            _hasChosen = false;

            _updateCounter = 0;


            _cupsNormal = new List<Cup>();
            _cupsLast = new List<Cup_last>();
            _dropTex = _content.Load<Texture2D>("World/drop_tx");
            _drops = new List<Component>();
              

            _physxWorld = new World(new Vector2(0, 20));                //new physics connected world (strong gravity 16g)
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);             //64 pixels = 1 meter when it comes to simulation


            //add cups - different difficulty level - different number of cups
            switch (difficulty)
            {
                case 1:
                    _cupsNormal.Add(new Cup(_physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f)));
                    _cupsLast.Add(new Cup_last(_physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.8f), (int)(graphicsDevice.Viewport.Height / 1.95f)));
                    _cupsLast.Add(new Cup_last(_physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f)));
                    _testPipe = new PipeAB_1(_physxWorld, graphicsDevice, _cupsNormal[0]._pipeA_X, _cupsNormal[0]._pipeA_Y, _cupsLast[0]._posX, -1);         //TO BE DELETED
                    _testPipe2 = new PipeAB_1(_physxWorld, graphicsDevice, _cupsNormal[0]._pipeB_X, _cupsNormal[0]._pipeB_Y, _cupsLast[1]._posX, 0);         //TO BE DELETED
                    break;

                default:    break;
            }

            

        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var cup in _cupsNormal)
                cup.Draw(gameTime, spriteBatch);

            foreach (var cup in _cupsLast)
                cup.Draw(gameTime, spriteBatch);

            _testPipe.Draw(gameTime, spriteBatch);                       //TO BE DELETED
            _testPipe2.Draw(gameTime, spriteBatch);                       //TO BE DELETED



            #region temp1
            foreach (var drop in _drops)
            {
               drop.Draw(gameTime, spriteBatch);
            }

            #endregion

        }

        public void Update(GameTime gameTime)
        {
            #region temp2
            if (_updateCounter < _dropsStrength)
            {
                _drops.Add(new Drop(_dropTex, _graphicsDevice, _physxWorld, random.Next(-10, 10)));
                _updateCounter++;
            }
            else
                _updateCounter = 0;
            #endregion


            _physxWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);



            #region temp3

            for (int i = _drops.Count - 1; i >= 0; i--)
            {
                if (_drops[i].CheckPos())
                    _drops.RemoveAt(i);
            
            }
            #endregion

        }
    }
}
