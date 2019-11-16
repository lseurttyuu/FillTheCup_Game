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
    class Cup : Component
    {


        #region cupdef
        //All numbers below should be even!
        private static readonly int _cup_X = 104;
        private static readonly int _cup_Y = 128;               //static dimensions of a cup

        private static readonly int _minDistanceBottom = 8;
        private static readonly int _maxDistanceTop = 20;       //static restrictions of joining point height related to a cup (vertical restrictions)

        private static readonly int _pipeWidth = 40;            //outer dimensions!
        private static readonly int _lineWidth = 4;
        private static readonly int _partsNumber = 5;


        public int _pipeA_X;
        public int _pipeA_Y;                                   //coordinates to connect pipes (A - left pipe, B - right pipe)
        public int _pipeB_X;
        public int _pipeB_Y;

        private List<Texture2D> _cupSprites;                   //every cup has 5 parts: bottom, 2 left, 2 right;
        private List<Color[]> _colorSprites;
        private List<Body> _cupParts;                          //physics connected parts of the cup;
        private List<Vector2> _cupOrigins;
        private List<Vector2> _partsPositions;

        private Random random;

        #endregion

        public Cup(World world, GraphicsDevice graphicsDevice, int posX, int posY)
        {
            random = new Random();

            _cupParts = new List<Body>();
            _cupSprites = new List<Texture2D>();
            _colorSprites = new List<Color[]>();
            _cupOrigins = new List<Vector2>();
            _partsPositions = new List<Vector2>();

            _cupSprites.Add(new Texture2D(graphicsDevice, _cup_X, _lineWidth));                                         //bottom line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(posX, posY + (_cup_Y - _lineWidth) / 2)));          //bottom line
            _colorSprites.Add(new Color[_cup_X * _lineWidth]);                                                          //bottom line



            int pipeA_relative_Y = random.Next((_maxDistanceTop + _pipeWidth / 2)/2, (_cup_Y - _minDistanceBottom - _pipeWidth / 2)/2);
            int pipeB_relative_Y = random.Next((_maxDistanceTop + _pipeWidth / 2)/2, (_cup_Y - _minDistanceBottom - _pipeWidth / 2)/2);
            pipeA_relative_Y *= 2;
            pipeB_relative_Y *= 2;


            _cupSprites.Add(new Texture2D(graphicsDevice, _lineWidth, pipeA_relative_Y-_pipeWidth/2+_lineWidth));                                                                       //top left line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(posX-(_cup_X-_lineWidth)/2, posY - _cup_Y / 2 + (pipeA_relative_Y - _pipeWidth / 2 + _lineWidth) / 2)));            //top left line
            _colorSprites.Add(new Color[_lineWidth * (pipeA_relative_Y - _pipeWidth / 2 + _lineWidth)]);                                                                                //top left line

            _cupSprites.Add(new Texture2D(graphicsDevice, _lineWidth, _cup_Y-pipeA_relative_Y-_pipeWidth/2+_lineWidth));                                                                //bottom left line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(posX - (_cup_X - _lineWidth) / 2,posY+_cup_Y/2-(_cup_Y - pipeA_relative_Y - _pipeWidth / 2 + _lineWidth)/2)));      //bottom left line
            _colorSprites.Add(new Color[_lineWidth * (_cup_Y - pipeA_relative_Y - _pipeWidth / 2 + _lineWidth)]);                                                                       //bottom left line

            _cupSprites.Add(new Texture2D(graphicsDevice, _lineWidth, pipeB_relative_Y - _pipeWidth / 2 + _lineWidth));                                                                 //top right line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(posX + (_cup_X - _lineWidth) / 2, posY - _cup_Y / 2 + (pipeB_relative_Y - _pipeWidth / 2 + _lineWidth) / 2)));      //top right line
            _colorSprites.Add(new Color[_lineWidth * (pipeB_relative_Y - _pipeWidth / 2 + _lineWidth)]);                                                                                //top right line

            _cupSprites.Add(new Texture2D(graphicsDevice, _lineWidth, _cup_Y - pipeB_relative_Y - _pipeWidth / 2 + _lineWidth));                                                        //bottom right line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(posX + (_cup_X - _lineWidth) / 2, posY+_cup_Y/2-(_cup_Y - pipeB_relative_Y - _pipeWidth / 2 + _lineWidth) /2)));    //bottom right line
            _colorSprites.Add(new Color[_lineWidth * (_cup_Y - pipeB_relative_Y - _pipeWidth / 2 + _lineWidth)]);                                                                       //bottom right line


            for(int j=0; j<_partsNumber; j++)
            {
                for (int i = 0; i < _colorSprites[j].Length; ++i)
                    _colorSprites[j][i] = Color.FromNonPremultiplied(0x46, 0x46, 0x46, 255);

                _cupSprites[j].SetData(_colorSprites[j]);
                _cupOrigins.Add(new Vector2(_cupSprites[j].Width / 2, _cupSprites[j].Height / 2));
            }
            


            for (int i=0; i<_partsNumber; i++)
            {
                _cupParts.Add(null);
                _cupParts[i] = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(_cupSprites[i].Width), ConvertUnits.ToSimUnits(_cupSprites[i].Height), 1f, _partsPositions[i]);
                _cupParts[i].BodyType = BodyType.Static;
                _cupParts[i].Restitution = 0.5f;
                _cupParts[i].Friction = 0.5f;
            }


        }



        public override bool CheckPos()
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for(int i=0; i<_partsNumber; i++)
            {
                spriteBatch.Draw(_cupSprites[i], ConvertUnits.ToDisplayUnits(_cupParts[i].Position), null, Color.White, 0f, _cupOrigins[i], 1f, SpriteEffects.None, 0f);
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
