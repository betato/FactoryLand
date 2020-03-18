using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    class TestTileEntity : TileEntity
    {
        public TestTileEntity(Point location) : base(location)
        {
            texture = TextureManager.GetTexture(TextureId.Test);
            UpdateGraphics();
        }
    }
}
