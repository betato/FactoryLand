using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class Chunk
    {
        public static readonly int SIZE = 64;
        private static readonly float LAND_THRESHOLD = 0.4f;

        public Point Location { get; set; }
        public Tile[,] Tiles { get; private set; } = new Tile[SIZE, SIZE];

        public Chunk(Point location)
        {
            Location = location;

            for (int i = 0; i < Chunk.SIZE; i++)
            {
                for (int j = 0; j < Chunk.SIZE; j++)
                {
                    Tiles[i, j] = new Tile(Perlin(i, j) >= LAND_THRESHOLD ? true : false);
                }
            }

            Tiles[12, 18] = new Tile(false);
            Tiles[12, 19] = new Tile(false);
            Tiles[12, 20] = new Tile(false);
            Tiles[13, 18] = new Tile(false);
            Tiles[13, 19] = new Tile(false);
        }

        public float Perlin(int x, int y)
        {
            return 1;
        }
    }
}
