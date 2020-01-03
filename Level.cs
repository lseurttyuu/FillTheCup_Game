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
using Microsoft.Xna.Framework.Audio;

namespace FillTheCup
{
    /// <summary>
    /// Klasa definiująca konkretny poziom (ułożenie kubeczków, istnienie świata w znaczeniu fizycznym - generowanie świata opartego na fizyce VelcroPhysics)
    /// </summary>
    public class Level
    {

        #region Fields

        /// <summary>
        /// Obiekt _content dla dostępu do zawartości ładowanej przez Monogame Content Pipeline
        /// </summary>
        protected ContentManager _content;
        /// <summary>
        /// Obiekt _game dla możliwości utworzenia nowego stanu (kolejny poziom/koniec gry - menu)
        /// </summary>
        protected Game1 _game;
        /// <summary>
        /// Obiekt _gameState dla możliwości zmiany parametrów poziomu w szerszym znaczeniu słowa (stan poza faktycznym "poziomem").
        /// </summary>
        protected GameState _gameState;
        /// <summary>
        /// Pole _graphicsDevice służące dla dostępu do właściwości i pól okna graficznego
        /// </summary>
        protected GraphicsDevice _graphicsDevice;

        /// <summary>
        /// Informacja czy użytkownik wybrał już jakiś kubeczek, czy też nie.
        /// </summary>
        public bool _hasChosen = false;                                    //for 2 states: choice (0) and animation (1)
        /// <summary>
        /// Informacja jaki numer kubeczka został wybrany.
        /// <example>Użytkownik wybrał kubeczek nr 2 - zmienna przyjmuje wartość 2.</example>
        /// </summary>
        public short _cupChosen = 0;                                         //to be filled with number of a cup that has been choosen
        private short _cupWon = 0;
        /// <summary>
        /// Informacja czy użytkownik wygrał rundę, czy też nie (w znaczeniu: czy przegrał).
        /// </summary>
        public bool _hasWon = false;                                       //information to know what trigger after particular level is finished
        /// <summary>
        /// Informacja czy nastąpił koniec poziomu (w znaczeniu: wraz z werdyktem uznaje się koniec poziomu w klasie "Level")
        /// </summary>
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
        private List<SoundEffect> _soundEffects;
        /// <summary>
        /// Pole opisujące stan efektu dźwiękowego (początek dźwięku spadania wody).
        /// </summary>
        public SoundEffectInstance _waterStart;
        /// <summary>
        /// Pole opisujące stan efektu dźwiękowego (powtarzający się dźwięk spania wody).
        /// </summary>
        public SoundEffectInstance _waterLoop;
        /// <summary>
        /// Pole opisujące stan efektów dźwiękowych dla danego poziomu (początek dźwięku spadania należy
        /// odwtorzyć dokładnie 1 raz, po skończeniu tego odwtorzenia należy zapoczątkować zapętlony
        /// dźwięk _waterLoop).
        /// </summary>
        public short _soundFxState = 0;

        private int _updateCounter;
        private static readonly int _dropsStrength = 3;             // the lower number - the more drops are generated (linearly) - from 1 to inf;

        private readonly Random random;

        #endregion

        #region Methods

        /// <summary>
        /// Konstruktor konkretnego poziomu klasy Level.
        /// </summary>
        /// <param name="gameState">Obecny stan gry (gameState) klasy GameState.</param>
        /// <param name="game">Instancja gry (klasa Game1)</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        /// <param name="content">Obecny Content Manager od Monogame (klasa ContentManager)</param>
        /// <param name="difficulty">Poziom który ma zostać utworzony (1, 2, 3, 4, 5, itd.)</param>
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
            _soundEffects = new List<SoundEffect>();
            _tap_open = _content.Load<Texture2D>("World/tap_open");
            _tap_closed = _content.Load<Texture2D>("World/tap_closed");
            _grassBar = _content.Load<Texture2D>("World/bckg_grass_bar");
            _soundEffects.Add(_content.Load<SoundEffect>("audio/fx/water_start"));
            _soundEffects.Add(_content.Load<SoundEffect>("audio/fx/water_loop"));
            _waterStart = _soundEffects[0].CreateInstance();
            _waterLoop = _soundEffects[1].CreateInstance();

