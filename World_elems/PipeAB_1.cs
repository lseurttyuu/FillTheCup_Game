using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;

namespace FillTheCup.World_elems
{
    /// <summary>
    /// Klasa definiująca rury razem z ich blokadami. Dostępne są 2 rodzaje rur (po prawej i po lewej stronie) z różnymi blokadami.
    /// </summary>
    public class PipeAB_1 : Pipe
    {
        #region PipeDefinition

        private int _startX;
        private int _startY;
        private bool _canBlock;
        private bool _mustBlock;
        private bool _isBlockade;
        private int _endX;
        private int _pipeType;                                  //Type of pipe: -1 means A (left side), 0 means B (right side);

        private static Random random = new Random();

        private int _topLineWidth;
        private int _bottomLineWidth;

        static readonly int _lineWidth = Cup._lineWidth;
        static readonly int _pipeWidth = Cup._pipeWidth;

        private static readonly int _partsNumber = 4;           //pipe A and B have 4 parts;


        private List<Texture2D> _pipeSprites;                   
        private List<Color[]> _colorSprites;
        private List<Body> _pipeParts;                          //physics connected parts of the cup;
        private List<Vector2> _pipeOrigins;
        private List<Vector2> _partsPositions;

        private Texture2D _blockadeSprite;
        private Vector2 _blockadeOrigin;
        private Vector2 _blockadePosition;
        private Body _blockadeBody;

        #endregion


        #region Methods

        /// <summary>
        /// Konstruktor 1 rury w danym poziomie <c>Level</c>.
        /// </summary>
        /// <param name="world">Fizyczny świat VelcroPhysics utworzony w klasie <c>Level</c>.</param>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        /// <param name="startX">Współrzędna X początkowa rury (punkt zaczepienia).</param>
        /// <param name="startY">>Współrzędna Y początkowa rury (punkt zaczepienia).</param>
        /// <param name="endX">Współrzędna X końcowa rury (punkt zaczepienia). Definiowana przez współrzędną X niższego kubeczka.</param>
        /// <param name="pipeType">Rodzaj rury. -1 oznacza rurę z lewej strony, 0 oznacza rurę z prawej strony.</param>
        /// <param name="canBlock">Zmienna mówiąca czy w rurze może wystąpić blokada przepływu.</param>
        /// <param name="mustBlock">Zmienna mówiąca czy w rurze musi wystąpić blokada przepływu.</param>
        public PipeAB_1(World world, GraphicsDevice graphicsDevice, int startX, int startY, int endX, int pipeType, bool canBlock, bool mustBlock)
        {
            _startX = startX;
            _startY = startY;
            _endX = endX;
            _pipeType = pipeType;
            _canBlock = canBlock;
            _mustBlock = mustBlock;
            //types of blockades: only 1 can occur (vertical/horizontal) in one pipe; there are 2 random decisions: 1. is blockade present (IS when mustBlock == true), 2. is vertical or horizontal;

            _pipeParts = new List<Body>();
            _pipeSprites = new List<Texture2D>();
            _colorSprites = new List<Color[]>();
            _pipeOrigins = new List<Vector2>();
            _partsPositions = new List<Vector2>();


            if (_pipeType == -1)
            {
                _topLineWidth = _startX - _endX + _lineWidth / 2 + _pipeWidth / 2;
                _bottomLineWidth = _startX - _endX + (3 * _lineWidth) / 2 - _pipeWidth / 2;
                ConstructPipeA(graphicsDevice);
            }
            else
            {
                _topLineWidth = _endX - _startX + _lineWidth / 2 + _pipeWidth / 2;
                _bottomLineWidth = _endX - _startX + (3 * _lineWidth) / 2 - _pipeWidth / 2;
                ConstructPipeB(graphicsDevice);
            }

            
            for (int j = 0; j < _partsNumber; j++)
            {
                for (int i = 0; i < _colorSprites[j].Length; ++i)
                    _colorSprites[j][i] = Color.FromNonPremultiplied(0x46, 0x46, 0x46, 255);

                _pipeSprites[j].SetData(_colorSprites[j]);
                _pipeOrigins.Add(new Vector2(_pipeSprites[j].Width / 2, _pipeSprites[j].Height / 2));
            }


            for (int i = 0; i < _partsNumber; i++)
            {
                _pipeParts.Add(null);
                _pipeParts[i] = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(_pipeSprites[i].Width), ConvertUnits.ToSimUnits(_pipeSprites[i].Height), 1f, _partsPositions[i]);
                _pipeParts[i].BodyType = BodyType.Static;
                _pipeParts[i].Restitution = 0.5f;
                _pipeParts[i].Friction = 0.5f;
            }

            #region Blockade

            _isBlockade = Convert.ToBoolean(random.Next(0, 2));
            if (_mustBlock)
                _isBlockade = true;
            if (!_canBlock)
                _isBlockade = false;

