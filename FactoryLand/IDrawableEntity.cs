using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryLand
{
    interface IDrawableEntity
    {
        void GetVertexData(out VertexPositionColorTexture[] verticies, out short[] indicies, out Texture2D texture);
    }
}
