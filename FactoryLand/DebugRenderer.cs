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
        private static SpriteFont font;

        private static bool drawPixels;
        private static UInt32[] pixels;
        private static Rectangle size;
        private static bool drawText;
        private static Dictionary<string, string> strings = new Dictionary<string, string>();

        public static void Initialize(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            DebugRenderer.graphicsDevice = graphicsDevice;
            DebugRenderer.spriteBatch = new SpriteBatch(graphicsDevice);
            DebugRenderer.font = font;
        }

        public static void AddPixels(UInt32[] pixels, Rectangle size)
        {
            drawPixels = true;
            canvas = new Texture2D(graphicsDevice, size.Width, size.Height);
            DebugRenderer.pixels = pixels;
            DebugRenderer.size = size;
        }

        public static void AddText(string text, string tag)
        {
            drawText = true;
            if (strings.ContainsKey(tag))
            {
                strings[tag] = text;
            }
            else
            {
                strings.Add(tag, text);
            }
        }

        public static void Draw()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            if (drawPixels)
            {
                graphicsDevice.Textures[0] = null;
                canvas.SetData<UInt32>(pixels, 0, pixels.Length);
                spriteBatch.Draw(canvas, size, Color.White);
            }

            if (drawText)
            {
                StringBuilder displayString = new StringBuilder();
                foreach (string key in strings.Keys)
                {
                    displayString.Append("--- ");
                    displayString.Append(key);
                    displayString.Append(" ---\n");
                    displayString.Append(strings[key]);
                    displayString.Append("\n\n");
                }
                spriteBatch.DrawString(font, displayString.ToString(), new Vector2(10, 10), Color.White);
            }
            spriteBatch.End();
        }
    }
}
