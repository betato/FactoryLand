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
        public const int LOAD_EXTENT = 1;
        public const int UNLOAD_EXTENT = 4;
        public const int GENERARION_EXTENT = 2;

        private Generator generator;
        private IntIndexArray<IntIndexArray<Chunk>> chunks = new IntIndexArray<IntIndexArray<Chunk>>();

        // Terrain bounds defined by viewport
        private int lastXMin, lastXMax, lastYMin, lastYMax;

        public Terrain()
        {
            generator = new Generator(0);
            GenerateChunkGroup(-GENERARION_EXTENT, GENERARION_EXTENT, -GENERARION_EXTENT, GENERARION_EXTENT);
            LoadChunkGroupGraphics(-LOAD_EXTENT, LOAD_EXTENT, -LOAD_EXTENT, LOAD_EXTENT);
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
                        while (index >= positive.Length)
                        {
                            Array.Resize<T>(ref positive, positive.Length * 2);
                        }
                        positive[index] = value;
                    }
                    else
                    {
                        index = -index;
                        while (index >= negative.Length)
                        {
                            Array.Resize<T>(ref negative, negative.Length * 2);
                        }
                        negative[index] = value;
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

        private void LoadChunkGroupGraphics(int xMin, int xMax, int yMin, int yMax)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    Chunk chunk = GetChunk(x, y);
                    if (!chunk.GraphicsLoaded)
                    {
                        chunk.LoadGraphics();
                        Console.WriteLine("Chunk graphics loaded at X:" + x + " Y:" + y);
                    }
                }
            }
        }

        private void UnloadChunkGroupGraphics(int xMin, int xMax, int yMin, int yMax)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    Chunk chunk = GetChunk(x, y);
                    if (chunk != null)
                    {
                        GetChunk(x, y).UnloadGraphics();
                        Console.WriteLine("Chunk graphics unloaded at X:" + x + " Y:" + y);
                    }
                }
            }
        }

        private void GenerateChunkGroup(int xMin, int xMax, int yMin, int yMax)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (GetChunk(x, y) == null)
                    {
                        GenerateChunk(x, y);
                        Console.WriteLine("Chunk generated at X:" + x + " Y:" + y);
                    }
                }
            }
        }

        public void GenerateChunk(int x, int y)
        {
            Chunk chunk = new Chunk(new Point(x, y));
            generator.GenerateChunk(chunk);
            AddChunk(x, y, chunk);

            // Calculate graphics of adjecent chunks
            Chunk n =   GetChunk(x,     y + 1);
            Chunk ne =  GetChunk(x + 1, y + 1);
            Chunk e =   GetChunk(x + 1, y);
            Chunk se =  GetChunk(x + 1, y - 1);
            Chunk s =   GetChunk(x,     y - 1);
            Chunk sw =  GetChunk(x - 1, y - 1);
            Chunk w =   GetChunk(x - 1, y);
            Chunk nw =  GetChunk(x - 1, y + 1);

            chunk.CalculateAllGraphicsTiles(s, w, sw);
            if (n != null)
            {
                n.CalculateBottomGraphics(chunk);
                n.CalculateCornerGraphics(chunk, nw, w);
                n.UpdateGraphics();
            }
            if (ne != null)
            {
                ne.CalculateCornerGraphics(e, n, chunk);
                ne.UpdateGraphics();
            }
            if (e != null)
            {
                e.CalculateSideGraphics(chunk);
                e.CalculateCornerGraphics(se, chunk, s);
                e.UpdateGraphics();
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

            // Unload old chunk graphics that have moved beyond the unload distance
            UnloadChunkGroupGraphics(lastXMin + 1 - UNLOAD_EXTENT,  xMin - UNLOAD_EXTENT,           lastYMin - UNLOAD_EXTENT,       lastYMax + UNLOAD_EXTENT);
            UnloadChunkGroupGraphics(xMax + UNLOAD_EXTENT,          lastXMax - 1 + UNLOAD_EXTENT,   lastYMin - UNLOAD_EXTENT,       lastYMax + UNLOAD_EXTENT);
            UnloadChunkGroupGraphics(lastXMin - UNLOAD_EXTENT,      lastXMax + UNLOAD_EXTENT,       lastYMin + 1 - UNLOAD_EXTENT,   yMin - UNLOAD_EXTENT);
            UnloadChunkGroupGraphics(lastXMin - UNLOAD_EXTENT,      lastXMax + UNLOAD_EXTENT,       yMax + UNLOAD_EXTENT,           lastYMax - 1 + UNLOAD_EXTENT);

            // Generate new chunks
            GenerateChunkGroup(xMin - GENERARION_EXTENT,            lastXMin - 1 - GENERARION_EXTENT,   yMin - GENERARION_EXTENT,           yMax + GENERARION_EXTENT);
            GenerateChunkGroup(lastXMax + 1 + GENERARION_EXTENT,    xMax + GENERARION_EXTENT,           yMin - GENERARION_EXTENT,           yMax + GENERARION_EXTENT);
            GenerateChunkGroup(xMin - GENERARION_EXTENT,            xMax + GENERARION_EXTENT,           yMin - GENERARION_EXTENT,           lastYMin - 1 - GENERARION_EXTENT);
            GenerateChunkGroup(xMin - GENERARION_EXTENT,            xMax + GENERARION_EXTENT,           lastYMax + 1 + GENERARION_EXTENT,   yMax + GENERARION_EXTENT);

            // Load chunk graphics
            LoadChunkGroupGraphics(xMin - LOAD_EXTENT,          lastXMin - 1 - LOAD_EXTENT, yMin - LOAD_EXTENT,         yMax + LOAD_EXTENT);
            LoadChunkGroupGraphics(lastXMax + 1 + LOAD_EXTENT,  xMax + LOAD_EXTENT,         yMin - LOAD_EXTENT,         yMax + LOAD_EXTENT);
            LoadChunkGroupGraphics(xMin - LOAD_EXTENT,          xMax + LOAD_EXTENT,         yMin - LOAD_EXTENT,         lastYMin - 1 - LOAD_EXTENT);
            LoadChunkGroupGraphics(xMin - LOAD_EXTENT,          xMax + LOAD_EXTENT,         lastYMax + 1 + LOAD_EXTENT, yMax + LOAD_EXTENT);

            lastXMin = xMin;
            lastYMin = yMin;
            lastXMax = xMax;
            lastYMax = yMax;
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;
            effect.TextureEnabled = true;

            effect.Texture = TextureManager.GetTexture(TextureId.Land);
            DrawLayer(1, graphicsDevice, effect);
            effect.Texture = TextureManager.GetTexture(TextureId.Mountain);
            DrawLayer(2, graphicsDevice, effect);

            rasterizerState.Dispose();
        }

        public void DrawLayer(int layer, GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            for (int y = lastYMin; y <= lastYMax; y++)
            {
                for (int x = lastXMin; x <= lastXMax; x++)
                {
                    GetChunk(x, y).Draw(layer, graphicsDevice, effect);
                }
            }
        }
    }
}
