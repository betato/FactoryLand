using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FactoryLand
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game, IInputReciever
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputManager = new InputManager();

        private Camera camera;
        private Terrain terrain = new Terrain();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            TextureManager.Initialize(Content);

            inputManager.AddInputReciever(this, InputType.CameraUp, InputState.Down);
            inputManager.AddInputReciever(this, InputType.CameraDown, InputState.Down);
            inputManager.AddInputReciever(this, InputType.CameraLeft, InputState.Down);
            inputManager.AddInputReciever(this, InputType.CameraRight, InputState.Down);
            inputManager.AddInputReciever(this, InputType.CameraZoom, InputState.Down);

            inputManager.AddKeyBinding(Keys.W, InputType.CameraUp);
            inputManager.AddKeyBinding(Keys.S, InputType.CameraDown);
            inputManager.AddKeyBinding(Keys.A, InputType.CameraLeft);
            inputManager.AddKeyBinding(Keys.D, InputType.CameraRight);
            inputManager.AddMouseBinding(MouseAction.Scroll, InputType.CameraZoom);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera(GraphicsDevice.Viewport);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;

            camera.Zoom = 0.1F;
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            camera.Viewport = GraphicsDevice.Viewport;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputManager.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.GetTransform());

            terrain.Draw(spriteBatch, gameTime);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void RecieveInput(InputType input, InputState state, Point mousePos, int scrollDelta)
        {
            switch (input)
            {
                case InputType.CameraUp:
                    camera.Location += new Vector2(0, -10);
                    break;
                case InputType.CameraDown:
                    camera.Location += new Vector2(0, 10);
                    break;
                case InputType.CameraLeft:
                    camera.Location += new Vector2(-10, 0);
                    break;
                case InputType.CameraRight:
                    camera.Location += new Vector2(10, 0);
                    break;
                case InputType.CameraZoom:
                    camera.Zoom += scrollDelta / 24000f;
                    break;
            }
        }
    }
}
