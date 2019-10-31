using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.World_elems
{
    class Flower : Component
    {

        private Texture2D _texture;
        private static Random random = new Random();

        public Vector2 Position { get; set; }


        public Flower(Texture2D texture, int windowSizeX)
        {
            _texture = texture;

            int posX = random.Next(windowSizeX - 2 * _texture.Width) + _texture.Width;          //random placing of flowers - a subject to change
            Position = new Vector2(posX, (int)(-0.1650390625* posX + 600));
        }



        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            //nothing to be done
        }

      

    }
}