            if (_isBlockade)
            {
                short blockadeType = (short)random.Next(0, 2);          //0 --> horizontal, 1 --> vertical;
                if (Convert.ToBoolean(blockadeType))                    //vertical blockade
                {
                    _blockadeOrigin = new Vector2(_lineWidth / 2, _pipeWidth / 2);
                    _blockadePosition = ConvertUnits.ToSimUnits(new Vector2(_startX, _startY));
                    _blockadeSprite = new Texture2D(graphicsDevice, _lineWidth, _pipeWidth);
                }
                else
                {
                    _blockadeOrigin = new Vector2(_pipeWidth / 2, _lineWidth / 2);
                    _blockadePosition = ConvertUnits.ToSimUnits(new Vector2(_endX, _startY + _pipeWidth / 2 + (int)(graphicsDevice.Viewport.Height / 64) - _lineWidth));
                    _blockadeSprite = new Texture2D(graphicsDevice, _pipeWidth, _lineWidth);
                }

                Color[] blockadeCol = new Color[_lineWidth * _pipeWidth];
                for (int i = 0; i < blockadeCol.Length; ++i)
                    blockadeCol[i] = Color.FromNonPremultiplied(0x46, 0x46, 0x46, 255);

                _blockadeSprite.SetData(blockadeCol);

                _blockadeBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(_blockadeSprite.Width), ConvertUnits.ToSimUnits(_blockadeSprite.Height), 1f, _blockadePosition);
                _blockadeBody.BodyType = BodyType.Static;
                _blockadeBody.Restitution = 0.5f;
                _blockadeBody.Friction = 0.5f;
            }

            #endregion

        }

        /// <summary>
        /// Rysowanie rur wraz z blokadami.
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        /// <param name="spriteBatch">Poborca wszystkich elementów graficznych, zainicjalizowany w klasie wyżej.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _partsNumber; i++)
                spriteBatch.Draw(_pipeSprites[i], ConvertUnits.ToDisplayUnits(_pipeParts[i].Position), null, Color.White, 0f, _pipeOrigins[i], 1f, SpriteEffects.None, 0f);

            if (_isBlockade)
                spriteBatch.Draw(_blockadeSprite, ConvertUnits.ToDisplayUnits(_blockadeBody.Position), null, Color.White, 0f, _blockadeOrigin, 1f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Aktualizacja; Nieużywana w tej klasie!
        /// </summary>
        /// <param name="gameTime">Czas gry.</param>
        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Funkcja odpowiedzialna za stworzenie i wyliczenie pozycji rury po lewej stronie.
        /// </summary>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        private void ConstructPipeA(GraphicsDevice graphicsDevice)
        {
            _pipeSprites.Add(new Texture2D(graphicsDevice, _topLineWidth, _lineWidth));                                                                                 //top line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX + _lineWidth / 2 - (float)Math.Ceiling((double)_topLineWidth / 2), _startY - (_pipeWidth - _lineWidth) / 2)));   //top line
            _colorSprites.Add(new Color[_topLineWidth * _lineWidth]);                                                                                                   //top line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _bottomLineWidth, _lineWidth));                                                                          //bottom line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX + _lineWidth / 2 - (float)Math.Ceiling((double)_bottomLineWidth / 2), _startY + (_pipeWidth - _lineWidth) / 2)));     //bottom line
            _colorSprites.Add(new Color[_bottomLineWidth * _lineWidth]);                                                                                            //bottom line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 64 + _pipeWidth - _lineWidth)));                                               //vertical left line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX - (_pipeWidth - _lineWidth) / 2, _startY - _pipeWidth / 2 + (int)(graphicsDevice.Viewport.Height / 64 + _pipeWidth - _lineWidth) / 2)));   //vertical left line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 64 + _pipeWidth - _lineWidth)]);                                                                 //vertical left line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 64)));                                                                       //vertical right line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX + (_pipeWidth - _lineWidth) / 2, _startY - _lineWidth + (int)((_pipeWidth + graphicsDevice.Viewport.Height / 64)/2))));   //vertical right line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 64)]);                                                                                         //vertical right line
        }

        /// <summary>
        /// Funkcja odpowiedzialna za stworzenie i wyliczenie pozycji rury po prawej stronie.
        /// </summary>
        /// <param name="graphicsDevice">Obecnie używane pole graficzne (klasa GraphicsDevice)</param>
        private void ConstructPipeB(GraphicsDevice graphicsDevice)
        {
            _pipeSprites.Add(new Texture2D(graphicsDevice, _topLineWidth, _lineWidth));                                                                                 //top line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX - _lineWidth / 2 + (float)Math.Floor((double)_topLineWidth / 2), _startY - (_pipeWidth - _lineWidth) / 2)));  //top line
            _colorSprites.Add(new Color[_topLineWidth * _lineWidth]);                                                                                                   //top line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _bottomLineWidth, _lineWidth));                                                                           //bottom line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX - _lineWidth / 2 + (float)Math.Floor((double)_bottomLineWidth / 2), _startY + (_pipeWidth - _lineWidth) / 2)));    //bottom line
            _colorSprites.Add(new Color[_bottomLineWidth * _lineWidth]);                                                                                             //bottom line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 64) + _pipeWidth - _lineWidth));                                               //vertical left line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX + (_pipeWidth - _lineWidth) / 2, _startY - _pipeWidth / 2 + (int)((graphicsDevice.Viewport.Height / 64 + _pipeWidth - _lineWidth) / 2))));   //vertical left line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 64 + _pipeWidth - _lineWidth)]);                                                                 //vertical left line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 64)));                                                                       //vertical right line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX - (_pipeWidth - _lineWidth) / 2, _startY + -_lineWidth + (int)((_pipeWidth + graphicsDevice.Viewport.Height/64)/2))));   //vertical right line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 64)]);                                                                                         //vertical right line
        }

        #endregion
    }
}
