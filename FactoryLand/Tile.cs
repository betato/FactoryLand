using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class Tile
    {
        public static readonly int SIZE = 64;

        public bool Land { get; set; }

        public Tile(bool land)
        {
            Land = land;
        }
    }
}
