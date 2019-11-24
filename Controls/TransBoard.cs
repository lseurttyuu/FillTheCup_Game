using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FillTheCup.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup.Controls
{
    public class TransBoard : Component
    {

        private ContentManager _content;
        private SpriteFont _font;
        private Texture2D _layerTrans;
        private Texture2D _layerSolid;
        private Vector2 _laSoSize;
        private Vector2 _laSoOrigin;



        public TransBoard(GameState gameState, GraphicsDevice graphicsDevice, ContentManager content, int score, bool hasWon)
        {
            _content = content;

            _font = _content.Load<SpriteFont>("Fonts/Menu");                                    //to be modified --> another font size, color, etc

            _layerTrans = new Texture2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            _laSoSize = new Vector2((int)(graphicsDevice.Viewport.Width * 0.7578125f), (int)(graphicsDevice.Viewport.Height * 0.5859375f));
            _laSoOrigin = new Vector2((graphicsDevice.Viewport.Width - _laSoSize.X) / 2, (graphicsDevice.Viewport.Height - _laSoSize.Y) / 2);
            _layerSolid = new Texture2D(graphicsDevice, (int)_laSoSize.X, (int)_laSoSize.Y);

            

            

        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_layerTrans, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(_layerSolid, _laSoOrigin, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
