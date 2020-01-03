using System;
using System.Collections.Generic;
using FillTheCup.Controls;
using FillTheCup.World_elems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;

namespace FillTheCup.States
{
    /// <summary>
    /// Klasa odpowiedzialna za zarządzanie danym poziomem i wydarzeniami z nim związanym
    /// (jest to znaczenie szersze od znaczenia "poziomu" zawartego w klasie <c>Level</c>).
    /// Klasa istnieje od początku do końca danego poziomu, wliczając w to akcje takie jak
    /// odliczanie czasu do początku poziomu, utworzenie ekranu po werdykcie, itp.
    /// </summary>
    public class GameState : State
    {
        #region Fields

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
        private List<Song> _music;
        private static Random random = new Random();

        /// <summary>
        /// Całkowity wynik podczas danej rozgrywki.
        /// </summary>
        public static int _totalScore = 0;
        private int _score = 0;

        /// <summary>
        /// Pole opisujące obecny stan poziomu (w szerokim znaczeniu słowa "poziom").
        /// </summary>
        public int _innerState = 0;                                //variable resposible for states: begining (counter), choice+animation, lvl end screen
        /// <summary>
        /// Pole opisujące stan poprzedni danego poziomu.
        /// </summary>
        public int _previousState = 0;
        /// <summary>
        /// Pole opisujące czas (na początku wynosi 0, rośnie do 4 --> czas na oczekiwanie przed
        /// inicjalizacją obiektu klasy Level; Następnie pole <c>_timeElapsed</c> jest zerowane,
        /// co pozwala kontrolować czas który upłynął od pojawienia się kubeczków na potrzeby
        /// licznika czasu, a także systemu obliczania wyniku za dany poziom).
        /// </summary>
        public float _timeElapsed = 0f;                             //gametime to measure start screen 3.. 2.. 1..
        /// <summary>
        /// Pole opisujące po jakim czasie została podjęta decyzja (wybór kubeczka).
        /// </summary>
        private float _timeStamp = 0f;
        /// <summary>
        /// Pole opisujące numer rozgrywanego poziomu (nowa gra zawsze zaczyna się od poziomu 1).
        /// </summary>
        public static int _lvlDifficulty = 1;                         //single level difficulty (number of cups)
        /// <summary>
        /// Pole opisujące poziom trudności wszystkich poziomów (ustawialne z poziomu ustawień gry).
        /// Ma wpływ na maksymalny czas dostępny dla użytkownika na rozwiązanie problemu.
        /// Dostępne wartości: 1 - poziom łatwy, 2 - poziom średni, 3 - poziom trudny.
        /// </summary>
        public static int _globalLvlDifficulty = 1;                     //1 - easy, 2 - medium, 3 - hard (time restriction)
        /// <summary>
        /// Pole opisujące maksymalny czas na decyzję użytkownika (w sekundach). Jest zmieniany przez konstruktor klasy Level dla poziomów >= 10.
        /// </summary>
        public static float _maxPlayerTime;
        private readonly float _maxTimeEasyDef = 10;
        private readonly float _maxTimeMediumDef = 5;
        private readonly float _maxTimeHardDef = 3;

        #endregion

        #region Methods

        /// <summary>
        /// Konstruktor klasy <c>GameState</c>. Jest zgodny z klasą abstrakcyjną State.
        /// </summary>
        /// <param name="game">Instancja gry (klasa Game1)</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        /// <param name="content">Obecny Content Manager od Monogame (klasa ContentManager)</param>
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = _content.Load<Texture2D>("World/background");
            _background_grass = _content.Load<Texture2D>("World/background_grass");
            _fontCounter = _content.Load<SpriteFont>("Fonts/Counter");
            _lvlFont = _content.Load<SpriteFont>("Fonts/Menu");


            List<int> randomNumbersFlowers = new List<int>();
            _flowers = new List<Component>();

            for(short i=0; i<9; i++)
                _flowers.Add(new Flower(randomNumbersFlowers, _content.Load<Texture2D>("World/flower_" + (i + 1)), _graphicsDevice));

            if (_lvlDifficulty == 1)
                if (_globalLvlDifficulty == 1)
                    _maxPlayerTime = _maxTimeEasyDef;
                else if (_globalLvlDifficulty == 2)
                    _maxPlayerTime = _maxTimeMediumDef;
                else
                    _maxPlayerTime = _maxTimeHardDef;

            #region Music
            _music = new List<Song>();

            for (short i = 0; i < 5; i++)
                _music.Add(_content.Load<Song>("audio/music/" + (i + 1)));

            if (_game._musicPlaying < 1)
            {
                _game._musicPlaying = (short)(random.Next(_music.Count) + 1);
                MediaPlayer.Stop();
                MediaPlayer.Play(_music[_game._musicPlaying - 1]);
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = 1.0f;
            }
            #endregion

        }

        /// <summary>
        /// Metoda odpowiedzialna za rysowanie wszystkich elementów związanych z danym poziomem.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
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
                _transBoard.Draw(gameTime, spriteBatch);
           
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
        /// Aktualizowanie całej logiki danego poziomu.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {

            if(MediaPlayer.State != MediaState.Playing)
            {
                _game._musicPlaying = (short)(random.Next(_music.Count)+1);
                MediaPlayer.Stop();
                MediaPlayer.Play(_music[_game._musicPlaying-1]);
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = 1.0f;
            }


            #region StatesHandling

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
                
                _transBoard = new TransBoard(_game, _level, _graphicsDevice, _content, _score, _totalScore, _level._hasWon);
                _innerState++;

            }

            if(_innerState == 4)
            {
                _transBoard.Update(gameTime);
            }

            #endregion


            #region KeyboardEvents

            _previousKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();

            if (_previousKeyboard != _currentKeyboard && _currentKeyboard.IsKeyDown(Keys.Escape))
                if (_innerState != -1)
                {
                    if(_level != null)
                    {
                        _level._waterStart.Stop();
                        _level._waterLoop.Stop();
                    }
                    
                    _previousState = _innerState;
                    _innerState = -1;
                    _pauseBoard = new PauseBoard(_game, this, _level, _graphicsDevice, _content);
                }
                else
                {
                    if (_level != null && _level._hasChosen)
                    {
                        _level._soundFxState = 2;
                        _level._waterLoop.Play();
                    }

                    if (_previousState == 1)
                        _timeElapsed += (float)((_maxPlayerTime - _timeElapsed) * 0.3);

                _innerState = _previousState;
                }

            #endregion

        }
        #endregion
    }
}
