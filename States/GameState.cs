using System;
using System.Collections.Generic;
using FillTheCup.World_elems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;

namespace FillTheCup.States
{
    public class GameState : State
    {

        private List<Component> _components;                    //not used now!
        private List<Component> _flowers;
        private Texture2D _background;                                


        #region phsx
        private readonly World _world;
        private Body _groundBody;
        private Texture2D _dropTex;
        private Texture2D _cupSprite;
        private Vector2 _mainCupOrigin;
        private Random random;
        private List<Component> _drops;



        //TEMP SECTION

        private Body _groundBody2;
        private Texture2D _cupSprite2;
        private Vector2 _mainCupOrigin2;
        private Body _groundBody3;
        private Texture2D _cupSprite3;
        private Vector2 _mainCupOrigin3;




        #endregion

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = _content.Load<Texture2D>("World/background");
            
            _flowers = new List<Component>();

            for(int i=0; i<9; i++)
                _flowers.Add(new Flower(_content.Load<Texture2D>("World/flower_" + (i + 1)), graphicsDevice.Viewport.Width));


            #region TestPhysics

            random = new Random();

            _dropTex = _content.Load<Texture2D>("World/drop_tx");
            _drops = new List<Component>();
            

            _world = new World(new Vector2(0, 16));



            



            _cupSprite = new Texture2D(graphicsDevice, 200, 10);

            Color[] data = new Color[200 * 10];
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Chocolate;
            _cupSprite.SetData(data);

            _mainCupOrigin = new Vector2(_cupSprite.Width / 2, _cupSprite.Height / 2);


            _cupSprite2 = new Texture2D(graphicsDevice, 10, 300);

            Color[] data2 = new Color[300 * 10];
            for (int i = 0; i < data2.Length; ++i)
                data2[i] = Color.Chocolate;
            _cupSprite2.SetData(data2);

            _mainCupOrigin2 = new Vector2(_cupSprite2.Width / 2, _cupSprite2.Height / 2);


            _cupSprite3 = new Texture2D(graphicsDevice, 10, 300);
            _cupSprite3.SetData(data2);



            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);     //64 pixels = 1 meter when it comes to simulation






            Vector2 groundPosition = ConvertUnits.ToSimUnits(new Vector2(graphicsDevice.Viewport.Width / 2f, graphicsDevice.Viewport.Height / 2f)) + new Vector2(0, 1.25f);
            Vector2 groundPosition2 = ConvertUnits.ToSimUnits(new Vector2(graphicsDevice.Viewport.Width / 2f - 100, graphicsDevice.Viewport.Height / 2f - 150)) + new Vector2(0, 1.25f);
            Vector2 groundPosition3 = ConvertUnits.ToSimUnits(new Vector2(graphicsDevice.Viewport.Width / 2f + 100, graphicsDevice.Viewport.Height / 2f - 150)) + new Vector2(0, 1.25f);

            _groundBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(200f), ConvertUnits.ToSimUnits(10f), 1f, groundPosition);
            _groundBody.BodyType = BodyType.Static;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;


            _groundBody2 = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(10f), ConvertUnits.ToSimUnits(300f), 1f, groundPosition2);
            _groundBody2.BodyType = BodyType.Static;
            _groundBody2.Restitution = 0.3f;
            _groundBody2.Friction = 0.5f;


            _groundBody3 = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(10f), ConvertUnits.ToSimUnits(300f), 1f, groundPosition3);
            _groundBody3.BodyType = BodyType.Static;
            _groundBody3.Restitution = 0.3f;
            _groundBody3.Friction = 0.5f;



            #endregion


            //TEMP!!!!





        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);

            foreach (var flower in _flowers)
            {
                flower.Draw(gameTime, spriteBatch);
            }




            #region physx
            spriteBatch.Draw(_cupSprite, ConvertUnits.ToDisplayUnits(_groundBody.Position), null, Color.White, 0f, _mainCupOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_cupSprite2, ConvertUnits.ToDisplayUnits(_groundBody2.Position), null, Color.White, 0f, _mainCupOrigin2, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_cupSprite3, ConvertUnits.ToDisplayUnits(_groundBody3.Position), null, Color.White, 0f, _mainCupOrigin2, 1f, SpriteEffects.None, 0f);

            foreach (var drop in _drops)
            {
                drop.Draw(gameTime, spriteBatch);
            }

            #endregion



            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {

            #region physxxx
            _drops.Add(new Drop(_dropTex,_graphicsDevice, _world, random.Next(-10,10)));
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);


            //foreach(Drop drop in _drops.ToArray())
            //{
            //    if(drop.CheckPos())
            //    {
             //       _drops.Remove(drop);
             //   }
            //}


            for (int i = _drops.Count - 1; i >= 0; i--)
            {
                if (_drops[i].CheckPos())
                    _drops.RemoveAt(i);

            }



            #endregion



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
            }

        }
    }
}
