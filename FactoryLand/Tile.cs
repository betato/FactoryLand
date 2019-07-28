using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class Tile
    {
        public const int PIXEL_LENGTH = 64;

        public bool Land { get; set; }

        public Tile(bool land)
        {
            Land = land;
        }
    }
}
