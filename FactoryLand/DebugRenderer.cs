using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    static class DebugRenderer
    {
        private static SpriteBatch spriteBatch;
        private static GraphicsDevice graphicsDevice;
        private static Texture2D canvas;
        private static bool draw;

        private static UInt32[] pixels;
        private static Rectangle size;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            DebugRenderer.graphicsDevice = graphicsDevice;
            DebugRenderer.spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public static void AddPixels(UInt32[] pixels, Rectangle size)
        {
            draw = true;
            canvas = new Texture2D(graphicsDevice, size.Width, size.Height);
            DebugRenderer.pixels = pixels;
            DebugRenderer.size = size;
        }

        public static void Draw()
        {
            if (draw)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                graphicsDevice.Textures[0] = null;
                canvas.SetData<UInt32>(pixels, 0, pixels.Length);
                spriteBatch.Draw(canvas, size, Color.White);
                spriteBatch.End();
                //draw = false;
            }
        }
    }
}
