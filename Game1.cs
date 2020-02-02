﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleSystem.Helpers;
using ParticleSystem.Emitters;
using ParticleSystem.Providers.Color;
using ParticleSystem.Providers.Numeric;

namespace ParticleSystem
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private IEmitter _emitter;
        private IEmitter _emitter2;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
         }

        protected override void Initialize()
        {
            var colorProvider = new GradientProvider();
            colorProvider.AddValuePoint(0f, Color.Blue);
            colorProvider.AddValuePoint(0.5f, Color.Orange);
            colorProvider.AddValuePoint(1f, Color.Red);

            var scaleProvider = new FloatProvider();
            scaleProvider.AddValuePoint(0f, 0f);
            scaleProvider.AddValuePoint(0.25f, 4f);
            scaleProvider.AddValuePoint(0.5f, 1f);
            scaleProvider.AddValuePoint(0.75f, 5f);
            scaleProvider.AddValuePoint(1f, 0f);

            var opacityProvider = new FloatProvider();
            opacityProvider.AddValuePoint(0f, 0f);
            opacityProvider.AddValuePoint(0.2f, 0.5f);
            opacityProvider.AddValuePoint(0.5f, 1f);
            opacityProvider.AddValuePoint(1f, 0f);

            var colorProvider2 = new GradientProvider();
            colorProvider2.AddValuePoint(0f, Color.White);
            colorProvider2.AddValuePoint(1f, Color.Blue);

            var scaleProvider2 = new FloatProvider();
            scaleProvider2.AddValuePoint(0f, 1);
            scaleProvider2.AddValuePoint(1f, 0);

            var opacityProvider2 = new FloatProvider();
            opacityProvider2.AddValuePoint(0f, 1);
            opacityProvider2.AddValuePoint(1f, 1);

            //_emitter = new PointEmitter2D(colorProvider, scaleProvider, opacityProvider);

            //_emitter.Location = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2,
            //    _graphics.GraphicsDevice.Viewport.Height / 2);

            _emitter = new RectangleEmitter2D(_graphics.GraphicsDevice.Viewport.Width/2,
                _graphics.GraphicsDevice.Viewport.Height, colorProvider, scaleProvider,
                opacityProvider);

            _emitter2 = new RectangleEmitter2D(_graphics.GraphicsDevice.Viewport.Width/2,
                _graphics.GraphicsDevice.Viewport.Height, colorProvider2, scaleProvider2,
                opacityProvider2);

            _emitter.Location = new Vector2(0, 0);
            _emitter2.Location = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2, 0);
            _emitter2.ParticleMaxSpeed = 0;
            _emitter2.Lifetime = 240;
            _emitter2.ParticleMaximumLife = 30;
            _emitter2.StartDelay = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _emitter.Texture = Content.Load<Texture2D>("ParticleSolid");
            _emitter2.Texture = Content.Load<Texture2D>("ParticleSolid");
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardHelper.UpdateState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (KeyboardHelper.IsPressed(Keys.Space))
            {
                _emitter.Trigger();
                _emitter2.Trigger();
            }

            if (KeyboardHelper.IsPressed(Keys.S))
            {
                _emitter.Stop();
                _emitter2.Stop();
            }

            /*_emitter.Location = new Vector2(Mouse.GetState().Position.X,
                Mouse.GetState().Position.Y);

            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                _emitter.Trigger();*/

            _emitter.Update();
            _emitter2.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            _emitter.Render(_spriteBatch);
            _emitter2.Render(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
