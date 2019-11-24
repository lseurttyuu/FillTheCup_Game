using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VelcroPhysics.Dynamics;

namespace FillTheCup.World_elems
{
    public abstract class Pipe : Component
    {

        public abstract override void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract override void Update(GameTime gameTime);
    }
}
