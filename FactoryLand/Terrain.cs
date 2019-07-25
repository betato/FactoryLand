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
        private List<Chunk> chunks = new List<Chunk>();

        public Terrain()
        {
            Chunk chunk = new Chunk(new Point(0, 0));
            chunks.Add(chunk);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Chunk chunk in chunks)
            {
                for (int i = 1; i < Chunk.SIZE; i++)
                {
                    for (int j = 1; j < Chunk.SIZE; j++)
                    {
                        spriteBatch.Draw(TextureManager.GetTerrain(
                            (chunk.Tiles[i - 1, j - 1].Land ? 1 : 0) + (chunk.Tiles[i, j - 1].Land ? 2 : 0) +
                            (chunk.Tiles[i - 1, j].Land ? 4 : 0) + (chunk.Tiles[i, j].Land ? 8 : 0)),
                            new Vector2((i + chunk.Location.X * Chunk.SIZE) * Tile.SIZE, (j + chunk.Location.Y * Chunk.SIZE) * Tile.SIZE), Color.White);
                    }
                }
            }
        }
    }
}
