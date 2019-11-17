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
    class Cup_last : Component
    {
        #region cupdef
        //All numbers below should be even!
        static readonly int _cup_X = Cup._cup_X;
        static readonly int _cup_Y = Cup._cup_Y;                //static dimensions of a cup

        public readonly int _posX;
        public readonly int _posY;

        static readonly int _lineWidth = Cup._lineWidth;
        private static readonly int _partsNumber = 3;


        private List<Texture2D> _cupSprites;                   //every cup has 5 parts: bottom, 2 left, 2 right;
        private List<Color[]> _colorSprites;
        private List<Body> _cupParts;                          //physics connected parts of the cup;
        private List<Vector2> _cupOrigins;
        private List<Vector2> _partsPositions;

        #endregion

        public Cup_last(World world, GraphicsDevice graphicsDevice, int posX, int posY)
        {
            _posX = posX;
            _posY = posY;
            _cupParts = new List<Body>();
            _cupSprites = new List<Texture2D>();
            _colorSprites = new List<Color[]>();
            _cupOrigins = new List<Vector2>();
            _partsPositions = new List<Vector2>();

            _cupSprites.Add(new Texture2D(graphicsDevice, _cup_X, _lineWidth));                                         //bottom line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_posX, _posY + (_cup_Y - _lineWidth) / 2)));        //bottom line
            _colorSprites.Add(new Color[_cup_X * _lineWidth]);                                                          //bottom line 

            _cupSprites.Add(new Texture2D(graphicsDevice, _lineWidth, _cup_Y));                                         //left line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_posX - (_cup_X - _lineWidth) / 2, _posY)));        //left line
            _colorSprites.Add(new Color[_lineWidth * _cup_Y]);                                                          //left line

            _cupSprites.Add(new Texture2D(graphicsDevice, _lineWidth, _cup_Y));                                         //right line
            _partsPositions.Add(ConvertUnits.ToSimUnits(new Vector2(_posX + (_cup_X - _lineWidth) / 2, _posY)));        //right line
            _colorSprites.Add(new Color[_lineWidth * _cup_Y]);                                                          //right line


            for (int j = 0; j < _partsNumber; j++)
            {
                for (int i = 0; i < _colorSprites[j].Length; ++i)
                    _colorSprites[j][i] = Color.FromNonPremultiplied(0x46, 0x46, 0x46, 255);

                _cupSprites[j].SetData(_colorSprites[j]);
                _cupOrigins.Add(new Vector2(_cupSprites[j].Width / 2, _cupSprites[j].Height / 2));
            }



            for (int i = 0; i < _partsNumber; i++)
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
            for (int i = 0; i < _partsNumber; i++)
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
