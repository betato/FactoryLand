using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    enum LayerType
    {
        Water = 0,
        Land = 1,
        Mountain = 2
    }

    class Tile
    {
        public const int PIXEL_LENGTH = 64;
        public const int NUM_LAYER_TYPES = 3;
        
        public bool[] layerTypes = new bool[NUM_LAYER_TYPES];

        public Tile()
        {
            
        }
    }
}
