using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FillTheCup
{
    public abstract class Component
    {
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);
        public abstract bool CheckPos();
    }
}
