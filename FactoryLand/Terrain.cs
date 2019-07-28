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

        private VertexPositionColorTexture[] graphicsTiles;
        short vertexIndex;
        short[] graphicsTileIndices;
        short indiciesIndex;
        
        private int LastXMin, LastXMax, LastYMin, LastYMax;

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
                    CalculateChunkGraphicsTiles(GetChunk(x, y), GetChunk(x, y - 1), GetChunk(x - 1, y), GetChunk(x - 1, y - 1));
                }
            }
        }

        private void CalculateChunkGraphicsTiles(Chunk chunk, Chunk up, Chunk left, Chunk corner)
        {
            if (chunk == null)
            {
                return;
            }

            for (int tileY = 1; tileY < Chunk.SIZE; tileY++)
            {
                for (int tileX = 1; tileX < Chunk.SIZE; tileX++)
                {
                    chunk.GraphicsTiles[tileX, tileY] =
                        (chunk.Tiles[tileX - 1, tileY - 1].Land ? 1 : 0) + (chunk.Tiles[tileX, tileY - 1].Land ? 2 : 0) +
                        (chunk.Tiles[tileX - 1, tileY].Land ? 4 : 0) + (chunk.Tiles[tileX, tileY].Land ? 8 : 0);
                }
            }

            if (up != null)
            {
                for (int tileX = 1; tileX < Chunk.SIZE; tileX++)
                {
                    chunk.GraphicsTiles[tileX, 0] =
                        (up.Tiles[tileX - 1, Chunk.SIZE - 1].Land ? 1 : 0) + (up.Tiles[tileX, Chunk.SIZE - 1].Land ? 2 : 0) +
                        (chunk.Tiles[tileX - 1, 0].Land ? 4 : 0) + (chunk.Tiles[tileX, 0].Land ? 8 : 0);
                }
            }
            else
            {
                for (int tileX = 1; tileX < Chunk.SIZE; tileX++)
                {
                    chunk.GraphicsTiles[tileX, 0] = -1;
                }
            }

            if (left != null)
            {
                for (int tileY = 1; tileY < Chunk.SIZE; tileY++)
                {
                    chunk.GraphicsTiles[0, tileY] =
                        (left.Tiles[Chunk.SIZE - 1, tileY - 1].Land ? 1 : 0) + (chunk.Tiles[0, tileY - 1].Land ? 2 : 0) +
                        (left.Tiles[Chunk.SIZE - 1, tileY].Land ? 4 : 0) + (chunk.Tiles[0, tileY].Land ? 8 : 0);
                }
            }
            else
            {
                for (int tileY = 1; tileY < Chunk.SIZE; tileY++)
                {
                    chunk.GraphicsTiles[0, tileY] = -1;
                }
            }

            if (up != null && left != null && corner != null)
            {
                chunk.GraphicsTiles[0, 0] =
                    (corner.Tiles[Chunk.SIZE - 1, Chunk.SIZE - 1].Land ? 1 : 0) + (up.Tiles[0, Chunk.SIZE - 1].Land ? 2 : 0) +
                    (left.Tiles[Chunk.SIZE - 1, 0].Land ? 4 : 0) + (chunk.Tiles[0, 0].Land ? 8 : 0);
            }
            else
            {
                chunk.GraphicsTiles[0, 0] = -1;
            }
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

        public void UpdateChunkGraphics(Camera camera)
        {
            int chunkPixelSize = Chunk.SIZE * Tile.SIZE;
            Rectangle viewRect = new Rectangle();//= camera.GetViewRect();
            int xMin = viewRect.X / chunkPixelSize - (viewRect.X < 0 ? 1 : 0);
            int xMax = viewRect.Right / chunkPixelSize + (viewRect.Right > 0 ? 1 : 0);
            int yMin = viewRect.Y / chunkPixelSize - (viewRect.Y < 0 ? 1 : 0);
            int yMax = viewRect.Bottom / chunkPixelSize + (viewRect.Bottom > 0 ? 1 : 0);

            if (LastXMin == xMin && xMax == LastXMax && LastYMin == yMin && LastYMax == yMax)
            {
                //return;
            }

            LastXMin = xMin;
            LastXMax = xMax;
            LastYMin = yMin;
            LastYMax = yMax;

            DebugRenderer.AddText(
                "xMin: " + xMin + " xMax: " + xMax +
                "\nyMin: " + yMin + " yMax: " + yMax, "Terrain Bounds");

            //graphicsTiles = new VertexPositionColorTexture[(xMax - xMin) * (yMax - yMin) * Chunk.SIZE * Chunk.SIZE * 6]; // Fix this size
            //graphicsTiles = new VertexPositionColorTexture[4 * 4 * Chunk.SIZE * Chunk.SIZE * 6]; // Fix this size

            int numGraphicsTiles = 1 * 1 * Chunk.SIZE * Chunk.SIZE;
            graphicsTiles = new VertexPositionColorTexture[numGraphicsTiles * 4];
            graphicsTileIndices = new short[numGraphicsTiles * 6];

            vertexIndex = 0;
            indiciesIndex = 0;

            //for (int chunkY = yMin; chunkY < yMax; chunkY++)
            //{
            //    for (int chunkX = xMin; chunkX < xMax; chunkX++)
            //    {
            for (int chunkY = 0; chunkY < 1; chunkY++)
            {
                for (int chunkX = 0; chunkX < 1; chunkX++)
                {

                    Chunk chunk = GetChunk(chunkX, chunkY);
                    if (chunk != null)
                    {
                        // Corner
                        if (chunk.GraphicsTiles[0, 0] >= 0)
                        {
                            AddTextureSquare(chunk, 0, 0);
                        }

                        //Top
                        if (chunk.GraphicsTiles[0, 1] >= 0)
                        {
                            for (int tileX = 1; tileX < Chunk.SIZE; tileX++)
                            {
                                AddTextureSquare(chunk, tileX, 0);
                            }
                        }

                        // Left
                        if (chunk.GraphicsTiles[1, 0] >= 0)
                        {
                            for (int tileY = 1; tileY < Chunk.SIZE; tileY++)
                            {
                                AddTextureSquare(chunk, 0, tileY);
                            }
                        }

                        // Main tile
                        for (int tileY = 1; tileY < Chunk.SIZE; tileY++)
                        {
                            for (int tileX = 1; tileX < Chunk.SIZE; tileX++)
                            {
                                AddTextureSquare(chunk, tileX, tileY);
                            }
                        }
                    }
                }
            }
            //for (int i = 0; i < 5000; i++)
            //{
            //    Console.WriteLine(graphicsTiles[i * 4].ToString());
            //    Console.WriteLine(graphicsTiles[i * 4 + 1].ToString());
            //    Console.WriteLine(graphicsTiles[i * 4 + 2].ToString());
            //    Console.WriteLine(graphicsTiles[i * 4 + 3].ToString());
            //    Console.WriteLine(graphicsTileIndices[i * 6].ToString());
            //    Console.WriteLine(graphicsTileIndices[i * 6 + 1].ToString());
            //    Console.WriteLine(graphicsTileIndices[i * 6 + 2].ToString());
            //    Console.WriteLine(graphicsTileIndices[i * 6 + 3].ToString());
            //    Console.WriteLine(graphicsTileIndices[i * 6 + 4].ToString());
            //    Console.WriteLine(graphicsTileIndices[i * 6 + 5].ToString());
            //}
        }

        private void AddTextureSquare(Chunk chunk, int x, int y)
        {
            int tileX = (x + chunk.Location.X * Chunk.SIZE) * Tile.SIZE;
            int tileY = (y + chunk.Location.Y * Chunk.SIZE) * Tile.SIZE;
            int tileType = chunk.GraphicsTiles[x, y];

            // Top Left
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX, tileY, 0), Color.White,
                GetTextureMapCoord(tileType, false, false));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            // Top Right
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX + Tile.SIZE, tileY, 0), Color.White,
                GetTextureMapCoord(tileType, false, true));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            // Bottom Left
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX, tileY + Tile.SIZE, 0), Color.White,
                GetTextureMapCoord(tileType, true, false));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            graphicsTileIndices[indiciesIndex++] = (short)(vertexIndex - 2);
            // Bottom Right
            graphicsTiles[vertexIndex] = new VertexPositionColorTexture(
                new Vector3(tileX + Tile.SIZE, tileY + Tile.SIZE, 0), Color.White,
                GetTextureMapCoord(tileType, true, true));
            graphicsTileIndices[indiciesIndex++] = vertexIndex++;
            graphicsTileIndices[indiciesIndex++] = (short)(vertexIndex - 2);
        }

        private Vector2 GetTextureMapCoord(int tile, bool bottom, bool right)
        {
            return new Vector2(tile / 16f + (right ? 0.0615234375f : 0.0009765625f), (bottom ? 1f : 0f));
            //return new Vector2(right ? 0.0625f : 0f, bottom ? 1f : 0f);
        }
        
        public void Draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            effect.TextureEnabled = true;
            effect.Texture = TextureManager.GetTexture(TextureId.Terrain);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;
            
            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), graphicsTileIndices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(graphicsTileIndices);
            graphicsDevice.Indices = indexBuffer;

            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), graphicsTiles.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorTexture>(graphicsTiles);
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, graphicsTiles.Length / 2);
            }

            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            rasterizerState.Dispose();

            //using (VertexBuffer buffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, NUM_VERTICES, BufferUsage.WriteOnly))
            //{
            //    buffer.SetData(vertices);
            //    graphicsDevice.SetVertexBuffer(buffer);
            //    graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, NUM_TRIANGLES_PER_FACE);
            //}


            //foreach (Tuple<int, Vector2> graphicsTile in graphicsTiles)
            //{
            //    spriteBatch.Draw(TextureManager.GetTerrain(graphicsTile.Item1), graphicsTile.Item2, Color.White);
            //}


            //VertexBuffer vertexBuffer;

            //BasicEffect basicEffect;
            ////Matrix world = Matrix.CreateTranslation(0, 0, 0);
            //Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            //Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);
            //basicEffect = new BasicEffect(graphicsDevice);

            //VertexPositionColor[] vertices = new VertexPositionColor[3];
            //vertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            //vertices[1] = new VertexPositionColor(new Vector3(+0.5f, 0, 0), Color.Green);
            //vertices[2] = new VertexPositionColor(new Vector3(-0.5f, 0, 0), Color.Blue);

            //vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            //vertexBuffer.SetData<VertexPositionColor>(vertices);

            ////basicEffect.World = world;
            //basicEffect.View = view;
            //basicEffect.Projection = projection;
            //basicEffect.VertexColorEnabled = true;

            //graphicsDevice.SetVertexBuffer(vertexBuffer);

            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //graphicsDevice.RasterizerState = rasterizerState;

            //foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            //}

        }
    }
}
