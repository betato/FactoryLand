using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FactoryLand
{
    abstract class TileEntity : IDrawableEntity
    {
        public Point Location { get; private set; }
        protected Texture2D texture;
        protected VertexPositionColorTexture[] verticies = new VertexPositionColorTexture[4];
        protected short[] indicies = new short[6];

        public TileEntity(Point location)
        {
            Location = location;
        }

        protected void UpdateGraphics()
        {
            verticies[0] = new VertexPositionColorTexture(new Vector3(Location.X, Location.Y, 0), Color.White, new Vector2(0, 0));
            verticies[1] = new VertexPositionColorTexture(new Vector3(Location.X + 1, Location.Y, 0), Color.White, new Vector2(1, 0));
            verticies[2] = new VertexPositionColorTexture(new Vector3(Location.X, Location.Y + 1, 0), Color.White, new Vector2(0, 1));
            verticies[3] = new VertexPositionColorTexture(new Vector3(Location.X + 1, Location.Y + 1, 0), Color.White, new Vector2(1, 1));

            indicies[0] = 0;
            indicies[1] = 1;
            indicies[2] = 2;
            indicies[3] = 1;
            indicies[4] = 3;
            indicies[5] = 2;
        }

        public void GetVertexData(out VertexPositionColorTexture[] verticies, out short[] indicies, out Texture2D texture)
        {
            verticies = this.verticies;
            indicies = this.indicies;
            texture = this.texture;
        }
    }
}