            _waterStart.IsLooped = false;
            _waterStart.Volume = 0.3f;
            _waterLoop.IsLooped = true;
            _waterLoop.Volume = 0.3f;


            int barWidth = (int)(_graphicsDevice.Viewport.Width / 2.85);
            int barHeight = (int)(_graphicsDevice.Viewport.Height / 15.36);


            _bar = new Texture2D(_graphicsDevice, barWidth, barHeight);
            Color[] barColor = new Color[barWidth*barHeight];

            for(int i=0; i < barColor.Length; ++i)
               barColor[i] = Color.FromNonPremultiplied(0xFF, 0xFF, 0xDD, 255);

            _bar.SetData(barColor);

            _barPos = new Vector2((int)(_graphicsDevice.Viewport.Width / 2 - barWidth / 2), (int)(_graphicsDevice.Viewport.Height * 0.920572917));


            _physxWorld = new World(new Vector2(0, 25));                //new physics connected world (strong gravity 25g)
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);             //64 pixels = 1 meter when it comes to simulation

            #region LevelDefinitions

            switch (difficulty)
            {
                case 1:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.8f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 3));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, false, false));
                    break;
                case 2:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.8f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 3));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, true));
                    break;
                case 3:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.8f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 3));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    break;
                case 4:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.8f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 3));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 5.6f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 4));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeA_X, _cups[1]._pipeA_Y, _cups[3]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeB_X, _cups[1]._pipeB_Y, (int)(graphicsDevice.Viewport.Width / 2f), 0, true, true));
                    break;
                case 5:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 3.3f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 3));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 6f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 4));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.4f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 5));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeA_X, _cups[1]._pipeA_Y, _cups[3]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeB_X, _cups[1]._pipeB_Y, _cups[4]._posX, 0, false, false));
                    break;
                case 6:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 3.3f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.55f), (int)(graphicsDevice.Viewport.Height / 1.95f), 3, 3));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 6f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 4));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.4f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 5));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeA_X, _cups[1]._pipeA_Y, _cups[3]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeB_X, _cups[1]._pipeB_Y, _cups[4]._posX, 0, true, false));
                    break;
                case 7:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 3.3f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.4f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 3));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 6f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 4));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.4f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 5));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.75f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 6));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeA_X, _cups[1]._pipeA_Y, _cups[3]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeB_X, _cups[1]._pipeB_Y, _cups[4]._posX, 0, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeA_X, _cups[2]._pipeA_Y, _cups[5]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeB_X, _cups[2]._pipeB_Y, (int)(graphicsDevice.Viewport.Width / 1.2f), 0, true, true));
                    break;
                case 8:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 3.3f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.4f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 3));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 6f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 4));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.4f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 5));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.75f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 6));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeA_X, _cups[1]._pipeA_Y, _cups[3]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeB_X, _cups[1]._pipeB_Y, _cups[4]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeA_X, _cups[2]._pipeA_Y, _cups[5]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeB_X, _cups[2]._pipeB_Y, (int)(graphicsDevice.Viewport.Width / 1.2f), 0, true, true));
                    break;
                case 9:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 3.3f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.4f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 3));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 6f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 4));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.4f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 5));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.75f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 6));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.2f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 7));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeA_X, _cups[1]._pipeA_Y, _cups[3]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeB_X, _cups[1]._pipeB_Y, _cups[4]._posX, 0, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeA_X, _cups[2]._pipeA_Y, _cups[5]._posX, -1, false, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeB_X, _cups[2]._pipeB_Y, _cups[6]._posX, 0, false, false));
                    break;

                default:
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, graphicsDevice.Viewport.Width / 2, (int)(graphicsDevice.Viewport.Height / 2.9f), 5, 1));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 3.3f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 2));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.4f), (int)(graphicsDevice.Viewport.Height / 1.95f), 5, 3));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 6f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 4));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 2.4f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 5));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.75f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 6));
                    _cups.Add(new Cup(this, _physxWorld, graphicsDevice, (int)(graphicsDevice.Viewport.Width / 1.2f), (int)(graphicsDevice.Viewport.Height / 1.45f), 3, 7));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeA_X, _cups[0]._pipeA_Y, _cups[1]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[0]._pipeB_X, _cups[0]._pipeB_Y, _cups[2]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeA_X, _cups[1]._pipeA_Y, _cups[3]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[1]._pipeB_X, _cups[1]._pipeB_Y, _cups[4]._posX, 0, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeA_X, _cups[2]._pipeA_Y, _cups[5]._posX, -1, true, false));
                    _pipes.Add(new PipeAB_1(_physxWorld, graphicsDevice, _cups[2]._pipeB_X, _cups[2]._pipeB_Y, _cups[6]._posX, 0, true, false));
                    GameState._maxPlayerTime *= 0.95f;
                    break;
            }

            #endregion


        }


        /// <summary>
        /// Metoda odpowiedzialna za rysowanie kubeczków, kranu, paska z pozostałym czasem, rur i kropel wody.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_bar, _barPos, Color.White);
            spriteBatch.Draw(_grassBar, new Vector2(0, _graphicsDevice.Viewport.Height - _grassBar.Height), Color.White);
            _timer.Draw(gameTime, spriteBatch);

            foreach (var drop in _drops)
                drop.Draw(gameTime, spriteBatch);

            foreach (var pipe in _pipes)
                pipe.Draw(gameTime, spriteBatch);

            foreach (var cup in _cups)
                cup.Draw(gameTime, spriteBatch);
            
            
            if (_hasChosen)
                spriteBatch.Draw(_tap_open, new Vector2(_graphicsDevice.Viewport.Width - _tap_open.Width, 0), Color.White);
            else
                spriteBatch.Draw(_tap_closed, new Vector2(_graphicsDevice.Viewport.Width - _tap_closed.Width, 0), Color.White);

        }

        /// <summary>
        /// Metoda odpowiedzialna za werdykt wygranej/przegranej danego poziomu. Aktualizowanie świata fizycznego.
        /// Optymalizacja wykorzystania zasobów.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public void Update(GameTime gameTime)
        {
            if (_hasChosen)
            {
                if (_gameState._innerState == 1)
                    _gameState._innerState++;

                _updateCounter++;

                if (_updateCounter == _dropsStrength)
                {
                    _drops.Add(new Drop(_dropTex, _graphicsDevice, _physxWorld, random.Next(-15, 16)));                 //bigger drops variety (horizontal start position)
                    _updateCounter = 0;
                }


                if (_endLvl == false)
                    if (_cupWon == 0)
                    {
                        short currentCup = 1;
                        foreach (var cup in _cups)
                        {
                            short drops_inside = 0;

                            foreach (Drop drop in _drops)
                                if (ConvertUnits.ToDisplayUnits(drop._drop.Position).X > cup._posX - Cup._cup_X / 2 &&
                                    ConvertUnits.ToDisplayUnits(drop._drop.Position).X < cup._posX + Cup._cup_X / 2 &&
                                    ConvertUnits.ToDisplayUnits(drop._drop.Position).Y > cup._posY - Cup._cup_Y / 2 &&
                                    ConvertUnits.ToDisplayUnits(drop._drop.Position).Y < cup._posY + Cup._cup_Y / 2)
                                    drops_inside++;
                            
                            if (drops_inside > 130)
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
                            _hasWon = true;
                        else
                            _hasWon = false;

                    }
                
                if (_soundFxState == 0)
                {
                    _waterStart.Play();
                    _soundFxState++;
                }

                if (_soundFxState == 1 && _waterStart.State != SoundState.Playing)
                {
                    _waterLoop.Play();
                    _soundFxState++;
                }

            }
            else
                foreach (var cup in _cups)
                    cup.Update(gameTime);



            _physxWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);


            for (int i = _drops.Count - 1; i >= 0; i--)
                if (_drops[i].CheckPos())
                    _drops.RemoveAt(i);

        }

        /// <summary>
        /// Metoda odpowiedzialna za aktualizację pozycji paska z pozostałym czasem.
        /// </summary>
        /// <param name="timeElapsed">Czas który upłynął od początku poziomu (tj. odkąd pojawiły się kubeczki).</param>
        /// <param name="maxPlayerTime">Maksymalny czas na podjęcie decyzji.</param>
        public void UpdateBar(float timeElapsed, float maxPlayerTime)
        {
            _barPos.X = (int)(_graphicsDevice.Viewport.Width / 2 - _bar.Width / 2 - _bar.Width*timeElapsed/maxPlayerTime);
            _timer.UpdateTime(timeElapsed, maxPlayerTime);
        }

        #endregion
    }
}
