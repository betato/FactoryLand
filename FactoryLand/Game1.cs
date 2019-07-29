using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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
        private BasicEffect effect; // Kind of a shader
        private InputManager inputManager = new InputManager();
        private FramerateCounter fpsCounter = new FramerateCounter();

        private Camera camera;
        private Terrain terrain;
        private Selector selector;

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
            inputManager.AddInputReciever(this, InputType.Click, InputState.Pressed);

            inputManager.AddKeyBinding(Keys.W, InputType.CameraUp);
            inputManager.AddKeyBinding(Keys.S, InputType.CameraDown);
            inputManager.AddKeyBinding(Keys.A, InputType.CameraLeft);
            inputManager.AddKeyBinding(Keys.D, InputType.CameraRight);
            inputManager.AddMouseBinding(MouseAction.Scroll, InputType.CameraZoom);
            inputManager.AddMouseBinding(MouseAction.LeftButton, InputType.Click);


            base.Initialize();
        }

        private void UpdateProjection()
        {
            effect.Projection = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0.01f, 100000f);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            DebugRenderer.Initialize(GraphicsDevice, Content.Load<SpriteFont>("Default"));

            camera = new Camera();
            camera.Location = new Vector2(Chunk.SIZE * Tile.PIXEL_LENGTH * 0.5f, Chunk.SIZE * Tile.PIXEL_LENGTH * 0.5f);
            effect = new BasicEffect(GraphicsDevice);
            effect.World *= Matrix.CreateScale(Tile.PIXEL_LENGTH); // Scale tiles drawn at 1x1 to full pixel size
            UpdateProjection();

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;

            selector = new Selector();

            terrain = new Terrain();
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            UpdateProjection();
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

            selector.Update(ScreenToWorld(inputManager.MousePos.ToVector2()));
            terrain.UpdateChunkGraphics(GetScreenWorldBounds());

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            fpsCounter.Update(gameTime);
            GraphicsDevice.Clear(Color.Black);

            effect.View = camera.View;
            
            terrain.Draw(GraphicsDevice, effect);

            DrawEntities();

            DebugRenderer.AddText(
                "Zoom: " + camera.Zoom.ToString() + "\nLocation: " + camera.Location.ToString() + 
                "\nViewport Bounds: " + GraphicsDevice.Viewport.Bounds.ToString() + "\nWorld Bounds: " + 
                GetScreenWorldBounds().ToString(), "Camera/View Parameters");

            DebugRenderer.AddText(fpsCounter.AverageFps.ToString(), "FPS");

            DebugRenderer.Draw();
            base.Draw(gameTime);
        }

        private void DrawEntities()
        {
            effect.TextureEnabled = true;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            DrawEntity(selector);
            rasterizerState.Dispose();
        }

        private void DrawEntity(IDrawableEntity entity)
        {
            entity.GetVertexData(out VertexPositionColorTexture[] verticies, out short[] indicies, out Texture2D texture);
            effect.Texture = texture;
            
            IndexBuffer indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), indicies.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indicies);
            GraphicsDevice.Indices = indexBuffer;

            VertexBuffer vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorTexture), verticies.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorTexture>(verticies);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verticies.Length / 2);
            }

            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        // (x, y, z, w) = (Lower X, Lower Y, Higher X, Higher Y)
        public Vector4 GetScreenWorldBounds()
        {
            Rectangle screenBounds = GraphicsDevice.Viewport.Bounds;
            Vector2 topLeft = ScreenToWorld(screenBounds.Location.ToVector2());
            Vector2 bottomRight = ScreenToWorld((screenBounds.Size - screenBounds.Location).ToVector2());
            return new Vector4(topLeft.X, bottomRight.Y, bottomRight.X, topLeft.Y);
        }

        public Vector2 ScreenToWorld(Vector2 point)
        {
            Vector3 click = GraphicsDevice.Viewport.Unproject(new Vector3(point.X, point.Y, 0), effect.Projection, effect.View, effect.World);
            return new Vector2(click.X, click.Y);
        }

        public void RecieveInput(InputType input, InputState state, Point mousePos, int scrollDelta)
        {
            if (input == InputType.Click)
            {
                Vector2 click = ScreenToWorld(mousePos.ToVector2());
                DebugRenderer.AddText(String.Format("X:{0} Y:{1}", click.X, click.Y), "Last Click Location");
            }

            int movementSpeed = 10;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                movementSpeed = 100;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                movementSpeed = 1;
            }

            switch (input)
            {
                case InputType.CameraUp:
                    camera.Location += new Vector2(0, movementSpeed);
                    break;
                case InputType.CameraDown:
                    camera.Location += new Vector2(0, -movementSpeed);
                    break;
                case InputType.CameraLeft:
                    camera.Location += new Vector2(-movementSpeed, 0);
                    break;
                case InputType.CameraRight:
                    camera.Location += new Vector2(movementSpeed, 0);
                    break;
                case InputType.CameraZoom:
                    if (scrollDelta > 0 && camera.Zoom < 16)
                    {
                        camera.Zoom *= 2;
                    }
                    if (scrollDelta < 0 && camera.Zoom > 0.0625f)
                    {
                        camera.Zoom /= 2;
                    }
                    break;
            }
        }
    }
}
