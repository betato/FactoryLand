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

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera)
        {
            int chunkPixelSize = Chunk.SIZE * Tile.SIZE;
            Rectangle viewRect = camera.GetViewRect();
            int xMin = viewRect.X / chunkPixelSize - (viewRect.X < 0 ? 1 : 0);
            int xMax = viewRect.Right / chunkPixelSize + (viewRect.Right > 0 ? 1 : 0);
            int yMin = viewRect.Y / chunkPixelSize - (viewRect.Y < 0 ? 1 : 0);
            int yMax = viewRect.Bottom / chunkPixelSize + (viewRect.Bottom > 0 ? 1 : 0);

            DebugRenderer.AddText(
                "xMin: " + xMin + " xMax: " + xMax + 
                "\nyMin: " + yMin + " yMax: " + yMax, "Terrain Bounds");

            for (int chunkY = yMin; chunkY < yMax; chunkY++)
            {
                for (int chunkX = xMin; chunkX < xMax; chunkX++)
                {
                    Chunk chunk = GetChunk(chunkX, chunkY);
                    if (chunk != null)
                    {
                        // Draw chunk
                        for (int tileY = 1; tileY < Chunk.SIZE; tileY++)
                        {
                            for (int tileX = 1; tileX < Chunk.SIZE; tileX++)
                            {
                                spriteBatch.Draw(TextureManager.GetTerrain(
                                    (chunk.Tiles[tileX - 1, tileY - 1].Land ? 1 : 0) + (chunk.Tiles[tileX, tileY - 1].Land ? 2 : 0) +
                                    (chunk.Tiles[tileX - 1, tileY].Land ? 4 : 0) + (chunk.Tiles[tileX, tileY].Land ? 8 : 0)),
                                    new Vector2((tileX + chunk.Location.X * Chunk.SIZE) * Tile.SIZE, (tileY + chunk.Location.Y * Chunk.SIZE) * Tile.SIZE), Color.White);
                            }
                        }

                        // Draw connections to adjacent chunks
                        // Upper and right chunks are used because graphical tiles are drawn from the center of each game tile
                        Chunk up = GetChunk(chunkX, chunkY - 1);
                        if (up != null)
                        {
                            for (int tileX = 1; tileX < Chunk.SIZE; tileX++)
                            {
                                spriteBatch.Draw(TextureManager.GetTerrain(
                                    (up.Tiles[tileX - 1, Chunk.SIZE - 1].Land ? 1 : 0) + (up.Tiles[tileX, Chunk.SIZE - 1].Land ? 2 : 0) +
                                    (chunk.Tiles[tileX - 1, 0].Land ? 4 : 0) + (chunk.Tiles[tileX, 0].Land ? 8 : 0)),
                                    new Vector2((tileX + chunk.Location.X * Chunk.SIZE) * Tile.SIZE, (chunk.Location.Y * Chunk.SIZE) * Tile.SIZE), Color.White);
                            }
                        }

                        Chunk left = GetChunk(chunkX - 1, chunkY);
                        if (left != null)
                        {
                            for (int tileY = 1; tileY < Chunk.SIZE; tileY++)
                            {
                                spriteBatch.Draw(TextureManager.GetTerrain(
                                    (left.Tiles[Chunk.SIZE - 1, tileY - 1].Land ? 1 : 0) + (chunk.Tiles[0, tileY - 1].Land ? 2 : 0) +
                                    (left.Tiles[Chunk.SIZE - 1, tileY].Land ? 4 : 0) + (chunk.Tiles[0, tileY].Land ? 8 : 0)),
                                    new Vector2((chunk.Location.X * Chunk.SIZE) * Tile.SIZE, (tileY + chunk.Location.Y * Chunk.SIZE) * Tile.SIZE), Color.White);
                            }
                        }

                        if (up != null && left != null)
                        {
                            Chunk corner = GetChunk(chunkX - 1, chunkY - 1);
                            if (corner != null)
                            {
                                spriteBatch.Draw(TextureManager.GetTerrain(
                                    (corner.Tiles[Chunk.SIZE - 1, Chunk.SIZE - 1].Land ? 1 : 0) + (up.Tiles[0, Chunk.SIZE - 1].Land ? 2 : 0) +
                                    (left.Tiles[Chunk.SIZE - 1, 0].Land ? 4 : 0) + (chunk.Tiles[0, 0].Land ? 8 : 0)),
                                    new Vector2((chunk.Location.X * Chunk.SIZE) * Tile.SIZE, (chunk.Location.Y * Chunk.SIZE) * Tile.SIZE), Color.White);
                            }
                        }
                    }
                }
            }
        }
    }
}
