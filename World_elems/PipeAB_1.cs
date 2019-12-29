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
//TO BE DONE: shorten pipes (vertically) to ensure compatibility!!!
namespace FillTheCup.World_elems
{
    class PipeAB_1 : Pipe
    {
        #region pipedef

        private int _startX;
        private int _startY;
        private int _endX;
        private int _pipeType;                                  //Type of pipe: -1 means A (left side), 0 means B (right side);

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

        #endregion


        public PipeAB_1(World world, GraphicsDevice graphicsDevice, int startX, int startY, int endX, int pipeType)
        {
            _startX = startX;
            _startY = startY;
            _endX = endX;
            _pipeType = pipeType;

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

        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _partsNumber; i++)
            {
                spriteBatch.Draw(_pipeSprites[i], ConvertUnits.ToDisplayUnits(_pipeParts[i].Position), null, Color.White, 0f, _pipeOrigins[i], 1f, SpriteEffects.None, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        private void ConstructPipeA(GraphicsDevice graphicsDevice)
        {
            _pipeSprites.Add(new Texture2D(graphicsDevice, _topLineWidth, _lineWidth));                                                                                 //top line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX + _lineWidth / 2 - _topLineWidth / 2, _startY - (_pipeWidth - _lineWidth) / 2)));           //top line
            _colorSprites.Add(new Color[_topLineWidth * _lineWidth]);                                                                                                   //top line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _bottomLineWidth, _lineWidth));                                                                           //bottom line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX + _lineWidth / 2 - _bottomLineWidth / 2, _startY + (_pipeWidth - _lineWidth) / 2)));     //bottom line
            _colorSprites.Add(new Color[_bottomLineWidth * _lineWidth]);

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 32 + _pipeWidth - _lineWidth)));                                                 //vertical left line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX - (_pipeWidth - _lineWidth) / 2, _startY - _pipeWidth / 2 + (int)(graphicsDevice.Viewport.Height / 32 + _pipeWidth - _lineWidth) / 2)));   //vertical left line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 32 + _pipeWidth - _lineWidth)]);                                                                 //vertical left line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 32)));                                                                       //vertical right line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX + (_pipeWidth - _lineWidth) / 2, _startY - _lineWidth + (int)((_pipeWidth + graphicsDevice.Viewport.Height / 32)/2))));   //vertical right line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 32)]);                                                                                         //vertical right line
        }


        private void ConstructPipeB(GraphicsDevice graphicsDevice)
        {
            _pipeSprites.Add(new Texture2D(graphicsDevice, _topLineWidth, _lineWidth));                                                                                 //top line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX - _lineWidth / 2 + _topLineWidth / 2, _startY - (_pipeWidth - _lineWidth) / 2)));           //top line
            _colorSprites.Add(new Color[_topLineWidth * _lineWidth]);                                                                                                   //top line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _bottomLineWidth, _lineWidth));                                                                           //bottom line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_startX - _lineWidth / 2 + _bottomLineWidth / 2, _startY + (_pipeWidth - _lineWidth) / 2)));     //bottom line
            _colorSprites.Add(new Color[_bottomLineWidth * _lineWidth]);

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 32) + _pipeWidth - _lineWidth));                                                 //vertical left line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX + (_pipeWidth - _lineWidth) / 2, _startY - _pipeWidth / 2 + (int)((graphicsDevice.Viewport.Height / 32 + _pipeWidth - _lineWidth) / 2))));   //vertical left line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 32 + _pipeWidth - _lineWidth)]);                                                                 //vertical left line

            _pipeSprites.Add(new Texture2D(graphicsDevice, _lineWidth, (int)(graphicsDevice.Viewport.Height / 32)));                                                                       //vertical right line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_endX - (_pipeWidth - _lineWidth) / 2, _startY + -_lineWidth + (int)((_pipeWidth + graphicsDevice.Viewport.Height/32)/2))));   //vertical right line
            _colorSprites.Add(new Color[_lineWidth * (int)(graphicsDevice.Viewport.Height / 32)]);                                                                                         //vertical right line
        }
    }
}
