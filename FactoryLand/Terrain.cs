using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class Terrain
    {
        private Generator generator;
        private IntIndexArray<IntIndexArray<Chunk>> chunks = new IntIndexArray<IntIndexArray<Chunk>>();

        // Terrain bounds defined by viewport
        private int lastXMin, lastXMax, lastYMin, lastYMax;

        public Terrain()
        {
            generator = new Generator(0);
            for (int y = -9; y < 9; y++)
            {
                for (int x = -9; x < 9; x++)
                {
                    Chunk chunk = new Chunk(new Point(x, y));
                    generator.GenerateChunk(chunk);
                    AddChunk(x, y, chunk);
                }
            }
            for (int y = -9; y < 9; y++)
            {
                for (int x = -9; x < 9; x++)
                {
                    GetChunk(x, y).CalculateGraphicsTiles(GetChunk(x, y - 1), GetChunk(x - 1, y), GetChunk(x - 1, y - 1));
                }
            }

            GetChunk(0, 0).UpdateGraphics();
        }
        
        private class IntIndexArray<T>
        {
            private const UInt32 INITIAL_LENGTH = 32;
            
            private T[] positive = new T[INITIAL_LENGTH];
            private T[] negative = new T[INITIAL_LENGTH];

            public T this[int index]
            {
                get
                {
                    if (index >= 0)
                    {
                        if (index < positive.Length)
                        {
                            return positive[index];
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                    else
                    {
                        index = -index;
                        if (index < negative.Length)
                        {
                            return negative[index];
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }
                set
                {
                    if (index >= 0)
                    {
                        if (index < positive.Length)
                        {
                            positive[index] = value;
                        }
                        else
                        {
                            Array.Resize<T>(ref positive, positive.Length * 2);
                        }
                    }
                    else
                    {
                        index = -index;
                        if (index < negative.Length)
                        {
                            negative[index] = value;
                        }
                        else
                        {
                            Array.Resize<T>(ref negative, negative.Length * 2);
                        }
                    }
                }
            }
        }

        private void AddChunk(int x, int y, Chunk chunk)
        {
            if (chunks[y] == null)
            {
                IntIndexArray<Chunk> row = new IntIndexArray<Chunk>();
                row[x] = chunk;
                chunks[y] = row;
            }
            else
            {
                chunks[y][x] = chunk;
            }
        }
        
        private Chunk GetChunk(int x, int y)
        {
            IntIndexArray<Chunk> row = chunks[y];
            if (row == null)
            {
                return null;
            }
            else
            {
                return row[x];
            }
        }

        private void UpdateChunkGroup(int xMin, int xMax, int yMin, int yMax)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    GetChunk(x, y).UpdateGraphics();
                    Console.WriteLine("Chunk graphics update at X:" + x + " Y:" + y);
                }
            }
        }

        public void UpdateChunkGraphics(Vector4 bounds)
        {
            // Add one to top and right to remove half tile gap 
            bounds.Z++;
            bounds.W++;
            bounds /= Chunk.SIZE;

            int xMin = (int)Math.Floor(bounds.X);
            int yMin = (int)Math.Floor(bounds.Y);
            int xMax = (int)Math.Ceiling(bounds.Z);
            int yMax = (int)Math.Ceiling(bounds.W);

            DebugRenderer.AddText(String.Format("X:{0} Y:{1} X:{2} Y:{3}", xMin, yMin, xMax, yMax), "CHUNKY");

            // Update newly loaded chunk graphics
            UpdateChunkGroup(xMin, lastXMin - 1, yMin, yMax);
            UpdateChunkGroup(lastXMax + 1, xMax, yMin, yMax);
            UpdateChunkGroup(xMin, xMax, yMin, lastYMin - 1);
            UpdateChunkGroup(xMin, xMax, lastYMax + 1, yMax);

            lastXMin = xMin;
            lastYMin = yMin;
            lastXMax = xMax;
            lastYMax = yMax;
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            effect.TextureEnabled = true;
            effect.Texture = TextureManager.GetTexture(TextureId.Terrain);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;

            for (int y = lastYMin; y <= lastYMax; y++)
            {
                for (int x = lastXMin; x <= lastXMax; x++)
                {
                    GetChunk(x, y).Draw(graphicsDevice, effect);
                }
            }

            rasterizerState.Dispose();
        }
    }
}
