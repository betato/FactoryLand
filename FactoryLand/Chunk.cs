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
        public const int SIZE = 64;

        public Point Location { get; set; }
        public Tile[,] Tiles { get; private set; } = new Tile[SIZE, SIZE];

        public Chunk(Point location)
        {
            Location = location;
        }
    }
}
